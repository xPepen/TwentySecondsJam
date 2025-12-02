using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Scripts.Core.Generator
{
    public class RoomMeshGenerator
    {
        private readonly LevelGenConfig _config;
        private readonly GameObject _container;

        public RoomMeshGenerator(LevelGenConfig config)
        {
            this._config = config;
            this._container = new GameObject("Rooms");
        }

        public void Generate(RoomGraph graph)
        {
            foreach (var room in graph.Rooms)
            {
                ProcessRoom(room, graph.Neighbors[room]);
            }
        }

        private void ProcessRoom(Room room, List<Room> neighbors)
        {
            Rect localRect = new Rect(
                -room.Rect.width / 2f,
                -room.Rect.height / 2f,
                room.Rect.width,
                room.Rect.height
            );

            //Prepare Job Data
            NativeList<float3> verts = new NativeList<float3>(Allocator.TempJob);
            NativeList<float2> uvs = new NativeList<float2>(Allocator.TempJob);
            NativeList<int> floorTris = new NativeList<int>(Allocator.TempJob);
            NativeList<int> wallTris = new NativeList<int>(Allocator.TempJob);
            // NEW: Allocator for Roof
            NativeList<int> roofTris = new NativeList<int>(Allocator.TempJob);

            int4 neighborFlags = new int4(
                HasNeighbor(room, neighbors, Vector2.up) ? 1 : 0,
                HasNeighbor(room, neighbors, Vector2.down) ? 1 : 0,
                HasNeighbor(room, neighbors, Vector2.right) ? 1 : 0,
                HasNeighbor(room, neighbors, Vector2.left) ? 1 : 0
            );

            RoomGeometryJob job = new RoomGeometryJob
            {
                Rect = localRect,
                WallHeight = _config.WallHeight,
                DoorHeight = _config.DoorHeight,
                DoorWidth = _config.DoorWidth,
                NeighborFlags = neighborFlags,
                Vertices = verts,
                UVs = uvs,
                FloorTriangles = floorTris,
                WallTriangles = wallTris,
                RoofTriangles = roofTris // Assign new list
            };

            //Schedule and Complete Job
            JobHandle handle = job.Schedule();
            handle.Complete();

            // Create Unity Object (Main Thread)
            // //note: we could only create the object once in a future iteration
            GameObject roomObj = LevelObjectFactory.CreateRoomObject(
                _config,
                $"Room_{room.Rect.center}",
                _container.transform
            );

            roomObj.transform.position = new Vector3(room.Rect.center.x, 0, room.Rect.center.y);
            room.RoomObject = roomObj;

            //Assign Mesh
            Mesh mesh = new Mesh();

            // Set indices for 3 submeshes (Floor, Wall, Roof)
            mesh.SetVertices(verts.AsArray());
            mesh.SetUVs(0, uvs.AsArray());

            mesh.subMeshCount = 3; // Increase count

            mesh.SetTriangles(floorTris.AsArray().ToArray(), 0);
            mesh.SetTriangles(wallTris.AsArray().ToArray(), 1);
            mesh.SetTriangles(roofTris.AsArray().ToArray(), 2); // Set roof tris

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            roomObj.GetComponent<MeshFilter>().mesh = mesh;
            roomObj.GetComponent<MeshCollider>().sharedMesh = mesh;

            //Cleanup Native Arrays
            verts.Dispose();
            uvs.Dispose();
            floorTris.Dispose();
            wallTris.Dispose();
            roofTris.Dispose(); // Dispose new list
        }

        private bool HasNeighbor(Room r, List<Room> neighbors, Vector2 dir)
        {
            foreach (var n in neighbors)
            {
                Vector2 diff = (n.Center - r.Center).normalized;
                if (Vector2.Dot(diff, dir) > 0.8f) return true;
            }

            return false;
        }
    }
}