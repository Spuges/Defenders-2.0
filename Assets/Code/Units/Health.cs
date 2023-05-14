using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Defender
{

    public class Health : MonoBehaviour, IHealth, IObservable<Health.Changed>
    {
        public struct Changed
        {
            public Damage source;
            public Health taker;
        }

        [SerializeField] private float max_health;
        private float current_health;

        private event Action<Changed> onChanged;

        public float health => current_health;

        [SerializeField] private List<GameObject> spawn_on_death = new List<GameObject>();

        public float NormalizedHealth() => current_health / max_health;

        public void Restore()
        {
            current_health = max_health;
            onChanged?.Invoke(new Changed() { source = null, taker = this });
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
                onChanged?.Invoke(new Changed() { source = null, taker = this });
            }
            else
            {
                Restore();
            }

            GameManager.I.onLevelChanged.Subscribe((level) => Restore());
        }

        public void Damage(Damage source, float damage)
        {
            current_health -= damage;
            onChanged?.Invoke(new Changed() { source = source, taker = this });

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

        public void Subscribe(Action<Changed> callback)
        {
            onChanged += callback;
        }

        public void Unsubscribe(Action<Changed> callback)
        {
            onChanged += callback;
        }
    }
}
