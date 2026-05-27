using UnityEngine;

public class HeartHealthBar : MonoBehaviour
{
    [SerializeField] private HealthManager healthManager;
    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private int maxHearts = 5;
    [SerializeField] private float heartSpacing = 60f;

    [Header("Death")]
    [SerializeField] private Animator animator;

    private GameObject[] _hearts;

    void Start()
    {
        _hearts = new GameObject[maxHearts];
        for (int i = 0; i < maxHearts; i++)
        {
            _hearts[i] = Instantiate(heartPrefab, transform);
            _hearts[i].transform.localPosition = new Vector3(i * heartSpacing, 0, 0);
        }

        healthManager.OnHpChanged.AddListener(Refresh);
    }

    void Refresh(int currentHp)
    {
        int hpPerHeart = healthManager.MaxHp / maxHearts;
        int heartsToKeep = currentHp > 0 ? Mathf.CeilToInt((float)currentHp / hpPerHeart) : 0;

        for (int i = heartsToKeep; i < maxHearts; i++)
        {
            if (_hearts[i] != null)
            {
                Destroy(_hearts[i]);
                _hearts[i] = null;
            }
        }

        if (currentHp <= 0 && animator != null)
            animator.SetTrigger(healthManager.gameObject.tag + "_die");
    }

    void OnDestroy()
    {
        if (healthManager != null)
            healthManager.OnHpChanged.RemoveListener(Refresh);
    }
}
