using System;
using UnityEngine;

namespace BlockBlast.Data
{
    /// <summary>One piece shape and how often it is dealt relative to the others.</summary>
    [Serializable]
    public struct ShapeDef
    {
        [Tooltip("Rows separated by '/', top row first. '#' filled, '.' empty.")]
        public string pattern;
        [Tooltip("Relative chance of being dealt; 0 = never.")]
        [Min(0f)] public float weight;

        public ShapeDef(string pattern, float weight)
        {
            this.pattern = pattern;
            this.weight = weight;
        }
    }
}
