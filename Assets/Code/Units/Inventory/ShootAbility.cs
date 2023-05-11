using UnityEngine;
using Unity.Mathematics;

namespace Defender
{
    [CreateAssetMenu(fileName = "New ShootAbility", menuName = "Game/Abilities/Shoot Ability")]
    public class ShootAbility : BaseAbility, IActiveAbility
    {
        // Quality of life
        public Vector3 Offset => spawn_offset; // Draw a wire sphere on the spawn position of the player obect in the scene.

        [SerializeField] float start_velocity = 50f;
        [SerializeField] float cooldown = default;
        [SerializeField] float damage = default;
        [SerializeField] GameObject projectile = default;
        [SerializeField] float3 spawn_offset;
        [SerializeField] float life_time = 10f; // Die and go to pool.

        private Collider m_collider;
        private IVelocity m_velocity;
        private Transform m_transform;

        private float next_fire = 0f;

        bool IActiveAbility.CanActivate() => CanFire();
        void IActiveAbility.Activate() => Shoot();

        public float get_cooldown => cooldown;

        public float get_cooldown_remaining => next_fire - Time.timeSinceLevelLoad;


        /// <summary>
        /// Happens after the ability is copied
        /// </summary>
        protected override void Initialise()
        {
            m_transform = owner.GetComponent<Transform>();  // To get the rotation
            m_velocity = owner.GetComponent<IVelocity>();   // Add velocity for projectiles depending on the flight speed
            m_collider = owner.GetComponent<Collider>();    // Ignore collision
        }

        public bool CanFire()
        {
            return next_fire <= Time.timeSinceLevelLoad;
        }

        public void Shoot()
        {
            if (!CanFire())
                return;

            next_fire = Time.timeSinceLevelLoad + cooldown;

            float3 offset = spawn_offset;


            GameObject new_projectile;

            if (!projectile.Copy(out new_projectile))
                Debug.LogError($"Ro-ou, no projectile prefab");


            // Get the velocity and add it to the projectile
            if (m_velocity != null)
            {
                float3 dir = math.normalize(new float3(m_transform.forward.x, 0, 0));
                offset *= dir;
                float3 projectile_velocity = dir * start_velocity;
                projectile_velocity.x += m_velocity.velocity.x;

                Rigidbody rigid = new_projectile.GetComponent<Rigidbody>();
                rigid.velocity = projectile_velocity;
            }    

            if(m_collider != null) // Ignore origin
                Physics.IgnoreCollision(m_collider, new_projectile.GetComponent<Collider>());
            new_projectile.transform.position = (float3)m_transform.position + offset;

            // TODO
            // Get lifetime component of projectile and set the duration
            Lifetime life = new_projectile.GetComponent<Lifetime>() ?? new_projectile.AddComponent<Lifetime>();
            life.Setup(life_time);

            // Get the damage component of projectile and set it up
        }
    }
}