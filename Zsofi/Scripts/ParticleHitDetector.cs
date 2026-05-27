using System.Collections.Generic;
using UnityEngine;

// Attach this to any character that can receive particle hits.
// Set hostileParticleTags to the tags of particle systems that should damage THIS entity:
//   - On Katarina / Mano / Ashe: add the enemy/DM particle tags (e.g. "EnemyAttack")
//   - On the DM / enemy: add the player particle tags (e.g. "PlayerAttack")
// This naturally prevents friendly fire — players only list enemy tags, never each other's.
//
// IMPORTANT Unity setup required:
//   1. Each attacking particle system must have "Send Collision Messages" checked in its inspector.
//   2. This GameObject (the receiver) must have a Collider component.
//   3. Tag the particle system's GameObject with one of the hostile tags listed below.
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

    private static readonly Dictionary<string, ParticleHitDetector> _registry =
        new Dictionary<string, ParticleHitDetector>();

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

    private void Start()
    {
        string tags = hostileParticleTags.Count > 0 ? string.Join(", ", hostileParticleTags) : "NONE";
        Debug.Log($"[ParticleHitDetector] {gameObject.name} ready — listening for tags: [{tags}]");
    }

    private void OnParticleCollision(GameObject other)
    {
        if (hostileParticleTags.Count == 0)
        {
            Debug.LogWarning($"[ParticleHitDetector] {gameObject.name}: hostileParticleTags is empty — no collisions will register.");
            return;
        }

        bool isHostile = false;
        foreach (string tag in hostileParticleTags)
        {
            if (other.CompareTag(tag))
            {
                isHostile = true;
                break;
            }
        }

        if (!isHostile) return;

        string activeRoller = GameEventListener.LastDiceRollingCharacter;

        if (!string.IsNullOrEmpty(activeRoller) && activeRoller != characterId)
        {
            // Particles hit the wrong character physically — redirect to the correct one
            if (_registry.TryGetValue(activeRoller, out ParticleHitDetector target))
                target.RegisterHit(other);
            else
                Debug.LogWarning($"[ParticleHitDetector] No registered detector for active roller \"{activeRoller}\"");
            return;
        }

        RegisterHit(other);
    }

    private void RegisterHit(GameObject other)
    {
        Debug.Log($"[ParticleHit] {gameObject.name} was hit by particles from \"{other.name}\" (tag: {other.tag})");
    }
}
