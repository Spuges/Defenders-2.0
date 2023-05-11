using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;
using Unity.Mathematics;
using Random = UnityEngine.Random; // Will switch to mathematics once I get my internet back

namespace Defender
{
    [DefaultExecutionOrder(-200)]
    public class WorldGen : MonoBehaviour, IObservable<WorldGen.Data>
    {
        public static WorldGen I { get; private set; }

        public int GetWrapDistance => wrap_distance;

        [SerializeField] private int wrap_distance = 512;
        private float2 offset;
        [SerializeField] private int building_layers = 8; // Depth
        [SerializeField] private float space_between_depth = 2;
        [SerializeField,Header("Z pullback, normalised")] private float pullback = 0.5f;

        // Make it into a scriptableobject, also add script that contains the dimensions
        [SerializeField] private List<GameObject> buildings = new List<GameObject>();

        public bool generating
        {
            get => m_generating;
            private set
            {
                if(m_generating != value)
                {
                    m_generating = value;
                    update_state();
                }
            }
        }

        private bool m_generating;

        public float normalized_progress 
        {
            get => m_normalized;
            private set
            {
                if (!Mathf.Approximately(m_normalized, value))
                {
                    m_normalized = value;
                    update_state();
                }
            }
        }
        private float m_normalized;

        private event Action<Data> onDataChanged;

        public struct Data
        {
            public float progress;
            public bool generating;
        }

        public void Subscribe(Action<Data> callback)
        {
            onDataChanged += callback;
        }

        public void Unsubscribe(Action<Data> callback)
        {
            onDataChanged -= callback;
        }

        private void Awake()
        {
            I = this;
            // Random seed
            Random.InitState(Guid.NewGuid().GetHashCode());

            generating = true;
            normalized_progress = 0f;

            // Some variable initialisation
            offset = new()
            {
                x = wrap_distance / 2f,
                y = building_layers * space_between_depth * -pullback,
            };
        }

        void Start()
        {
            StartCoroutine(generate_world());
        }

        IEnumerator generate_world()
        {
            UnityEngine.Debug.Log($"Start generating...");
            yield return new WaitForEndOfFrame();

            Stopwatch m_timer = new Stopwatch();

            long total_time = 0;
            long total_build_time = 0;

            m_timer.Start();

            int max = building_layers * wrap_distance;
            int index = 0;

            while (generating)
            {
                // Create prop
                //UnityEngine.Debug.Log($"prop? {m_timer.ElapsedMilliseconds}");
                index = create_prop(index);

                if(max <= index)
                    generating = false;


                if (m_timer.ElapsedMilliseconds > 33)
                {
                    normalized_progress = Mathf.Min(1f, index / (float)max);
                    total_build_time += m_timer.ElapsedMilliseconds;
                    yield return null;
                    total_time += m_timer.ElapsedMilliseconds;
                    m_timer.Restart();
                }
            }

            normalized_progress = Mathf.Min(1f, index / (float)max);
            m_timer.Stop();
            total_time += m_timer.ElapsedMilliseconds;
            UnityEngine.Debug.Log($"Time spent: {total_time}ms - Time spent building: {total_build_time}ms - Progress: {normalized_progress}");
        }

        private void update_state()
        {
            Data data = new Data()
            {
                progress = normalized_progress,
                generating = generating,
            };
            onDataChanged?.Invoke(data);
        }

        private int create_prop(int index)
        {
            int x = index % wrap_distance;
            int z = (index - x) / wrap_distance;
            int z_variance = (z % 2) * 2;
            Vector3 position = new Vector3(x - offset.x + z_variance, 0, z * space_between_depth + offset.y);



            int building_index = Random.Range(0, buildings.Count);

            GameObject building = Instantiate(buildings[building_index]);

            building.transform.SetParent(transform);
            building.transform.position = position;

            return index + 4;
        }
    }
}
