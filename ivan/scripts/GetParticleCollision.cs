using UnityEngine;

public class GetParticleCollision : MonoBehaviour
{
    [Header("Linked Network System")]
    [Tooltip("Drag the target character's HealthManager component here.")]
    public HealthManager healthManager;
    
    [Tooltip("Drag the GameEventListener object from the scene here to read incoming phone rolls.")]
    public GameEventListener networkListener;

    [Header("Combat Configuration")]
    [Tooltip("Set this to match the Tag of particles that can hurt this character (e.g., 'Fire' or 'Leaf').")]
    public string collisionTag = "Fire"; 
    
    [Tooltip("How many individual particle sparks equal ONE actual hit event.")]
    public int hitsToDamage = 25; 
    
    [Tooltip("The static baseline damage before the dice multiplier.")]
    public int baseDamage = 10;

    private int hitCount = 0;
    private int latestDiceRoll = 1;

    void OnEnable()
    {
        // Listen to the network so we always know what the latest phone roll calculation is
        if (SupabaseRealtimeClient.Instance != null)
        {
            SupabaseRealtimeClient.Instance.OnGameEvent += OnNetworkDataReceived;
        }
    }

    void OnDisable()
    {
        if (SupabaseRealtimeClient.Instance != null)
        {
            SupabaseRealtimeClient.Instance.OnGameEvent -= OnNetworkDataReceived;
        }
    }

    private void OnNetworkDataReceived(GameEvent e)
    {
        // If the incoming network packet contains a die roll result, save it for the next attack calculation!
        if (e.event_type == "dice" && e.die_result > 0)
        {
            latestDiceRoll = e.die_result;
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        if (healthManager == null) return;

        // Verify that the incoming particles match what can hurt us
        if (other.CompareTag(collisionTag))
        {
            hitCount++;

            if (hitCount >= hitsToDamage)
            {
                hitCount = 0; // Reset threshold counter

                // Calculate math: Base Damage * Networked Dice Multiplier
                int finalDamage = baseDamage * latestDiceRoll;
                
                Debug.Log($"{gameObject.name} took particle damage! Roll Multiplier: {latestDiceRoll} | Final: {finalDamage}");
                
                healthManager.TakeDamage(finalDamage);

                latestDiceRoll = 1; 
            }
        }
    }
}