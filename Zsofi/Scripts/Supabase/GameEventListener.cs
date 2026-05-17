using UnityEngine;
using TMPro;

public class GameEventListener : MonoBehaviour
{
    [SerializeField] private SupabaseRealtimeClient realtimeClient;
    [SerializeField] private Animator characterAnimator;
    [SerializeField] private TMP_Text diceResultText;

    private static readonly int VelocityZ = Animator.StringToHash("Velocity Z");

    void OnEnable()
    {
        realtimeClient.OnGameEvent += HandleGameEvent;
    }

    void OnDisable()
    {
        realtimeClient.OnGameEvent -= HandleGameEvent;
    }

    void HandleGameEvent(GameEvent e)
    {
        Debug.Log("Game event received: " + e.event_type + " / " + e.action_name);

        switch (e.event_type)
        {
            case "action":
                HandleAction(e.action_name);
                break;

            case "die_roll":
                HandleDieRoll(e.die_type, e.die_result);
                break;

            default:
                Debug.LogWarning("Unknown event_type: " + e.event_type);
                break;
        }
    }

    void HandleAction(string actionName)
    {
        if (characterAnimator == null) return;

        switch (actionName)
        {
            case "walk":
                characterAnimator.SetFloat(VelocityZ, 1f);
                break;
            case "stop":
                characterAnimator.SetFloat(VelocityZ, 0f);
                break;
            case "attack":
                characterAnimator.SetTrigger("Attack");
                break;
            default:
                Debug.LogWarning("Unknown action_name: " + actionName);
                break;
        }
    }

    void HandleDieRoll(string dieType, int dieResult)
    {
        if (diceResultText != null)
            diceResultText.text = dieType + ": " + dieResult;

        Debug.Log("Die roll — " + dieType + " → " + dieResult);
    }
}
