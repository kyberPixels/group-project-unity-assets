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

    [Header("Movement")]
    [SerializeField] private CharacterMovement2 archerMovement;
    [SerializeField] private CharacterMovement2 assassinMovement;
    [SerializeField] private CharacterMovement2 sorcererMovement;
    [SerializeField] private CharacterMovement2 dmMovement;
    public ParticleSystem leaf;
    public ParticleSystem fire;

    void OnEnable()
    {
        if (realtimeClient == null) { Debug.LogError("[GameEventListener] realtimeClient is not assigned in the Inspector!"); return; }
        realtimeClient.OnGameEvent += HandleGameEvent;
        realtimeClient.OnPlayerStateChange += HandlePlayerState;
        Debug.Log("[GameEventListener] Subscribed to Supabase events.");
    }

    void OnDisable()
    {
        if (realtimeClient == null) return;
        realtimeClient.OnGameEvent -= HandleGameEvent;
        realtimeClient.OnPlayerStateChange -= HandlePlayerState;
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

        if (actionName.StartsWith("archer_")) target = archerAnimator;
        else if (actionName.StartsWith("assassin_")) target = assassinAnimator;
        else if (actionName.StartsWith("sorcerer_")) target = sorcererAnimator;
        else if (actionName.StartsWith("dm_")) target = dmAnimator;

        if (target == null)
        {
            Debug.LogWarning("No animator found for action: " + actionName);
            return;
        }

        Debug.Log("Calling SetTrigger(\"" + actionName + "\") on: " + target.gameObject.name);
        target.SetTrigger(actionName);
        if (actionName == "sorcerer_fight") leaf.Play();
        if (actionName == "dm_fight") fire.Play();
    }

    void HandleDieRoll(string dieType, int dieResult)
    {
        if (diceResultText != null)
            diceResultText.text = dieType + ": " + dieResult;

        Debug.Log("Die roll — " + dieType + " → " + dieResult);
    }

    void HandlePlayerState(PlayerState ps)
    {
        Debug.Log($"[player_state] type={ps.state_type} | vel={ps.velocity} | rot={ps.rotation}");

        CharacterMovement2 target = null;
        if (ps.state_type != null && ps.state_type.StartsWith("archer_")) target = archerMovement;
        else if (ps.state_type != null && ps.state_type.StartsWith("assassin_")) target = assassinMovement;
        else if (ps.state_type != null && ps.state_type.StartsWith("sorcerer_")) target = sorcererMovement;
        else if (ps.state_type != null && ps.state_type.StartsWith("dm_")) target = dmMovement;

        if (target == null) return;

        target.ReceiveNetworkInput(ps.velocity, ps.rotation);
    }
}
