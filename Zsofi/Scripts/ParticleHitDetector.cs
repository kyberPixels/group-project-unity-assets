using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleHitDetector : MonoBehaviour
{
    [Header("Character Identity")]
    [Tooltip("Set to the character prefix: archer, assassin, sorcerer, or dm")]
    [SerializeField] private string characterId;

    [Header("Hostile Particle Tags")]
    [Tooltip("Tags of particle system GameObjects that can hit this character.\n" +
             "Players: add the enemy's particle tag.\n" +
             "Enemy: add each player's particle tag.")]
    public List<string> hostileParticleTags = new List<string>();

    [Header("Health")]
    [SerializeField] private HealthManager healthManager;

    private static readonly Dictionary<string, ParticleHitDetector> _registry =
        new Dictionary<string, ParticleHitDetector>();

    private readonly List<ParticleCollisionEvent> _collisionEvents = new List<ParticleCollisionEvent>();
    private int _accumulatedHits;
    private Coroutine _settleCoroutine;

    private void Awake()
    {
        if (!string.IsNullOrEmpty(characterId))
            _registry[characterId] = this;
    }

    private void OnDestroy()
    {
        if (!string.IsNullOrEmpty(characterId) && _registry.ContainsKey(characterId))
            _registry.Remove(characterId);
    }

    private void OnParticleCollision(GameObject other)
    {
        Debug.Log($"[ParticleHitDetector] OnParticleCollision fired on {gameObject.name} from \"{other.name}\"");

        if (hostileParticleTags.Count == 0)
        {
            Debug.LogWarning($"[ParticleHitDetector] {gameObject.name}: hostileParticleTags is empty — no collisions will register.");
            return;
        }

        bool isHostile = false;
        foreach (string tag in hostileParticleTags)
        {
            if (other.CompareTag(tag)) { isHostile = true; break; }
        }
        if (!isHostile) return;

        ParticleSystem ps = other.GetComponent<ParticleSystem>();
        int count = ps != null ? ps.GetCollisionEvents(gameObject, _collisionEvents) : 1;

        AccumulateHit(count);
    }

    private void AccumulateHit(int count)
    {
        _accumulatedHits += count;
        if (_settleCoroutine != null) StopCoroutine(_settleCoroutine);
        _settleCoroutine = StartCoroutine(SettleDamage());
    }

    private IEnumerator SettleDamage()
    {
        yield return new WaitForSeconds(0.5f);

        // If the last roller was a player (not DM), only that player can be attacked by DM particles
        string lastRoller = GameEventListener.LastDiceRollingCharacter;
        bool dmIsAttacking = characterId != "dm";
        bool playerRolledLast = !string.IsNullOrEmpty(lastRoller) && lastRoller != "dm";
        if (dmIsAttacking && playerRolledLast && characterId != lastRoller)
        {
            _accumulatedHits = 0;
            _settleCoroutine = null;
            yield break;
        }

        int dice = Mathf.Max(1, GameEventListener.LastDiceResult);
        int damage = _accumulatedHits * dice;
        Debug.Log($"[ParticleHit] {gameObject.name} — {_accumulatedHits} particles × dice {dice} = {damage} damage");
        if (healthManager != null)
        {
            healthManager.TakeDamage(damage);
            HealthManager.LogAllHp();
        }
        _accumulatedHits = 0;
        _settleCoroutine = null;
    }
}
