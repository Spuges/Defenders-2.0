using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Object = UnityEngine.Object; // Defining so system Object doesn't conflict.

namespace Defender
{
    public static class Extensions
    {
        public static List<T> InstantiateList<T>(this List<T> list) where T : Object
        {
            List<T> copies = new List<T>();

            foreach(T obj in list)
            {
                copies.Add(Object.Instantiate(obj));
            }

            return copies;
        }

        public static T GetRandom<T>(this IEnumerable<T> list)
        {
            return list.ElementAt(Random.Range(0, list.Count()));
        }
    }
}
