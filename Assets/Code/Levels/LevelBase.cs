using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Mathematics;
using Random = UnityEngine.Random;

namespace Defender
{
    // Making this modular for future
    [CreateAssetMenu(fileName = "New Level", menuName = "Game/Level")]
    public class LevelBase : ScriptableObject
    {
        [SerializeField, Min(0.1f)] private float difficulty_level = 1f;
        //[SerializeField] private List<WinCondition> Conditions = new List<WinCondition>();
        [SerializeField] private List<Spawner> spawners = new List<Spawner>();
        [SerializeField] private int score_requirement = 200;

        private float current_score;

        public float Progress() => math.clamp(current_score / score_requirement, 0f, 1f);

        public LevelBase Initialise(float difficulty_modifier = 1f)
        {
            LevelBase instance = Instantiate(this);
            instance.current_score = 0;

            float frac = (difficulty_modifier - 1f) / 10f;
            instance.difficulty_level = math.max(difficulty_modifier + frac, 1f);

            // If we're at last level.. Which should be a bonus level, scale that attribute and everything should scale progressively. Hoping
            instance.score_requirement = (int)(instance.score_requirement * instance.difficulty_level);

            Debug.Assert(1f <= instance.difficulty_level);
            foreach (var spawn in instance.spawners)
                spawn.Initialise(instance);

            //instance.Conditions = Conditions.InstantiateList();

            return instance;
        }

        public float GetDifficulty() => difficulty_level;

        public bool HasWon() => score_requirement <= current_score;

        // Doubt I'll have time to make unique levels :P
        // public bool HasWon() => Conditions.TrueForAll(o => o.CheckWon());
        
        public bool CanIncreaseLevel() => spawners.TrueForAll(s => s.GetAliveCount() == 0);

        public void Update()
        {
            if(!HasWon())
            {
                foreach(Spawner spawn in spawners)
                {
                    if (spawn.CanSpawn())
                        spawn.Spawn();
                }
            }
        }

        [Serializable]
        public class Spawner
        {
            [SerializeField] int max_unit_count;
            [SerializeField] int spawn_after_kills;
            [SerializeField] float2 spawn_interval;
            [SerializeField] int score_on_kill = 10;
            float next_spawn;

            [SerializeField] AIBase enemy_unity;
            private List<ISpaceCraft> alive_units;
            private LevelBase level;

            public int GetAliveCount() => alive_units.Count;

            public bool CanSpawn()
            {
                return  next_spawn <= Time.timeSinceLevelLoad &&
                        alive_units.Count < max_unit_count;
            }

            public void Spawn()
            {
                AIBase enemy;
                
                if(enemy_unity.Copy(out enemy))
                {
                    alive_units.Add(enemy);

                    float rng_dir = math.sign(Random.Range(-1f, 1f));

                    float rng_x = rng_dir * Random.Range(WorldGen.offset.x * .2f, WorldGen.offset.x * 1f);
                    float rng_y = Random.Range(GameRules.I.BoundsY.x, GameRules.I.BoundsY.y);

                    if (Player.I)
                        rng_x = Player.I.transform.position.x + rng_x;
                    else
                        rng_x = Camera.main.transform.position.x + rng_x;

                    enemy.transform.position = new float3(rng_x, rng_y, 0f);

                    Debug.DrawRay(enemy.transform.position, Vector3.up, Color.red, 1f);
                    //Debug.Break();

                    next_spawn = Time.timeSinceLevelLoad + Random.Range(spawn_interval.x, spawn_interval.y);

                    GameManager.Spawn(new ISpaceCraft.Spawn { space_craft = enemy });
                }
                else
                {
                    Debug.LogError($"Failed to spawn enemy...");
                }
            }
            
            public void Initialise(LevelBase level)
            {
                this.level = level;
                alive_units = new ();
                GameManager.I.onDeathEvent.Subscribe(OnDeath);
            }

            void OnDeath(ISpaceCraft.Death death)
            {
                if(alive_units.Remove(death.space_craft) && Player.I)
                {
                    if(death.source.owner == Player.I.gameObject)
                    {
                        level.current_score += score_on_kill * level.difficulty_level;
                        GameManager.I.AddScore((int)math.round(score_on_kill * level.difficulty_level));
                        //Debug.Log("Removed enemy from my alive list");
                    }
                }
            }
        }
    }
}
