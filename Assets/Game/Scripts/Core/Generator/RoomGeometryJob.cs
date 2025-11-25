using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Scripts.Core.Generator
{
    /// <summary>
    /// Burst-compiled job to calculate room geometry logic in parallel.
    /// </summary>
    [BurstCompile]
    public struct RoomGeometryJob : IJob
    {
        // Inputs
        public Rect Rect;
        public float WallHeight;
        public float DoorWidth;
        public float DoorHeight;
        
        // Neighbor info packed: [0]=North, [1]=South, [2]=East, [3]=West. 1 = has neighbor, 0 = no neighbor.
        public int4 NeighborFlags; 

        // Outputs (using NativeList for dynamic sizing)
        public NativeList<float3> Vertices;
        public NativeList<float2> UVs;
        public NativeList<int> FloorTriangles;
        public NativeList<int> WallTriangles;

        public void Execute()
        {
            // 1. Generate Floor
            GenerateFloor();

            // 2. Generate Walls
            // North (+Z)
            TryGenerateWall(new float2(0, 1), NeighborFlags[0] == 1);
            // South (-Z)
            TryGenerateWall(new float2(0, -1), NeighborFlags[1] == 1);
            // East (+X)
            TryGenerateWall(new float2(1, 0), NeighborFlags[2] == 1);
            // West (-X)
            TryGenerateWall(new float2(-1, 0), NeighborFlags[3] == 1);
        }

        private void GenerateFloor()
        {
            float xMin = Rect.xMin;
            float xMax = Rect.xMax;
            float yMin = Rect.yMin;
            float yMax = Rect.yMax;

            // Vertices
            int startIdx = Vertices.Length;
            Vertices.Add(new float3(xMin, 0, yMin));
            Vertices.Add(new float3(xMax, 0, yMin));
            Vertices.Add(new float3(xMin, 0, yMax));
            Vertices.Add(new float3(xMax, 0, yMax));

            // UVs
            UVs.Add(new float2(0, 0));
            UVs.Add(new float2(Rect.width, 0));
            UVs.Add(new float2(0, Rect.height));
            UVs.Add(new float2(Rect.width, Rect.height));

            // Triangles
            FloorTriangles.Add(startIdx + 0);
            FloorTriangles.Add(startIdx + 2);
            FloorTriangles.Add(startIdx + 1);
            FloorTriangles.Add(startIdx + 2);
            FloorTriangles.Add(startIdx + 3);
            FloorTriangles.Add(startIdx + 1);
        }

        private void TryGenerateWall(float2 dir, bool hasNeighbor)
        {
            if (hasNeighbor)
                CreateWallWithOpening(dir);
            else
                CreateSolidWall(dir);
        }

        private void CreateSolidWall(float2 dir)
        {
            GetWallSegments(dir, out float3 start, out float3 end);
            AddQuad(start, end, WallHeight);
        }

        private void CreateWallWithOpening(float2 dir)
        {
            GetWallSegments(dir, out float3 start, out float3 end);
            
            float3 mid = (dir.x != 0) ? new float3(dir.x > 0 ? Rect.xMax : Rect.xMin, 0, Rect.center.y) 
                                      : new float3(Rect.center.x, 0, dir.y > 0 ? Rect.yMax : Rect.yMin);

            float3 wallDir = math.normalize(end - start);
            float halfDoor = DoorWidth * 0.5f;

            float3 doorStartPos = mid - wallDir * halfDoor;
            float3 doorEndPos = mid + wallDir * halfDoor;

            AddQuad(start, doorStartPos, WallHeight);
            AddQuad(doorEndPos, end, WallHeight);

            float3 vDoorStartBottom = doorStartPos + new float3(0, DoorHeight, 0);
            float3 vDoorEndBottom = doorEndPos + new float3(0, DoorHeight, 0);
            
            // We use (WallHeight - DoorHeight) as the height of the lintel quad
            // But AddQuad expects absolute height from base, so we construct manually or offset
            AddLintelQuad(vDoorStartBottom, vDoorEndBottom, WallHeight - DoorHeight);
        }

        private void GetWallSegments(float2 dir, out float3 start, out float3 end)
        {
            if (dir.y > 0.5f) // North
            {
                start = new float3(Rect.xMax, 0, Rect.yMax);
                end = new float3(Rect.xMin, 0, Rect.yMax);
            }
            else if (dir.y < -0.5f) // South
            {
                start = new float3(Rect.xMin, 0, Rect.yMin);
                end = new float3(Rect.xMax, 0, Rect.yMin);
            }
            else if (dir.x > 0.5f) // East
            {
                start = new float3(Rect.xMax, 0, Rect.yMin);
                end = new float3(Rect.xMax, 0, Rect.yMax);
            }
            else // West
            {
                start = new float3(Rect.xMin, 0, Rect.yMax);
                end = new float3(Rect.xMin, 0, Rect.yMin);
            }
        }

        private void AddQuad(float3 start, float3 end, float height)
        {
            int idx = Vertices.Length;
            Vertices.Add(start);
            Vertices.Add(end);
            Vertices.Add(start + new float3(0, height, 0));
            Vertices.Add(end + new float3(0, height, 0));

            float len = math.distance(start, end);
            UVs.Add(new float2(0, 0));
            UVs.Add(new float2(len, 0));
            UVs.Add(new float2(0, height));
            UVs.Add(new float2(len, height));

            WallTriangles.Add(idx);
            WallTriangles.Add(idx + 1);
            WallTriangles.Add(idx + 2);
            WallTriangles.Add(idx + 1);
            WallTriangles.Add(idx + 3);
            WallTriangles.Add(idx + 2);
        }

        private void AddLintelQuad(float3 startBase, float3 endBase, float lintelHeight)
        {
            int idx = Vertices.Length;
            Vertices.Add(startBase);
            Vertices.Add(endBase);
            Vertices.Add(startBase + new float3(0, lintelHeight, 0));
            Vertices.Add(endBase + new float3(0, lintelHeight, 0));

            float len = math.distance(startBase, endBase);
            UVs.Add(new float2(0, 0));
            UVs.Add(new float2(len, 0));
            UVs.Add(new float2(0, lintelHeight));
            UVs.Add(new float2(len, lintelHeight));

            WallTriangles.Add(idx);
            WallTriangles.Add(idx + 1);
            WallTriangles.Add(idx + 2);
            WallTriangles.Add(idx + 1);
            WallTriangles.Add(idx + 3);
            WallTriangles.Add(idx + 2);
        }
    }
}