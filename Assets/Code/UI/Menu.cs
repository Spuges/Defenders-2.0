using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Defender
{
    [RequireComponent(typeof(MenuFade))]
    public class Menu : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            // Make sure the object is always on at start.
            I = FindObjectOfType<Menu>(true);
            I.gameObject.SetActive(true);
        }

        public static Menu I { get; private set; }

        [SerializeField] Button newGameBtn;
        [SerializeField] Button highscoresBtn;
        [SerializeField] Button exitBtn;

        public MenuFade Fade { get; private set; }

        private void Awake()
        {
            Fade = GetComponent<MenuFade>();

            GameManager.I.onGameOver += OnGameOver;

            Inputs.I.onEscape.Subscribe(() =>
            {
                if (!GameManager.I.GameOver())
                {
                    if (!gameObject.activeInHierarchy)
                    {
                        gameObject.SetActive(true);
                    }
                    else
                    {
                        Fade.FadeOut();
                        Time.timeScale = 1f;
                    }
                }
            });

            newGameBtn.onClick.AddListener(() =>
            {
                Fade.FadeOut();
                GameManager.I.NewGame();
            });

            highscoresBtn.onClick.AddListener(() =>
            {
                Highscores.I.gameObject.SetActive(true);
                Fade.FadeOut();
            });

            exitBtn.onClick.AddListener(() =>
            {
                Application.Quit();
            });

        }

        void OnGameOver()
        {
            if(Highscores.IsHighScore(GameManager.I.Score))
            {
                HighscorePrompt.I.gameObject.SetActive(true);
                HighscorePrompt.I.NewPrompt(GameManager.I.Score);
            }
            else
            {
                gameObject.SetActive(true);
            }
        }

        private void OnEnable()
        {
            if (!GameManager.I.GameOver())
            {
                Time.timeScale = 0f;
            }
        }
    }
}
