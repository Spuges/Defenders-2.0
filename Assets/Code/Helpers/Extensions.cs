using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using System.Linq;
using Object = UnityEngine.Object; // Defining so system Object doesn't conflict.
using Random = UnityEngine.Random;

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

        public static T Get<T>(this ISpaceCraft sc) where T : Component
        {
            return sc.GameObject.GetComponent<T>();
        }

        // Short hand conversion >.<
        public static float2 f2(this Vector3 v3) => ((float3)v3).xy;
        public static float3 f3(this Vector3 v3) => (float3)v3;
    }
}
