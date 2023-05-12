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
        [SerializeField] Damage projectile = default;
        [SerializeField] float3 spawn_offset;
        [SerializeField] float life_time = 10f; // Die and go to pool.
        [SerializeField] bool inherit_velocity = true;

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

            GameObject new_projectile = default;

            if (projectile.Copy(out Damage dmg))
            {
                dmg.amount = damage; // Override the old damage
                new_projectile = dmg.gameObject;
            }
            else
            {
                Debug.LogError($"Ro-ou, no projectile prefab");
            }

            // Initial position
            new_projectile.transform.position = (float3)m_transform.position + offset;

            // Add projectile start velocity
            float3 dir = math.normalize(new float3(m_transform.forward.x, 0, 0));
            offset *= dir;
            float3 projectile_velocity = dir * start_velocity;

            Rigidbody rigid = new_projectile.GetComponent<Rigidbody>();
            rigid.velocity = projectile_velocity;

            // Add inherited velocity
            if (m_velocity != null && inherit_velocity)
            {
                rigid.velocity += new Vector3(m_velocity.velocity.x, 0, 0);
            }

            // Ignore origin - collision
            if (m_collider != null)
                Physics.IgnoreCollision(m_collider, new_projectile.GetComponent<Collider>());

            // TODO
            // Get lifetime component of projectile and set the duration
            Lifetime life = new_projectile.GetComponent<Lifetime>() ?? new_projectile.AddComponent<Lifetime>();
            life.Setup(life_time);

            // Get the damage component of projectile and set it up
        }
    }
}