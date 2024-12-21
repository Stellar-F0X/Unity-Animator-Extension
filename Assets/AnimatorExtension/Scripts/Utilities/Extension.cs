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

        
        public static Animator BindAction(this Animator animator)
        {
            return animator;
        }
    }
}