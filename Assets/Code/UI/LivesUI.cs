using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Defender
{
    public class LivesUI : MonoBehaviour
    {
        [SerializeField] List<Image> lives;
        [SerializeField] private Color missing_color;

        private Color default_color;

        private void Awake()
        {
            default_color = lives[0].color;

            GameManager.I.onPlayerSpawned.Subscribe(SetLives);
        }

        public void SetLives(Player player)
        {
            for(int life = 0; life < lives.Count; life++)
            {
                if(life < player.current_lives)
                {
                    lives[life].color = default_color;
                }
                else
                {
                    lives[life].color = missing_color;
                }
            }
        }
    }
}
