using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Defender
{
    public class GeneratingWorldUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI progress_text;

        private WorldGen m_gen;
        private Animator m_animator;

        private void Awake()
        {
            m_gen = WorldGen.I;
            m_animator = GetComponent<Animator>();

            m_animator.enabled = false;
            m_gen.Subscribe(OnWorldGenStateChanged);
        }

        private void Update()
        {
            if(m_animator.enabled && !m_gen.generating && 1f <= m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime)
            {
                gameObject.SetActive(false);
            }
        }

        private void OnDisable()
        {
            WorldGen.I.Unsubscribe(OnWorldGenStateChanged);
        }

        void OnWorldGenStateChanged(WorldGen.Progress state)
        {
            progress_text.text = $"{(state.progress * 100).ToString("0.0")}%";

            if(!state.generating)
            {
                m_animator.enabled = true;
            }
        }
    }
}
