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
        [SerializeField] private List<WinCondition> Conditions = new List<WinCondition>();
        [SerializeField] private List<Spawner> spawners = new List<Spawner>();

        public LevelBase Initialise(float difficulty_modifier = 1f)
        {
            LevelBase instance = Instantiate(this);

            // If we're at last level.. Which should be a bonus level, scale that attribute and everything should scale progressively. Hoping
            difficulty_level *= difficulty_modifier;

            foreach (var spawn in instance.spawners)
                spawn.Initialise();

            instance.Conditions = Conditions.InstantiateList();

            return instance;
        }

        public bool HasWon() => Conditions.TrueForAll(o => o.CheckWon());

        public void Update()
        {
            foreach(Spawner spawn in spawners)
            {
                if (spawn.CanSpawn())
                    spawn.Spawn();
            }
        }

        [Serializable]
        public class Spawner
        {
            [SerializeField] int max_unit_count;
            [SerializeField] int spawn_after_kills;
            [SerializeField] float2 spawn_interval;
            float next_spawn;

            [SerializeField] Spacecraft enemy_unity;
            private List<Spacecraft> alive_units;

            public bool CanSpawn()
            {
                return  next_spawn <= Time.timeSinceLevelLoad &&
                        alive_units.Count < max_unit_count;
            }

            public void Spawn()
            {
                Spacecraft enemy;
                
                if(enemy_unity.Copy(out enemy))
                {
                    // Position?!?!
                    alive_units.Add(enemy);

                    float rng_x = Random.Range(-WorldGen.offset.x, WorldGen.offset.x);
                    float rng_y = Random.Range(GameRules.I.PlayerHeightBounds.x, GameRules.I.PlayerHeightBounds.y);

                    enemy.transform.position = new float3(rng_x, rng_y, 0f);

                    next_spawn = Time.timeSinceLevelLoad + Random.Range(spawn_interval.x, spawn_interval.y);

                    GameManager.Spawn(new Spacecraft.Spawn { space_craft = enemy });
                }
                else
                {
                    Debug.LogError($"Failed to spawn enemy...");
                }
            }
            
            public void Initialise()
            {
                alive_units = new ();
                GameManager.I.onDeathEvent.Subscribe(OnDeath);
            }

            void OnDeath(Spacecraft.Death death)
            {
                if(alive_units.Remove(death.space_craft))
                {
                    Debug.Log("Removed enemy from my alive list");
                }
            }
        }
    }
}
