using UnityEngine;
using System.Collections.Generic;
using Game.Scripts.Runtime.LightAndShadow;

namespace Game.Scripts.Core.Generator
{
    public class RoomConnectionGenerator
    {
        private readonly GameObject _container;
        private readonly LevelGenConfig _config;
        
        public RoomConnectionGenerator(LevelGenConfig config)
        {
            this._config = config;
            this._container = new GameObject("Corridors");
        }

        public void ConnectRooms(RoomGraph graph)
        {
            // Keep track of processed connections to avoid duplicates (A->B and B->A)
            HashSet<(Room, Room)> processed = new HashSet<(Room, Room)>();

            foreach (var room in graph.Rooms)
            {
                foreach (var neighbor in graph.Neighbors[room])
                {
                    if (processed.Contains((neighbor, room))) continue;
                    
                    CreateCorridorMesh(room, neighbor);
                    
                    SpawnDoors(room, neighbor); 
                    
                    processed.Add((room, neighbor));
                }
            }
        }

        private void CreateCorridorMesh(Room roomA, Room roomB)
        {
            Vector2 dir = (roomB.Center - roomA.Center).normalized;
            
            Vector3 startPos = GetDoorPosition(roomA, dir);
            Vector3 endPos = GetDoorPosition(roomB, -dir);

            GameObject corridor = LevelObjectFactory.CreateCorridorObject( _config,
                $"Corridor_{roomA.Center}_{roomB.Center}", 
                _container.transform
            );

            Mesh mesh = GenerateStraightCorridorMesh(startPos, endPos, dir);
            
            corridor.GetComponent<MeshFilter>().mesh = mesh;
            corridor.GetComponent<MeshCollider>().sharedMesh = mesh;
        }

        private void SpawnDoors(Room roomA, Room roomB)
        {
            if (_config.DoorPrefab == null)
            {
                Debug.LogError("DoorPrefab is missing in the LevelGenConfig! Please assign a prefab.");
                return;
            }

            Vector2 dir2D = (roomB.Center - roomA.Center).normalized;
            
            // Door position at Room A's boundary
            Vector3 startPos = GetDoorPosition(roomA, dir2D);
            // Door position at Room B's boundary
            Vector3 endPos = GetDoorPosition(roomB, -dir2D);

            // Calculate rotation. The door needs to be aligned with the wall plane.
            Vector3 wallNormal = new Vector3(-dir2D.y, 0, dir2D.x); 
            Quaternion baseRotation = Quaternion.LookRotation(wallNormal, Vector3.up);
            
            // Apply 90-degree correction (adjust based on your prefab's axis)
            Quaternion compensationRotation = Quaternion.Euler(0, 90f, 0);
            Quaternion doorRotation = baseRotation * compensationRotation;

            InstantiateDoor(roomA, roomB, startPos, doorRotation);

            InstantiateDoor(roomA, roomB, endPos, doorRotation);
        }

        private void InstantiateDoor(Room roomA, Room roomB, Vector3 position, Quaternion rotation)
        {
            Vector3 finalPos = position;
            finalPos.y = _config.DoorHeight;
            
            GameObject doorInstance = Object.Instantiate(
                _config.DoorPrefab, 
                finalPos, 
                rotation, 
                _container.transform
            );
            
            // Link the door to the rooms
            DoorLinker linker = doorInstance.GetComponent<DoorLinker>();
            if (!linker)
            {
                linker = doorInstance.AddComponent<DoorLinker>();
            }
            linker.Initialize(roomA, roomB);
            
            EnviroObject enviroObject = doorInstance.AddComponent<EnviroObject>();
            Subsystem.Get<LightAndShadowSubsystem>().Add(enviroObject);
        }

        private Vector3 GetDoorPosition(Room room, Vector2 dir)
        {
            Rect r = room.Rect;
            Vector2 center = r.center;

            if (dir.y > 0.5f) return new Vector3(center.x, 0, r.yMax);
            if (dir.y < -0.5f) return new Vector3(center.x, 0, r.yMin);
            if (dir.x > 0.5f) return new Vector3(r.xMax, 0, center.y);
            if (dir.x < -0.5f) return new Vector3(r.xMin, 0, center.y);

            return new Vector3(center.x, 0, center.y);
        }

        private Mesh GenerateStraightCorridorMesh(Vector3 start, Vector3 end, Vector2 dir2D)
        {
            Mesh mesh = new Mesh();
            List<Vector3> verts = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<int> floorTris = new List<int>();
            List<int> wallTris = new List<int>();

            float w = _config.DoorWidth * 0.5f;
            float h = _config.WallHeight;
            
            // Calculate Right Vector (Perpendicular to direction)
            Vector3 right = new Vector3(-dir2D.y, 0, dir2D.x); 

            // --- FLOOR ---
            // 0: Start Right, 1: Start Left, 2: End Right, 3: End Left
            verts.Add(start - right * w);
            verts.Add(start + right * w);
            verts.Add(end - right * w);
            verts.Add(end + right * w);

            float len = Vector3.Distance(start, end);
            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(_config.DoorWidth, 0));
            uvs.Add(new Vector2(0, len));
            uvs.Add(new Vector2(_config.DoorWidth, len));

            // Floor Winding: 0 -> 1 -> 2 (Clockwise = Faces UP)
            floorTris.AddRange(new int[] { 0, 1, 2, 2, 1, 3 });

            // --- WALL 1 (Right Side - Looking Down Corridor) ---
            int vBase = verts.Count;
            verts.Add(start - right * w); // vBase (0) - Bottom Start
            verts.Add(end - right * w);   // vBase+1 - Bottom End
            verts.Add(start - right * w + Vector3.up * h); // vBase+2 - Top Start
            verts.Add(end - right * w + Vector3.up * h);   // vBase+3 - Top End
            
            // Re-use UVs for simple mapping
            uvs.Add(new Vector2(0,0)); uvs.Add(new Vector2(len,0));
            uvs.Add(new Vector2(0,h)); uvs.Add(new Vector2(len,h));

            // Wall Winding 1: (1 -> 2 -> 0 / 3 -> 2 -> 1) - Faces INWARD
            wallTris.AddRange(new int[] { 
                vBase + 1, vBase + 2, vBase,  
                vBase + 3, vBase + 2, vBase + 1   
            });

            // --- WALL 2 (Left Side - Looking Down Corridor) ---
            vBase = verts.Count;
            verts.Add(start + right * w); // vBase (0) - Bottom Start
            verts.Add(end + right * w);   // vBase+1 - Bottom End
            verts.Add(start + right * w + Vector3.up * h); // vBase+2 - Top Start
            verts.Add(end + right * w + Vector3.up * h);   // vBase+3 - Top End
            
            uvs.Add(new Vector2(0,0)); uvs.Add(new Vector2(len,0));
            uvs.Add(new Vector2(0,h)); uvs.Add(new Vector2(len,h));

            // Wall Winding 2: (1 -> 0 -> 3 / 0 -> 2 -> 3) - Faces INWARD
            wallTris.AddRange(new int[] { 
                vBase + 1, vBase, vBase + 3,  
                vBase, vBase + 2, vBase + 3   
            });

            mesh.vertices = verts.ToArray();
            mesh.uv = uvs.ToArray();
            mesh.subMeshCount = 2;
            mesh.SetTriangles(floorTris.ToArray(), 0);
            mesh.SetTriangles(wallTris.ToArray(), 1);
            
            mesh.RecalculateNormals(); 
            mesh.RecalculateBounds();

            return mesh;
        }
    }
}