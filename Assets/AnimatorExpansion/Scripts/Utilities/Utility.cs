using System;
using UnityEngine;

namespace AnimatorExpansion
{
    public static class Utility
    {
        public static int StringToHash(string sentence)
        {
            if (string.IsNullOrEmpty(sentence) || string.IsNullOrWhiteSpace(sentence))
            {
                return 0;
            }
            
            return sentence.GetHashCode(StringComparison.Ordinal);
        }
    }
}