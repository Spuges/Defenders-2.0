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
    public class WorldGen : MonoBehaviour, IObservable<WorldGen.Progress>, IObservable<WorldGen.PropData>
    {
        public static WorldGen I { get; private set; }

        public static int WrapDistance() => I.wrap_distance;
        public static float2 offset { get; private set; }

        [SerializeField] private int wrap_distance = 512;
        [SerializeField] private int building_layers = 8; // Depth
        [SerializeField] private float space_between_depth = 2;
        [SerializeField,Header("Z pullback, normalised")] private float pullback = 0.5f;

        [Header("World wrapper settings")]
        [SerializeField, Range(8, 128)] byte chunks = 12;

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

        #region IObservable
        private event Action<Progress> onDataChanged;

        public struct Progress
        {
            public float progress;
            public bool generating;
        }
        
        private event Action<PropData> onPropCreated;
        public struct PropData
        {
            public GameObject obj;
        }
        #endregion

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
            WorldWrapper.I.Initialise(wrap_distance, (byte)chunks);
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
            Progress data = new Progress()
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

            onPropCreated?.Invoke(new PropData()
            {
                obj = building
            });

            return index + 4;
        }

        // UI, build progress.
        public void Subscribe(Action<Progress> callback)
        {
            onDataChanged += callback;
        }

        public void Unsubscribe(Action<Progress> callback)
        {
            onDataChanged -= callback;
        }

        /// <summary>
        /// Sub to Prop generated
        /// </summary>
        /// <param name="callback"></param>
        public void Subscribe(Action<PropData> callback)
        {
            onPropCreated += callback;
        }

        /// <summary>
        /// Unsub from Prop generated
        /// </summary>
        /// <param name="callback"></param>
        public void Unsubscribe(Action<PropData> callback)
        {
            onPropCreated -= callback;
        }
    }
}
