using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Defender
{
    [RequireComponent(typeof(Animator))]
    public class LevelUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI static_level_txt;
        [SerializeField] TextMeshProUGUI splash_level_txt;

        private Animator animator;

        private void Awake()
        {
            GameManager.I.onLevelChanged.Subscribe(OnLevelChanged);
            gameObject.SetActive(false);
            animator = GetComponent<Animator>();
        }

        void OnLevelChanged(int level)
        {
            string text = $"Level {level + 1}";

            static_level_txt.text = text;
            splash_level_txt.text = text;

            gameObject.SetActive(true);
        }

        private void Update()
        {
            if(1 <= animator.GetCurrentAnimatorStateInfo(0).normalizedTime)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
