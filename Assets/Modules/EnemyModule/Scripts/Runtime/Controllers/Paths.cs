using Modules.EnemyModule.Models;
using UnityEngine;
using UnityEngine.AI;

namespace Modules.EnemyModule.Controllers
{
    /// <summary>
    /// The next point a walker heads for on its way to a target: a corner of its NavMesh path, searched
    /// again now and then or when the target moves off. Without a NavMesh, or past the end of a path
    /// that stops short, it is the target itself - which is how a sealed base gets a wall broken.
    /// </summary>
    internal static class Paths
    {
        private const float SEARCH_INTERVAL = 0.5f;
        private const float TARGET_MOVED = 2f;
        private const float CORNER_REACHED = 0.4f;
        private const float SAMPLE_DISTANCE = 3f;

        private static readonly NavMeshPath Scratch = new();

        public static Vector3 Waypoint(IEnemyModel model, PathRVO path, Vector3 from, Vector3 target)
        {
            if (!model.HasNavMesh)
                return target;

            if (Time.time >= path.NextSearchTime || Flat(target - path.Target).sqrMagnitude > TARGET_MOVED * TARGET_MOVED)
                Search(path, from, target);

            while (path.Index < path.Corners.Length && Flat(path.Corners[path.Index] - from).sqrMagnitude < CORNER_REACHED * CORNER_REACHED)
                path.Index++;

            return path.Index < path.Corners.Length ? path.Corners[path.Index] : target;
        }

        private static void Search(PathRVO path, Vector3 from, Vector3 target)
        {
            path.Target = target;
            path.NextSearchTime = Time.time + SEARCH_INTERVAL + Random.value * 0.3f;
            path.Index = 1;

            if (NavMesh.SamplePosition(from, out NavMeshHit start, SAMPLE_DISTANCE, NavMesh.AllAreas)
                && NavMesh.SamplePosition(target, out NavMeshHit end, SAMPLE_DISTANCE, NavMesh.AllAreas)
                && NavMesh.CalculatePath(start.position, end.position, NavMesh.AllAreas, Scratch))
            {
                path.Corners = Scratch.corners;
                return;
            }

            path.Corners = System.Array.Empty<Vector3>();
        }

        private static Vector3 Flat(Vector3 v) => new(v.x, 0f, v.z);
    }
}
