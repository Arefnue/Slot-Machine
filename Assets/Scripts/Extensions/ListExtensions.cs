using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace Extensions
{
    public static class ListExtensions
    {
        public static T RandomItem<T>(this List<T> list)
        {
            if (list.Count == 0)
                throw new IndexOutOfRangeException("List is Empty");

            var randomIndex = Random.Range(0, list.Count);
            return list[randomIndex];
        }
        
        
        /// <summary>
        ///     Shuffles the List using Fisher Yates algorithm: https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle.
        /// </summary>
        public static void Shuffle<T>(this List<T> list)
        {
            var n = list.Count;
            for (var i = 0; i <= n - 2; i++)
            {
                //random index
                var rdn = Random.Range(0, n - i);

                //swap positions
                var curVal = list[i];
                list[i] = list[i + rdn];
                list[i + rdn] = curVal;
            }
        }
    }
}