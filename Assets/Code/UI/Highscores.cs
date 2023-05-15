using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using UnityEngine.UI;

namespace Defender
{
    [Serializable]
    public struct Score : IEquatable<Score>, IComparable<Score>
    {
        public string name;
        public int score;

        public int CompareTo(Score other)
        {
            int c = -score.CompareTo(other.score);

            if (c == 0)
                c = name.CompareTo(other.name);

            return c;
        }

        public bool Equals(Score other)
        {
            return score.Equals(other.score) && name.Equals(other.name);
        }
    }

    [RequireComponent(typeof(MenuFade))]
    public class Highscores : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            // Make sure the object is always off at start.
            I = FindObjectOfType<Highscores>(true);
            I.gameObject.SetActive(false);
        }

        public static Highscores I { get; private set; }

        public const string FILE_NAME = "highscores.json";
        public const int MAX_COUNT = 10;
        public static string PATH => Application.persistentDataPath + Path.AltDirectorySeparatorChar + FILE_NAME;

        private static List<Score> scores = new List<Score>()
        {
            new Score() { name = "Ana J", score = 4000 },
            new Score() { name = "Mighty Bob", score = 3340 },
            new Score() { name = "Elvis", score = 2950 },
            new Score() { name = "Kruger", score = 2500 },
            new Score() { name = "Mighty Bob", score = 1950 },
            new Score() { name = "Santa", score = 1600 },
            new Score() { name = "Freddy", score = 1560 },
            new Score() { name = "Shakur", score = 1200 },
            new Score() { name = "Henrik", score = 950 },
            new Score() { name = "Just Bob", score = 40 },
        };

        [SerializeField] private Button backBtn;
        [SerializeField] private HighscoreUI highscore_prefab;
        [SerializeField] private Transform highscore_parent;

        private List<HighscoreUI> ui_scores;
        public MenuFade Fade { get; private set; }

        public static bool IsHighScore(int score)
        {
            if(scores.Count < MAX_COUNT)
            {
                return true;
            }

            return scores.FindAll(hs => score < hs.score).Count() < MAX_COUNT;
        }

        public static void AddScore(Score score)
        {
            for(int i = 0; i < scores.Count; i++)
            {
                if (scores[i].score < score.score)
                {
                    scores.Insert(i, score);
                    break;
                }
            }

            if(MAX_COUNT < scores.Count)
            {
                scores = scores.GetRange(0, MAX_COUNT);
                scores.TrimExcess();
            }

            WriteHighscores();
        }

        public static void WriteHighscores()
        {
            string json = JsonConvert.SerializeObject(scores);
            //Debug.Log(json);
            File.WriteAllText(PATH, json);
            Debug.Log($"Saved highscores");
        }

        public static bool LoadHighscores(out List<Score> scores)
        {
            if (File.Exists(PATH))
            {
                scores = JsonConvert.DeserializeObject<List<Score>>(File.ReadAllText(PATH));
                Debug.Log("Loading highscores.");
                return true;
            }

            Debug.Log($"highscores do not exist");

            scores = default;
            return false;
        }

        private void Awake()
        {
            Fade = GetComponent<MenuFade>();

            // Load json
            if (LoadHighscores(out List<Score> highscores))
            {
                scores = highscores;
            }

            backBtn.onClick.AddListener(() =>
            {
                Menu.I.gameObject.SetActive(true);
                Fade.FadeOut();
            });

            ui_scores = new List<HighscoreUI>(MAX_COUNT);

            // Create UI Elements
            for(int i = 0; i < MAX_COUNT; i++)
            {
                HighscoreUI hs_ui = Instantiate(highscore_prefab);
                hs_ui.transform.SetParent(highscore_parent, false);
                ui_scores.Add(hs_ui);
            }
        }

        private void OnEnable()
        {
            scores.Sort();

            for(int i = 0; i < MAX_COUNT; i++)
            {
                // Offset because we're not altering the sorting order, we could do it in the IComparable, but I opted not to.
                ui_scores[i].name_txt.text = $"#{i + 1} {scores[i].name}";
                ui_scores[i].score_txt.text = $"{scores[i].score}";
            }
        }
    }
}
