using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Defender
{

    public class Health : MonoBehaviour, IHealth, IObservable<Damage>
    {
        [SerializeField] private float max_health;
        private float current_health;

        private event System.Action<Damage> on_damage;

        public float health => current_health;

        [SerializeField] private List<GameObject> spawn_on_death = new List<GameObject>();

        public void Restore()
        {
            current_health = max_health;
        }

        private void OnDisable()
        {
            GameManager.I.onLevelChanged.Unsubscribe((level) => Restore());
        }

        private void OnEnable()
        {
            if(GetComponent<AIBase>())
            {
                current_health = max_health * GameManager.I.GetDifficulty();
            }
            else
            {
                current_health = max_health;
            }

            GameManager.I.onLevelChanged.Subscribe((level) => Restore());
        }

        public void Damage(Damage source, float damage)
        {
            current_health -= damage;
            on_damage?.Invoke(source);

            if (0 < current_health)
                return;

            foreach(var spawn in spawn_on_death)
            {
                if (spawn.Copy(out GameObject copy))
                {
                    copy.transform.position = transform.position;
                }
            }

            ISpaceCraft space_craft = GetComponent<ISpaceCraft>();

            if (space_craft != null)
            {
                GameManager.Death(new ISpaceCraft.Death() { space_craft = space_craft, source = source });
            }

            gameObject.SetActive(false);
        }

        public void Subscribe(System.Action<Damage> callback)
        {
            on_damage += callback;
        }

        public void Unsubscribe(System.Action<Damage> callback)
        {
            on_damage += callback;
        }
    }
}
