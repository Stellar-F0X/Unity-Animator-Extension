using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        
        #region Referenced By https://discussions.unity.com/t/current-animator-state-name/584039/19

        private const BindingFlags _BINDING_FLAGS = BindingFlags.Instance | BindingFlags.NonPublic;
        
        private static Func<Animator, int, string> _getCurrentStateName;
        
        private static Func<Animator, int, string> _getNextStateName;
        
        private static Func<Animator, int, string> _resolveHash;
        
        /// <summary>Gets an instance method with single argument of type <typeparamref
        /// name="TArg0"/> and return type of <typeparamref name="TReturn"/> from <typeparamref
        /// name="TThis"/> and compiles it into a fast open delegate.</summary>
        /// <typeparam name="TThis">Type of the class owning the instance method.</typeparam>
        /// <typeparam name="TArg0">Type of the single parameter to the instance method to
        /// find.</typeparam>
        /// <typeparam name="TReturn">Type of the return for the method</typeparam>
        /// <param name="methodName">The name of the method the compile.</param>
        /// <returns>The compiled delegate, which should be about as fast as calling the function
        /// directly on the instance.</returns>
        /// <exception cref="ArgumentException">If the method can't be found, or it has an
        /// unexpected return type (the return type must match exactly).</exception>
        /// <see href="https://codeblog.jonskeet.uk/2008/08/09/making-reflection-fly-and-exploring-delegates/"/>
        private static Func<TThis, TArg0, TReturn> BuildFastOpenMemberDelegate<TThis, TArg0, TReturn>(string methodName)
        {
            MethodInfo method = typeof(TThis).GetMethod(methodName, _BINDING_FLAGS, null, new Type[] { typeof(TArg0) }, null);
            
            return (Func<TThis, TArg0, TReturn>)method.CreateDelegate(typeof(Func<TThis, TArg0, TReturn>));
        }

        /// <summary>[FOR DEBUGGING ONLY] Calls an internal method on <see cref="Animator"/> that
        /// returns the name of the current state for a layer. The internal method could be removed
        /// or refactored at any time, and may not have good performance.</summary>
        /// <param name="animator">The animator to get the current state from.</param>
        /// <param name="layer">The layer to get the current state from.</param>
        /// <returns>The name of the currently running state.</returns>
        public static string GetCurrentStateName(this Animator animator, int layer)
        {
            if (_getCurrentStateName == null)
            {
                _getCurrentStateName = BuildFastOpenMemberDelegate<Animator, int, string>("GetCurrentStateName");
            }

            return _getCurrentStateName(animator, layer);
        }


        /// <summary>[FOR DEBUGGING ONLY] Calls an internal method on <see cref="Animator"/> that
        /// returns the name of the next state for a layer. The internal method could be removed or
        /// refactored at any time, and may not have good performance.</summary>
        /// <param name="animator">The animator to get the next state from.</param>
        /// <param name="layer">The layer to get the next state from.</param>
        /// <returns>The name of the next running state.</returns>
        public static string GetNextStateName(this Animator animator, int layer)
        {
            if (_getNextStateName == null)
            {
                _getNextStateName = BuildFastOpenMemberDelegate<Animator, int, string>("GetNextStateName");
            }

            return _getNextStateName(animator, layer);
        }

        
        /// <summary>[FOR DEBUGGING ONLY] Calls an internal method on <see cref="Animator"/> that
        /// returns the string used to create a hash from
        /// <see cref="Animator.StringToHash(string)"/>. The internal method could be removed or
        /// refactored at any time, and may not have good performance.</summary>
        /// <param name="animator">The animator to get the string from.</param>
        /// <param name="hash">The hash to get the original string for.</param>
        /// <returns>The name of the string for <paramref name="hash"/>.</returns>
        public static string ResolveHash(this Animator animator, int hash)
        {
            if (_resolveHash == null)
            {
                _resolveHash = BuildFastOpenMemberDelegate<Animator, int, string>("ResolveHash");
            }

            return _resolveHash(animator, hash);
        }

        #endregion
    }
}