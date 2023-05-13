using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Defender
{
    [DefaultExecutionOrder(-100)]
    public class GameManager : MonoBehaviour
    {
        public static GameManager I { get; private set; }

        [SerializeField] List<LevelBase> levels = new List<LevelBase>();

        public Observable<Spacecraft.Death> onDeathEvent = new();
        public Observable<Spacecraft.Spawn> onSpawnEvent = new();

        private int current_level;
        private LevelBase active_level;

        public static void Death(Spacecraft.Death death)
        {
            Debug.Log($"<color=#FF7777>ship death {death.space_craft.name}</color>");
            I.onDeathEvent.Invoke(death);
        }

        public static void Spawn(Spacecraft.Spawn spawn)
        {
            Debug.Log($"<color=#77FF77>ship spawn {spawn.space_craft.name}</color>");
            I.onSpawnEvent.Invoke(spawn);
        }

        private void Awake()
        {
            I = this;

            current_level = 0;
        }

        private void Start()
        {
            StartLevel();
        }

        private void Update()
        {
            active_level.Update();
        }

        public void StartLevel()
        {
            active_level = levels[current_level].Initialise();
        }

        public void NextLevel()
        {
            current_level++;
        }

        public void OnReset()
        {
            onDeathEvent = null;
        }
    }
}
