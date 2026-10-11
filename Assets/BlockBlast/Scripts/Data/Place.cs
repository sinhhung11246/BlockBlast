using System;
using UnityEngine;

namespace BlockBlast.Data
{
    /// <summary>Where a popup part sits, relative to its popup: position plus world width (sprites) or cap height (text).</summary>
    [Serializable]
    public struct Place
    {
        public Vector2 pos;
        public float size;

        public Place(float x, float y, float size)
        {
            pos = new Vector2(x, y);
            this.size = size;
        }
    }
}
