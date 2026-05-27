using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthManager : MonoBehaviour
{
    [SerializeField] private int maxHp = 100;

    public int MaxHp => maxHp;
    public int CurrentHp { get; private set; }

    public UnityEvent<int> OnHpChanged;

    private static readonly Dictionary<string, HealthManager> _registry = new Dictionary<string, HealthManager>();

    void Awake()
    {
        CurrentHp = maxHp;
        _registry[gameObject.name] = this;
    }

    void OnDestroy()
    {
        _registry.Remove(gameObject.name);
    }

    public void TakeDamage(int amount)
    {
        CurrentHp = Mathf.Max(0, CurrentHp - amount);
        Debug.Log($"[HealthManager] {gameObject.name} took {amount} damage — HP: {CurrentHp}/{maxHp} | OnHpChanged listeners: {OnHpChanged?.GetPersistentEventCount()}");
        OnHpChanged?.Invoke(CurrentHp);
    }

    public void Heal(int amount)
    {
        CurrentHp = Mathf.Min(maxHp, CurrentHp + amount);
        OnHpChanged?.Invoke(CurrentHp);
    }

    public static void LogAllHp()
    {
        foreach (var kvp in _registry)
            Debug.Log($"[HP] {kvp.Key}: {kvp.Value.CurrentHp}/{kvp.Value.MaxHp}");
    }
}
