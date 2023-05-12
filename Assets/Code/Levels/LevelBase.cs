using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Mathematics;

namespace Defender
{
    // Making this modular for future
    [CreateAssetMenu(fileName = "New Level", menuName = "Game/Level")]
    public class LevelBase : ScriptableObject
    {
        [SerializeField, Min(0.1f)] private float difficulty_level = 1f;
        [SerializeField] private List<WinCondition> Conditions = new List<WinCondition>();
        [SerializeField] private List<Spawner> spawners = new List<Spawner>();

        public LevelBase Initialise()
        {
            LevelBase instance = Instantiate(this);

            Conditions = Conditions.InstantiateList();

            return instance;
        }

        public bool HasWon() => Conditions.TrueForAll(o => o.CheckWon());

        [Serializable]
        public struct Spawner
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
                    next_spawn = Time.timeSinceLevelLoad + UnityEngine.Random.Range(spawn_interval.x, spawn_interval.y);
                }
                else
                {
                    Debug.LogError($"Failed to spawn enemy...");
                }
            }
            
            public void Initialise()
            {
                alive_units = new ();

                if (spawn_after_kills > 0)
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
