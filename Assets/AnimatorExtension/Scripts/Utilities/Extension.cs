using System;
using UnityEngine;

namespace AnimatorExtension
{
    public static class Extension
    {
        public static int StringToHash(string sentence)
        {
            if (string.IsNullOrEmpty(sentence) || string.IsNullOrWhiteSpace(sentence))
            {
                return 0;
            }
            
            return sentence.GetHashCode(StringComparison.Ordinal);
        }


        public static bool Compare(this string a, string b)
        {
            return string.Compare(a, b, StringComparison.Ordinal) == 0;
        }
    }
}