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

            GameObject corridor = LevelObjectFactory.CreateCorridorObject(_config,
                $"Corridor_{roomA.Center}_{roomB.Center}",
                _container.transform
            );

            Mesh mesh = GenerateStraightCorridorMesh(startPos, endPos, dir);

            corridor.GetComponent<MeshFilter>().mesh = mesh;
            corridor.GetComponent<MeshCollider>().sharedMesh = mesh;
        }

        // ... SpawnDoors, GetHeight, InstantiateDoor, GetDoorPosition remain unchanged ...
        // (Copy from original file or keep as is)

        private void SpawnDoors(Room roomA, Room roomB)
        {
            // ... existing implementation ...
            if (_config.DoorPrefab == null)
            {
                Debug.LogError("DoorPrefab is missing in the LevelGenConfig! Please assign a prefab.");
                return;
            }

            Vector2 dir2D = (roomB.Center - roomA.Center).normalized;
            Vector3 startPos = GetDoorPosition(roomA, dir2D);
            Vector3 endPos = GetDoorPosition(roomB, -dir2D);
            Vector3 wallNormal = new Vector3(-dir2D.y, 0, dir2D.x);
            Quaternion baseRotation = Quaternion.LookRotation(wallNormal, Vector3.up);

            InstantiateDoor(roomA, roomB, startPos, baseRotation);
            InstantiateDoor(roomA, roomB, endPos, baseRotation);
        }

        float GetHeight(GameObject go)
        {
            Renderer r = go.GetComponent<Renderer>();
            if (!r)
            {
                r = go.GetComponentInChildren<Renderer>();
                if (!r) return 0f;
            }

            return r.bounds.size.y;
        }

        private void InstantiateDoor(Room roomA, Room roomB, Vector3 position, Quaternion rotation)
        {
            Vector3 finalPos = position;
            float height = GetHeight(_config.DoorPrefab.gameObject);
            finalPos.y = position.y + height * 0.5f;
            Door doorInstance = Object.Instantiate(_config.DoorPrefab, finalPos, rotation, _container.transform);
            doorInstance.Initialize(roomA, roomB);
            Subsystem.Get<LightAndShadowSubsystem>().Subscribe(doorInstance.gameObject);
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
            List<int> roofTris = new List<int>();
        
            float w = _config.DoorWidth * 0.5f;
            // ----------------------------------------------------------------------------------
            // FIX: Use DoorHeight for the corridor's internal dimensions (walls and roof height)
            // ----------------------------------------------------------------------------------
            float h = _config.DoorHeight;
        
            // We can keep the WallHeight for context if needed, but the tunnel mesh uses DoorHeight
            // float wallHeight = _config.WallHeight; 
        
            // Calculate Right Vector (Perpendicular to direction)
            Vector3 right = new Vector3(-dir2D.y, 0, dir2D.x);
        
            // --- FLOOR ---
            verts.Add(start - right * w);
            verts.Add(start + right * w);
            verts.Add(end - right * w);
            verts.Add(end + right * w);
        
            float len = Vector3.Distance(start, end);
            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(_config.DoorWidth, 0));
            uvs.Add(new Vector2(0, len));
            uvs.Add(new Vector2(_config.DoorWidth, len));
        
            floorTris.AddRange(new int[] { 0, 1, 2, 2, 1, 3 });
        
            // --- WALL 1 (Right Side) ---
            int vBase = verts.Count;
            verts.Add(start - right * w);
            verts.Add(end - right * w);
            verts.Add(start - right * w + Vector3.up * h); // Top Start -> Now at DoorHeight (h)
            verts.Add(end - right * w + Vector3.up * h); // Top End -> Now at DoorHeight (h)
        
            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(len, 0));
            // ----------------------------------------------------------------------------------
            // FIX: Update wall UV V-coordinates to match the new height (h = DoorHeight)
            // ----------------------------------------------------------------------------------
            uvs.Add(new Vector2(0, h));
            uvs.Add(new Vector2(len, h));
        
            wallTris.AddRange(new int[] { vBase + 1, vBase + 2, vBase, vBase + 3, vBase + 2, vBase + 1 });
        
            // --- WALL 2 (Left Side) ---
            vBase = verts.Count;
            verts.Add(start + right * w);
            verts.Add(end + right * w);
            verts.Add(start + right * w + Vector3.up * h); // Top Start -> Now at DoorHeight (h)
            verts.Add(end + right * w + Vector3.up * h); // Top End -> Now at DoorHeight (h)
        
            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(len, 0));
            // ----------------------------------------------------------------------------------
            // FIX: Update wall UV V-coordinates to match the new height (h = DoorHeight)
            // ----------------------------------------------------------------------------------
            uvs.Add(new Vector2(0, h));
            uvs.Add(new Vector2(len, h));
        
            wallTris.AddRange(new int[] { vBase + 1, vBase, vBase + 3, vBase, vBase + 2, vBase + 3 });
        
            // --- ROOF ---
            vBase = verts.Count;
            // All roof vertices correctly use h (now DoorHeight)
            Vector3 roofStartLeft = start - right * w + Vector3.up * h;
            Vector3 roofStartRight = start + right * w + Vector3.up * h;
            Vector3 roofEndLeft = end - right * w + Vector3.up * h;
            Vector3 roofEndRight = end + right * w + Vector3.up * h;
        
            verts.Add(roofStartLeft);
            verts.Add(roofStartRight);
            verts.Add(roofEndLeft);
            verts.Add(roofEndRight);
        
            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(_config.DoorWidth, 0));
            uvs.Add(new Vector2(0, len));
            uvs.Add(new Vector2(_config.DoorWidth, len));
        
            // Winding needs to look down (Counter Clockwise from top)
            roofTris.AddRange(new int[]
            {
                vBase + 3, vBase + 1, vBase + 2,
                vBase + 2, vBase + 1, vBase + 0
            });
        
            mesh.vertices = verts.ToArray();
            mesh.uv = uvs.ToArray();
        
            // Set 3 submeshes
            mesh.subMeshCount = 3;
            mesh.SetTriangles(floorTris.ToArray(), 0);
            mesh.SetTriangles(wallTris.ToArray(), 1);
            mesh.SetTriangles(roofTris.ToArray(), 2);
        
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
        
            return mesh;
        }
        
            private Mesh GenerateStraightCorridorMeshWithSpecialEffect(Vector3 start, Vector3 end, Vector2 dir2D)
            {
                Mesh mesh = new Mesh();
                List<Vector3> verts = new List<Vector3>();
                List<Vector2> uvs = new List<Vector2>();
                List<int> floorTris = new List<int>();
                List<int> wallTris = new List<int>();
                List<int> roofTris = new List<int>();
            
                float w = _config.DoorWidth * 0.5f;
                // Use DoorHeight for the internal space height
                float h = _config.DoorHeight;
            
                // Calculate Right Vector (Perpendicular to direction)
                Vector3 right = new Vector3(-dir2D.y, 0, dir2D.x);
            
                // --- FLOOR ---
                verts.Add(start - right * w); // 0
                verts.Add(start + right * w); // 1
                verts.Add(end - right * w); // 2
                verts.Add(end + right * w); // 3
            
                float len = Vector3.Distance(start, end);
                uvs.Add(new Vector2(0, 0));
                uvs.Add(new Vector2(_config.DoorWidth, 0));
                uvs.Add(new Vector2(0, len));
                uvs.Add(new Vector2(_config.DoorWidth, len));
            
                floorTris.AddRange(new int[] { 0, 1, 2, 2, 1, 3 });
            
                // --- WALL 1 (Right Side - Looking along the corridor) ---
                int vBase = verts.Count;
                verts.Add(start - right * w); // vBase + 0: Bottom Start
                verts.Add(end - right * w); // vBase + 1: Bottom End
                verts.Add(start - right * w + Vector3.up * h); // vBase + 2: Top Start
                verts.Add(end - right * w + Vector3.up * h); // vBase + 3: Top End
            
                // UVs are correctly calculated for the new height h
                uvs.Add(new Vector2(0, 0));
                uvs.Add(new Vector2(len, 0));
                uvs.Add(new Vector2(0, h));
                uvs.Add(new Vector2(len, h));
            
                // FORWARD WINDING (Normal direction)
                wallTris.AddRange(new int[] { vBase + 1, vBase + 2, vBase, vBase + 3, vBase + 2, vBase + 1 });
            
                // REVERSE WINDING (Double-Sided for interior clipping fix)
                wallTris.AddRange(new int[]
                {
                    vBase, vBase + 2, vBase + 1,
                    vBase + 1, vBase + 2, vBase + 3
                });
            
                // --- WALL 2 (Left Side - Looking along the corridor) ---
                vBase = verts.Count;
                verts.Add(start + right * w); // vBase + 0: Bottom Start
                verts.Add(end + right * w); // vBase + 1: Bottom End
                verts.Add(start + right * w + Vector3.up * h); // vBase + 2: Top Start
                verts.Add(end + right * w + Vector3.up * h); // vBase + 3: Top End
            
                // UVs are correctly calculated for the new height h
                uvs.Add(new Vector2(0, 0));
                uvs.Add(new Vector2(len, 0));
                uvs.Add(new Vector2(0, h));
                uvs.Add(new Vector2(len, h));
            
                // FORWARD WINDING (Normal direction)
                wallTris.AddRange(new int[] { vBase + 1, vBase, vBase + 3, vBase, vBase + 2, vBase + 3 });
            
                // REVERSE WINDING (Double-Sided for interior clipping fix)
                wallTris.AddRange(new int[]
                {
                    vBase + 3, vBase, vBase + 1,
                    vBase + 3, vBase + 2, vBase
                });
            
                // --- ROOF ---
                vBase = verts.Count;
                Vector3 roofStartLeft = start - right * w + Vector3.up * h;
                Vector3 roofStartRight = start + right * w + Vector3.up * h;
                Vector3 roofEndLeft = end - right * w + Vector3.up * h;
                Vector3 roofEndRight = end + right * w + Vector3.up * h;
            
                verts.Add(roofStartLeft); // vBase + 0
                verts.Add(roofStartRight); // vBase + 1
                verts.Add(roofEndLeft); // vBase + 2
                verts.Add(roofEndRight); // vBase + 3
            
                uvs.Add(new Vector2(0, 0));
                uvs.Add(new Vector2(_config.DoorWidth, 0));
                uvs.Add(new Vector2(0, len));
                uvs.Add(new Vector2(_config.DoorWidth, len));
            
                // Roof winding needs to look down (Counter Clockwise from top)
                roofTris.AddRange(new int[]
                {
                    vBase + 3, vBase + 1, vBase + 2,
                    vBase + 2, vBase + 1, vBase + 0
                });
            
                mesh.vertices = verts.ToArray();
                mesh.uv = uvs.ToArray();
            
                // Set 3 submeshes (0=Floor, 1=Wall, 2=Roof)
                mesh.subMeshCount = 3;
                mesh.SetTriangles(floorTris.ToArray(), 0);
                mesh.SetTriangles(wallTris.ToArray(), 1);
                mesh.SetTriangles(roofTris.ToArray(), 2);
            
                mesh.RecalculateNormals();
                mesh.RecalculateBounds();
            
                return mesh;
            }
    }
}