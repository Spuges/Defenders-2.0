using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Defender
{
    public class Poolable : MonoBehaviour
    {
        // Like a tag. Right?
        public GameObject Prefab { get; set; }

        private void OnDisable()
        {
            // Will see if I have to start managing these again by hand >.<
            Pooling.AddToPool(this);
        }

        public void Setup(GameObject prefab)
        {
            Prefab = prefab;
        }
    }

    public static class Pooling
    {
        private static Dictionary<GameObject, Queue<Poolable>> object_pool;

        public static bool Copy(this GameObject original, out GameObject copy)
        {
            copy = default;

            if (original == null)
                return false;

            if(original.transform.Copy(out Transform t))
            {
                copy = t.gameObject;
                return true;
            }

            return false;
        }

        public static bool Copy<T>(this Component component, out T obj) where T : Component
        {
            obj = default;

            if (component == null)
                return false;

            if (component.gameObject.scene.isLoaded)
            {
                throw new System.Exception($"Trying to get pooled object from non-prefab reference. {component.gameObject.scene.name}");
            }

            if(!GetPoolable(component.gameObject, out obj))
            {
                obj = Object.Instantiate(component).GetComponent<T>();

                Poolable pool = obj.GetComponent<Poolable>();

                if (pool == null)
                    obj.gameObject.AddComponent<Poolable>().Setup(component.gameObject);
                else
                    pool.Setup(component.gameObject);
            }
            else
            {
                obj.gameObject.SetActive(true);
            }

            return true;
        }

        public static void AddToPool(this Poolable poolable)
        {
            if (object_pool == null)
                object_pool = new ();

            if (!object_pool.ContainsKey(poolable.Prefab))
            {
                object_pool.Add(poolable.Prefab, new());
            }

            poolable.gameObject.SetActive(false);
            object_pool[poolable.Prefab].Enqueue(poolable);
        }

        private static bool GetPoolable<T>(this GameObject prefab, out T obj) where T : Component
        {
            obj = null;

            if (object_pool == null)
            {
                object_pool = new Dictionary<GameObject, Queue<Poolable>>();
            }

            if(object_pool.TryGetValue(prefab, out var queue))
            {
                while(queue.Count > 0 && obj == null)
                { 
                    obj = queue.Dequeue().GetComponent<T>();

                    if(obj != null)
                        return true;
                }
            }

            return false;
        }
    }
}
