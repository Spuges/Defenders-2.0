using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Defender
{
    [RequireComponent(typeof(MenuFade))]
    public class Menu : MonoBehaviour
    {
        [SerializeField] Button startBtn;
        [SerializeField] Button highscoresBtn;
        [SerializeField] Button exitBtn;

        public MenuFade Fade { get; private set; }

        private void Awake()
        {
            Fade = GetComponent<MenuFade>();

            GameManager.I.onGameOver += () => gameObject.SetActive(true);
            Inputs.I.onEscape.Subscribe(() => gameObject.SetActive(true));

            startBtn.onClick.AddListener(() =>
            {
                Fade.FadeOut();
                GameManager.I.NewGame();
            });

        }

        private void OnEnable()
        {
            if(!GameManager.I.GameOver())
            {
                Time.timeScale = 0f;
            }
        }
    }
}
