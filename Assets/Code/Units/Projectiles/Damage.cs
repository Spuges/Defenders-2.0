using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Defender
{
    public class Damage : MonoBehaviour
    {
        public float amount;
        [SerializeField] List<GameObject> spawn_on_damage;

        private bool is_projectile = true;

        private void Awake()
        {
            is_projectile = GetComponent<IHealth>() == null;
        }

        private void OnCollisionEnter(Collision collision)
        {
            // Apply damage if applicable
            IHealth health = collision.gameObject.GetComponent<IHealth>();

            if (health != null)
            {
                health.Damage(this, amount);
            }

            foreach(GameObject spawn in spawn_on_damage)
            {
                if (spawn.Copy(out GameObject copy))
                {
                    copy.transform.position = collision.contacts[0].point;
                    //Debug.Log($"Spawn on death spawned something: {gameObject.name}");
                }
            }

            if (!is_projectile)
                return;

            gameObject.SetActive(false);
        }
    }
}
