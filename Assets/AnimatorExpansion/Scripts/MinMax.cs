using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnimatorExpansion
{
    [Serializable]
    public struct MinMax : IEquatable<MinMax>
    {
        public float min;
        public float max;

        public bool HasValue
        {
            get { return min != 0 || max != 0; }
        }

        public Vector2 Vector
        {
            get { return new Vector2(min, max); }
        }

        public MinMax(float min, float max)
        {
            this.min = min;
            this.max = max;
        }
        
        public MinMax Flip()
        {
            return new MinMax(max, min);
        }

        public override string ToString()
        {
            return $"({min}, {max})";
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
