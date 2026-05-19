using UnityEngine;

public class HeartHealthBar : MonoBehaviour
{
    [SerializeField] private HealthManager healthManager;
    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private int maxHearts = 5;
    [SerializeField] private float heartSpacing = 60f;

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
        Refresh(healthManager.CurrentHp);
    }

    void Refresh(int currentHp)
    {
        int hpPerHeart = healthManager.MaxHp / maxHearts;
        int fullHearts = Mathf.CeilToInt((float)currentHp / hpPerHeart);

        for (int i = 0; i < maxHearts; i++)
            _hearts[i].SetActive(i < fullHearts);
    }

    void OnDestroy()
    {
        if (healthManager != null)
            healthManager.OnHpChanged.RemoveListener(Refresh);
    }
}
