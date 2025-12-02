using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Scripts.Core.Generator
{
    [BurstCompile]
    public struct RoomGeometryJob : IJob
    {
        // Inputs
        public Rect Rect;
        public float WallHeight;
        public float DoorWidth;
        public float DoorHeight;
        
        public int4 NeighborFlags; 

        // Outputs
        public NativeList<float3> Vertices;
        public NativeList<float2> UVs;
        public NativeList<int> FloorTriangles;
        public NativeList<int> WallTriangles;
        // NEW: Output for roof
        public NativeList<int> RoofTriangles;

        public void Execute()
        {
            // 1. Generate Floor
            GenerateFloor();

            // 2. Generate Walls
            TryGenerateWall(new float2(0, 1), NeighborFlags[0] == 1); // North
            TryGenerateWall(new float2(0, -1), NeighborFlags[1] == 1); // South
            TryGenerateWall(new float2(1, 0), NeighborFlags[2] == 1); // East
            TryGenerateWall(new float2(-1, 0), NeighborFlags[3] == 1); // West
            
            // 3. Generate Roof
            GenerateRoof();
        }

        private void GenerateFloor()
        {
            float xMin = Rect.xMin;
            float xMax = Rect.xMax;
            float yMin = Rect.yMin;
            float yMax = Rect.yMax;

            int startIdx = Vertices.Length;
            Vertices.Add(new float3(xMin, 0, yMin));
            Vertices.Add(new float3(xMax, 0, yMin));
            Vertices.Add(new float3(xMin, 0, yMax));
            Vertices.Add(new float3(xMax, 0, yMax));

            UVs.Add(new float2(0, 0));
            UVs.Add(new float2(Rect.width, 0));
            UVs.Add(new float2(0, Rect.height));
            UVs.Add(new float2(Rect.width, Rect.height));

            // Clockwise winding (Points UP)
            FloorTriangles.Add(startIdx + 0);
            FloorTriangles.Add(startIdx + 2);
            FloorTriangles.Add(startIdx + 1);
            FloorTriangles.Add(startIdx + 2);
            FloorTriangles.Add(startIdx + 3);
            FloorTriangles.Add(startIdx + 1);
        }

        private void GenerateRoof()
        {
            float xMin = Rect.xMin;
            float xMax = Rect.xMax;
            float yMin = Rect.yMin;
            float yMax = Rect.yMax;
            float h = WallHeight;

            // We create new vertices for the roof to ensure flat shading
            // If we shared vertices with walls, the normals might smooth weirdly at the corners
            int startIdx = Vertices.Length;
            Vertices.Add(new float3(xMin, h, yMin)); // 0
            Vertices.Add(new float3(xMax, h, yMin)); // 1
            Vertices.Add(new float3(xMin, h, yMax)); // 2
            Vertices.Add(new float3(xMax, h, yMax)); // 3

            UVs.Add(new float2(0, 0));
            UVs.Add(new float2(Rect.width, 0));
            UVs.Add(new float2(0, Rect.height));
            UVs.Add(new float2(Rect.width, Rect.height));

            // Counter-Clockwise winding (Points DOWN, visible from inside)
            // 0 -> 1 -> 2
            RoofTriangles.Add(startIdx + 0);
            RoofTriangles.Add(startIdx + 1);
            RoofTriangles.Add(startIdx + 2);
            
            // 2 -> 1 -> 3
            RoofTriangles.Add(startIdx + 2);
            RoofTriangles.Add(startIdx + 1);
            RoofTriangles.Add(startIdx + 3);
        }

        // ... (TryGenerateWall, CreateSolidWall, CreateWallWithOpening, GetWallSegments, AddQuad, AddLintelQuad remain unchanged) ...
        // Include the rest of the original file content here for TryGenerateWall downwards
        
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