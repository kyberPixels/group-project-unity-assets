using UnityEngine;

public class CharacterAttackController : MonoBehaviour
{
    [Header("Attack Settings")]
    [Tooltip("For Mano/Dragon: Drag the Particle System here.")]
    public ParticleSystem attackParticles;

    [Tooltip("For Archer/Assassin: Drag the Arrow/Dagger Prefab here.")]
    public GameObject projectilePrefab;

    [Tooltip("Where the arrow/dagger shoots out from")]
    public Transform spawnPoint;

    public float projectileSpeed = 15f;

    void Update()
    {
        // LOCAL TESTING: Press Spacebar on laptop to attack
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TriggerAttack();
        }
    }

    /// This is the master function. You can call this function 
    /// directly from the mobile input script when the app/phone button is pressed
    
    public void TriggerAttack()
    {
        // 1. Handle Particle Attacks (Mano & Dragon)
        if (attackParticles != null)
        {
            // Play the particle system burst
            attackParticles.Play();
            Debug.Log($"{gameObject.name} unleashed a magic attack!");
        }

        // 2. Handle Projectile Attacks (Archer & Assassin)
        if (projectilePrefab != null && spawnPoint != null)
        {
            // Spawn the arrow/dagger at the spawn point
            GameObject projectile = Instantiate(projectilePrefab, spawnPoint.position, spawnPoint.rotation);
            
            // Give it physics speed to fly forward
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = spawnPoint.forward * projectileSpeed;
            }

            Debug.Log($"{gameObject.name} shot a projectile!");
        }
    }
}