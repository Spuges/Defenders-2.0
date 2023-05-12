using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Defender
{

    public class Health : MonoBehaviour, IHealth
    {
        [SerializeField] private float max_health;
        private float current_health;

        float IHealth.Health => current_health;

        [SerializeField] private GameObject spawn_on_death;

        private void OnEnable()
        {
            current_health = max_health;
        }

        public void Damage(Damage source, float damage)
        {
            current_health -= damage;

            if(current_health < 0)
            {
                if(spawn_on_death.Copy(out GameObject copy))
                {
                    copy.transform.position = transform.position;
                    gameObject.SetActive(false);
                }
            }
        }
    }
}
