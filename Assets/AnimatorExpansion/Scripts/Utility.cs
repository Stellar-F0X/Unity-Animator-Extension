using System;
using UnityEngine;

namespace AnimatorExpansion
{
    public static class Utility
    {
        public static int StringToHash(string sentence)
        {
            return sentence.GetHashCode(StringComparison.Ordinal);
        }
    }
}