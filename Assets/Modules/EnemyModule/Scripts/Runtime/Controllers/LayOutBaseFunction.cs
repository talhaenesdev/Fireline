using System.Collections.Generic;
using Modules.EnemyModule.Data.UnityObjects;
using Modules.EnemyModule.Entities;
using UnityEngine;

namespace Modules.EnemyModule.Controllers
{
    /// <summary>
    /// Where the base's walls stand and which way each faces: laid out from the board's CD_BaseLayout
    /// round its centre, or, with no layout, the points authored under the board.
    /// </summary>
    internal class LayOutBaseFunction
    {
        public List<(Vector3 point, float yaw)> Execute(IZombieWaveBoard board)
        {
            var walls = new List<(Vector3, float)>();
            CD_BaseLayout layout = board.BaseLayout;

            if (layout == null)
            {
                for (int i = 0; i < board.BaseWallCount; i++)
                    walls.Add((board.GetBaseWallPoint(i), board.GetBaseWallYaw(i)));
                return walls;
            }

            Vector3 center = board.BaseCenter;
            float half = layout.HalfSize;
            float spacing = Mathf.Max(0.5f, layout.WallSpacing);
            int count = Mathf.FloorToInt(half * 2f / spacing);

            // Slots are centred on each side; the ones inside the gate are left open.
            for (int k = 0; k < count; k++)
            {
                float s = (k - (count - 1) * 0.5f) * spacing;
                if (Mathf.Abs(s) < layout.GateWidth * 0.5f)
                    continue;

                walls.Add((center + new Vector3(s, 0f, half), 0f));
                walls.Add((center + new Vector3(s, 0f, -half), 0f));
                walls.Add((center + new Vector3(half, 0f, s), 90f));
                walls.Add((center + new Vector3(-half, 0f, s), 90f));
            }

            if (!layout.CornerWalls)
                return walls;

            foreach (var corner in new[] { new Vector3(1f, 0f, 1f), new Vector3(-1f, 0f, 1f), new Vector3(1f, 0f, -1f), new Vector3(-1f, 0f, -1f) })
                walls.Add((center + corner * half, 45f));

            return walls;
        }
    }
}
