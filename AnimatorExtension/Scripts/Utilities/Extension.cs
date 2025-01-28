using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AnimatorExtension
{
    public static class Extension
    {
        public static int StringToHash(string sentence)
        {
            if (string.IsNullOrEmpty(sentence) || string.IsNullOrWhiteSpace(sentence))
            {
                Debug.LogError("[StringToHash] Sentence is empty");
                return -1;
            }

            return sentence.GetHashCode(StringComparison.Ordinal);
        }


        public static bool Compare(this string a, string b)
        {
            return string.Compare(a, b, StringComparison.Ordinal) == 0;
        }


        public static void ForEach<TKey, TElement>(this ILookup<TKey, TElement> lookup, TKey key, Action<TElement> action)
        {
            foreach (var item in lookup[key])
            {
                action.Invoke(item);
            }
        }
        
        
        public static void ForEach<T>(this T[] array, Action<T> action)
        {
            for (int i = 0; i < array.Length; i++)
            {
                action.Invoke(array[i]);
            }
        }
    }
}