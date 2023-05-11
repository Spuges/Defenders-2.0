using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Defender
{
    public class Lifetime : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod()]
        static void initialise_hash()
        {
            life_hash = Animator.StringToHash("life");
        }

        private static int life_hash;

        [Header("Normalized Param = \"life\"")]
        private Animator m_animator;
        
        private float death_time;
        private float start_time;
        private float duration;

        public void Setup(float life)
        {
            duration = life;
            start_time = Time.timeSinceLevelLoad;
            death_time = start_time + duration;

            StartCoroutine(LifeRoutine());
        }

        IEnumerator LifeRoutine()
        {
            if(m_animator)
            { 
                while (Time.timeSinceLevelLoad < death_time)
                {
                    float normalized_time = (1 - (death_time - Time.timeSinceLevelLoad)) / duration;
                    m_animator.SetFloat(life_hash, normalized_time);
                    yield return null;
                }
            }
            else
            {
                yield return new WaitForSeconds(duration);
            }

            gameObject.SetActive(false);
        }
    }
}
