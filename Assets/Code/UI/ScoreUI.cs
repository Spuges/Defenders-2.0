using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Mathematics;
using UnityEngine.UI;

namespace Defender
{
    public class ScoreUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI score_txt;
        [SerializeField] Image level_fill_img;

        [SerializeField] PID scoring_pid = new PID(0.1f, 0f, 0f);
        [SerializeField] PID fill_pid = new PID(0.1f, 0f, 0f);

        private float m_current = 0;
        private int m_desired = 0;

        private float m_desired_fill = 0f;

        private void Awake()
        {
            GameManager.I.onScoreChanged.Subscribe(OnScoreChanged);
            GameManager.I.onLevelChanged.Subscribe(OnLevelChanged);
            GameManager.I.onNewGame += OnEnable;
        }

        private void OnEnable()
        {
            m_current = 0;
            m_desired = 0;

            score_txt.text = m_current.ToString();
        }

        private void Update()
        {
            if(0f < math.abs(m_desired - math.round(m_current)))
            {
                m_current += scoring_pid.Update(m_desired, m_current, Time.unscaledDeltaTime);
                score_txt.text = ((int)math.round(m_current)).ToString();
            }

            if(0f < math.abs(m_desired_fill - level_fill_img.fillAmount))
            {
                level_fill_img.fillAmount += fill_pid.Update(m_desired_fill, level_fill_img.fillAmount, Time.unscaledDeltaTime);
            }
        }

        private void OnScoreChanged(int score)
        {
            if(!GameManager.I.GameOver())
            {
                m_desired_fill = GameManager.I.LevelProgress();
                m_desired = score;
            }
        }

        private void OnLevelChanged(int level)
        {
            if (!GameManager.I.GameOver())
            {
                m_desired_fill = 0f;
                level_fill_img.fillAmount = 0f;
            }
        }
    }
}
