using UnityEngine;
using UnityEngine.AI;

public static class NavMeshUtils
{
    public static bool TryGetRandomPoint(Vector3 center, float range, out Vector3 result)
    {
        var randomPoint = Vector3.zero;
        result = Vector3.zero;

        for (int i = 0; i < 30; i++)
        {
            var rand = Random.insideUnitCircle;

            randomPoint.x = center.x + (rand.x * range);
            randomPoint.y = center.y;
            randomPoint.z = center.z + (rand.y * range);

            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }
        return false;
    }
}