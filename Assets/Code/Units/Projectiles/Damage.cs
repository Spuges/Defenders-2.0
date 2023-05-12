using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Defender
{
    public class Damage : MonoBehaviour
    {
        [SerializeField] float amount;
        [SerializeField] GameObject spawn_on_death;

        private void OnCollisionEnter(Collision collision)
        {
            // Apply damage if applicable
            IHealth health = collision.gameObject.GetComponent<IHealth>();
            if (health != null)
            {
                health.Damage(this, amount);
            }

            if(spawn_on_death.Copy(out GameObject copy))
            {
                copy.transform.position = collision.contacts[0].point;
                Debug.Log($"Spawn on death spawned something: {gameObject.name}");
            }
            else
            {
                Debug.Log($"Spawn on death was null for damage component: {gameObject.name}");
            }

            gameObject.SetActive(false);
        }
    }
}
