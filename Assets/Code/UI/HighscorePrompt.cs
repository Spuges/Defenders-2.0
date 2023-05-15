using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;

namespace Defender
{
    [RequireComponent(typeof(MenuFade))]
    public class HighscorePrompt : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            // Make sure the object is always off at start.
            I = FindObjectOfType<HighscorePrompt>(true);
            I.gameObject.SetActive(false);
        }

        public static HighscorePrompt I { get; private set; }

        public MenuFade Fade { get; private set; }

        public Score LastScore { get; private set; }

        [SerializeField] TextMeshProUGUI score_txt;
        [SerializeField] TMP_InputField input_field;
        [SerializeField] Button ok_btn;

        private void Awake()
        {
            // In hindsight this fetching should have been added in to a base class
            Fade = GetComponent<MenuFade>();

            input_field.onValueChanged.AddListener(OnInputChanged);

            ok_btn.onClick.AddListener(() =>
            {
                LastScore = new Score() { name = input_field.text, score = LastScore.score };
                Highscores.AddScore(LastScore);
                Highscores.I.gameObject.SetActive(true);
                Fade.FadeOut();
            });
        }

        public void NewPrompt(int score)
        {
            LastScore = new Score() { name = "", score = score };
            score_txt.text = score.ToString();
            ok_btn.interactable = false;
            input_field.text = "";
        }

        private void OnInputChanged(string input)
        {
            ok_btn.interactable = 3 < input.Length;
        }
    }
}
