using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnimatorExpansion
{
    [Serializable]
    public struct MinMax : IEquatable<MinMax>
    {
        public MinMax(float min, float max)
        {
            this.min = min;
            this.max = max;
        }
        
        public float min;
        public float max;


        public float middle
        {
            get { return (min + max) * 0.5f; }
        }
        

        public void Flip()
        {
            this = new MinMax(max, min);
        }

        public bool Equals(MinMax other)
        {
            return (other.max - max) < float.Epsilon && (other.min - min) < float.Epsilon;
        }

        public static implicit operator Vector2(MinMax minMax)
        {
            return new Vector2(minMax.min, minMax.max);
        }

        public static implicit operator MinMax(Vector2 vector)
        {
            return (MinMax)vector;
        }
    }
}
