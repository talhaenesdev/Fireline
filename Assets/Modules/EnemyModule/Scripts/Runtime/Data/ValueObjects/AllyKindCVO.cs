using System;
using UnityEngine;

namespace Modules.EnemyModule.Data.ValueObjects
{
    /// <summary>One kind of soldier a barracks trains: how it holds up, moves and shoots, and how it looks.</summary>
    [Serializable]
    public class AllyKindCVO
    {
        public string Name = "Tüfekçi";
        public int Health = 120;
        public float MoveSpeed = 3.2f;
        public int Damage = 12;
        public float Range = 9f;
        public float FireInterval = 0.6f;
        public Color Tint = new(0.35f, 0.6f, 1f, 1f);
    }
}
