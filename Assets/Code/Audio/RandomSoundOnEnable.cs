using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Defender
{
    public class RandomSoundOnEnable : MonoBehaviour
    {
        [SerializeField] private bool manage_pooling = true;
        private List<AudioSource> m_audios;

        private AudioSource active;
        
        private void Awake()
        {
            m_audios = new List<AudioSource>(GetComponentsInChildren<AudioSource>());

            m_audios.ForEach(a => a.playOnAwake = false);
        }

        private void OnEnable()
        {
            active = m_audios.GetRandom();
            active.Play();

            if(manage_pooling)
                StartCoroutine(DisableAfterPlaying());
        }

        private IEnumerator DisableAfterPlaying()
        {
            while (active.isPlaying)
                yield return null;
            
            gameObject.SetActive(false);
        }
    }
}