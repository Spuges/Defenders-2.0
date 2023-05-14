using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Defender
{
    [RequireComponent(typeof(Animator))]
    public class MenuFade : MonoBehaviour
    {
        [SerializeField] private float duration = 0f;

        private Animator animator;
        private int time_hash = Animator.StringToHash("time");
        private float animation_time = 0f;

        private void Awake()
        {
             animator = GetComponent<Animator>();
        }

        public void FadeOut()
        {
            StartCoroutine(FadeRoutine(false));
        }

        private void OnEnable()
        {
            StartCoroutine(FadeRoutine(true));
        }

        private void Update()
        {
            animator.SetFloat(time_hash, animation_time);
        }

        private IEnumerator FadeRoutine(bool fade_in)
        {
            animation_time = animator.GetFloat(time_hash);
            float t = duration * animation_time;

            while (t <= duration)
            {
                t += Time.unscaledDeltaTime;

                if (fade_in)
                    animation_time = Mathf.Lerp(0, 1f, t / duration);
                else
                    animation_time = Mathf.Lerp(1f, 0f, t / duration);

                yield return null;
            }

            if (!fade_in)
                gameObject.SetActive(false);
        }
    }
}
