using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Defender
{
    [DefaultExecutionOrder(-100)]
    public class GameManager : MonoBehaviour
    {
        public static GameManager I { get; private set; }

        [SerializeField] List<LevelBase> levels = new List<LevelBase>();

        private int current_level;
        private LevelBase active_level;

        public Observable<int> onScoreChanged { get; private set; }

        public event Action onNewGame;
        public event Action onGameOver;

        public Observable<int> onLevelChanged = new();
        public Observable<ISpaceCraft.Death> onDeathEvent = new();
        public Observable<ISpaceCraft.Spawn> onSpawnEvent = new();
        public Observable<Player> onPlayerSpawned = new();

        public int Score { get; private set; }

        public static void Death(ISpaceCraft.Death death)
        {
            //Debug.Log($"<color=#FF7777>ship death {death.space_craft.GameObject.name}</color>");
            I.onDeathEvent?.Invoke(death);
        }

        public static void Spawn(ISpaceCraft.Spawn spawn)
        {
            //Debug.Log($"<color=#77FF77>ship spawn {spawn.space_craft.GameObject.name}</color>");
            I.onSpawnEvent?.Invoke(spawn);
        }

        private void Awake()
        {
            I = this;

            onScoreChanged = new Observable<int>();
            MenuMode();
        }

        public void MenuMode()
        {
            onDeathEvent.Clear();
            current_level = 0;
            StartLevel();
        }

        public void NewGame()
        {
            current_level = 0;
            onDeathEvent.Clear();
            ClearLevel();

            StartCoroutine(SpawnPlayer(true));

            StartLevel();
            onLevelChanged.Invoke(current_level);
        }

        private void Update()
        {
            if(active_level.HasWon() && active_level.CanIncreaseLevel())
            {
                NextLevel();
            }

            active_level.Update();
        }

        public void StartLevel()
        {
            active_level = levels[current_level].Initialise();
        }

        public void ClearLevel()
        {
            List<AIBase> ais = new List<AIBase>(FindObjectsOfType<AIBase>());

            foreach (var ai in ais)
            {
                ai.gameObject.SetActive(false);
            }
        }

        public float LevelProgress()
        {
            return active_level.Progress();
        }

        public float GetDifficulty()
        {
            return active_level.GetDifficulty();
        }

        public IEnumerator SpawnPlayer(bool is_newgame = false)
        {
            yield return new WaitForSeconds(1f);

            Player player = Player.I;

            if(player == null)
            {
                player = Instantiate(Resources.Load<Player>("player"));
            }

            player.transform.position = new Vector3(0f, UnityEngine.Random.Range(GameRules.I.BoundsY.x, GameRules.I.BoundsY.y), 0f);

            // Aiming to reset values on enable if possible. Reduces workload
            float range = UnityEngine.Random.Range(30f, 45f);
            float dir = math.sign(UnityEngine.Random.Range(-1f, 1f));

            player.transform.position += (Vector3.right * dir * range);
            player.gameObject.SetActive(true);
            
            if (is_newgame)
                player.RestoreLives();
            else
                player.DeductLife();

            onPlayerSpawned?.Invoke(player);
        }

        public void NextLevel()
        {
            current_level++;

            int difficulty = math.max(1 + current_level - levels.Count, 1);
            LevelBase newLevel = levels[math.min(current_level, levels.Count - 1)];
            active_level = newLevel.Initialise(difficulty);

            onLevelChanged?.Invoke(current_level);
        }

        public void AddScore(int gain)
        {
            //Debug.Log($"Adding score: {Score} Gain: {gain}");
            Score += gain;
            onScoreChanged?.Invoke(Score);
        }

        public void PlayerDeath()
        {
            if (Player.I.current_lives <= 0)
                onGameOver?.Invoke();
            else
            {
                StartCoroutine(SpawnPlayer());
            }
        }

        public bool GameOver()
        {
            return !Player.I || Player.I.hp.health <= 0 && Player.I.current_lives <= 0;
        }
    }
}
