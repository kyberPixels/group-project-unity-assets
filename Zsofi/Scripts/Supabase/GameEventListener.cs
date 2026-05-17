using UnityEngine;
using TMPro;

public class GameEventListener : MonoBehaviour
{
    [SerializeField] private SupabaseRealtimeClient realtimeClient;
    [SerializeField] private Animator archerAnimator;
    [SerializeField] private Animator assassinAnimator;
    [SerializeField] private Animator sorcererAnimator;
    [SerializeField] private Animator dmAnimator;
    [SerializeField] private TMP_Text diceResultText;

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

            case "dice":
                HandleDieRoll(e.die_type, e.die_result);
                break;

            default:
                Debug.LogWarning("Unknown event_type: " + e.event_type);
                break;
        }
    }

    void HandleAction(string actionName)
    {
        Animator target = null;

        if (actionName.StartsWith("archer_"))        target = archerAnimator;
        else if (actionName.StartsWith("assassin_")) target = assassinAnimator;
        else if (actionName.StartsWith("sorcerer_")) target = sorcererAnimator;
        else if (actionName.StartsWith("dm_"))       target = dmAnimator;

        if (target == null)
        {
            Debug.LogWarning("No animator found for action: " + actionName);
            return;
        }

        Debug.Log("Calling SetTrigger(\"" + actionName + "\") on: " + target.gameObject.name);
        target.SetTrigger(actionName);
    }

    void HandleDieRoll(string dieType, int dieResult)
    {
        if (diceResultText != null)
            diceResultText.text = dieType + ": " + dieResult;

        Debug.Log("Die roll — " + dieType + " → " + dieResult);
    }
}
