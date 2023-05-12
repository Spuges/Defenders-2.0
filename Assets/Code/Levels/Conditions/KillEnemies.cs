using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Defender
{
    [CreateAssetMenu(fileName = "New Kill Condition", menuName = "Game/Level/Conditions/Kill Enemies")]
    public class KillEnemies : WinCondition
    {
        [SerializeField] private int kill_count = 15;
        private int current_count = 0;

        public override void Initialise()
        {
            current_count = 0;

            // Register to AIManager observable
        }

        public override bool CheckWon()
        {
            return kill_count <= current_count;
        }
    }
}
