using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Scripts.Core.Generator
{
    public class LevelPathfinder
    {
        public static List<Vector3> GetCriticalPath(RoomGraph graph, List<Room> rooms)
        {
            if (rooms == null || rooms.Count == 0) return new List<Vector3>();

            Room startRoom = rooms[0];
            Dictionary<Room, Room> cameFrom = new Dictionary<Room, Room>();
            Queue<Room> queue = new Queue<Room>();

            queue.Enqueue(startRoom);
            cameFrom[startRoom] = null;

            Room farthestRoom = startRoom;

            while (queue.Count > 0)
            {
                Room current = queue.Dequeue();
                farthestRoom = current;

                if (graph.Neighbors.TryGetValue(current, out List<Room> neighbors))
                {
                    foreach (var neighbor in neighbors)
                    {
                        if (!cameFrom.ContainsKey(neighbor))
                        {
                            cameFrom[neighbor] = current;
                            queue.Enqueue(neighbor);
                        }
                    }
                }
            }

            List<Vector3> pathPositions = new List<Vector3>();
            Room step = farthestRoom;

            while (step != null)
            {
                pathPositions.Add(new Vector3(step.Center.x, 0, step.Center.y));
                step = cameFrom[step];
            }

            pathPositions.Reverse();
            return pathPositions;
        }

        public static List<Vector3> GetBranchPath(RoomGraph graph, List<Vector3> criticalPathPoints,
            List<Room> allRooms, int desiredSize)
        {
            HashSet<Room> mainPathSet = new HashSet<Room>();
            List<Room> mainPathRooms = new List<Room>();

            foreach (var pos in criticalPathPoints)
            {
                Room r = allRooms.Find(x =>
                    Mathf.Approximately(x.Center.x, pos.x) && Mathf.Approximately(x.Center.y, pos.z));
                if (r != null)
                {
                    mainPathSet.Add(r);
                    mainPathRooms.Add(r);
                }
            }

            var potentialBranchPoints = mainPathRooms.OrderBy(x => UnityEngine.Random.value).ToList();

            foreach (Room branchStart in potentialBranchPoints)
            {
                if (!graph.Neighbors.TryGetValue(branchStart, out var neighbors)) continue;

                foreach (Room neighbor in neighbors)
                {
                    // If this neighbor is NOT on the main path, it's a candidate for a side quest
                    if (!mainPathSet.Contains(neighbor))
                    {
                        List<Room> branch = new List<Room>();
                        HashSet<Room> visited = new HashSet<Room>(mainPathSet); // Don't allow re-entering main path

                        if (FindPathRecursive(graph, neighbor, visited, branch, desiredSize))
                        {
                            // Found a valid branch! Convert to Vector3
                            return branch.Select(r => new Vector3(r.Center.x, 0, r.Center.y)).ToList();
                        }
                    }
                }
            }

            return new List<Vector3>();
        }

        // Recursive DFS
        private static bool FindPathRecursive(RoomGraph graph, Room current, HashSet<Room> visited, List<Room> path, int targetLength)
        {
            if (path.Count >= targetLength)
            {
                return true; // We reached the desired size
            }

            visited.Add(current);
            path.Add(current);

            if (graph.Neighbors.TryGetValue(current, out var neighbors))
            {
                foreach (var next in neighbors)
                {
                    if (!visited.Contains(next))
                    {
                        if (FindPathRecursive(graph, next, visited, path, targetLength)) return true;
                    }
                }
            }

            // Backtrack if this route didn't work
            path.RemoveAt(path.Count - 1);
            // We don't remove from 'visited' strictly here because if it was a dead end for this path, it's a dead end for others too in this specific tree check
            return false;
        }
    }
}