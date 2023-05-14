using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Defender
{
    [RequireComponent(typeof(Animator))]
    public class HealthbarUI : MonoBehaviour
    {
        [Header("Normalised Value")]
        [SerializeField] string health_param_name = "health";
        [SerializeField] PID animator_pid = new PID(.1f, 0f, 0f);
        
        private Animator animator;
        private int health_hash;

        private float anim_desired;

        private bool first_spawn = true;

        private void Awake()
        {
            health_hash = Animator.StringToHash(health_param_name);
            animator = GetComponent<Animator>();

            //GameManager.I.onPlayerSpawned.Subscribe(OnPlayerSpawned);
        }

        private void Update()
        {
            if (!Player.I)
                return;

            anim_desired += animator_pid.Update(Player.I.hp.NormalizedHealth(), anim_desired, Time.unscaledDeltaTime);
            animator.SetFloat(health_hash, anim_desired);
        }

        //void OnPlayerSpawned(Player player)
        //{
        //    if (first_spawn)
        //    {
        //        player.hp.Subscribe(OnPlayerDamage);
        //        first_spawn = false;
        //    }

        //    //anim_desired = player.hp.NormalizedHealth();
        //    //anim_actual = anim_desired;
        //    //animator.SetFloat(health_hash, anim_actual);
        //}

        //void OnPlayerDamage(Health.Changed health)
        //{
        //    //anim_desired = health.taker.NormalizedHealth();
        //}
    }
}
