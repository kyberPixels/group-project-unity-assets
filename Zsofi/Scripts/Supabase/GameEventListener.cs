using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameEventListener : MonoBehaviour
{
    public static string LastDiceRollingCharacter { get; private set; }
    public static int LastDiceResult { get; private set; }
    public static readonly Dictionary<string, int> LastDiceByCharacter = new Dictionary<string, int>();

    private readonly Dictionary<string, int> _lastKnownDiceResult = new Dictionary<string, int>();

    [SerializeField] private SupabaseRealtimeClient realtimeClient;
    [SerializeField] private Animator diceAnimator; // SADA DODANO: Polje za tvoju kockicu!

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
    public ParticleSystem arrow;
    public ParticleSystem dragger1;
    public ParticleSystem dragger2;



    void OnEnable()
    {
        if (realtimeClient == null) { Debug.LogError("[GameEventListener] realtimeClient is not assigned in the Inspector!"); return; }
        realtimeClient.OnGameEvent += HandleGameEvent;
        realtimeClient.OnPlayerStateChange += HandlePlayerState;
    }

    void OnDisable()
    {
        if (realtimeClient == null) return;
        realtimeClient.OnGameEvent -= HandleGameEvent;
        realtimeClient.OnPlayerStateChange -= HandlePlayerState;
    }

    void HandleGameEvent(GameEvent e)
    {
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
        
        target.SetTrigger(actionName);
        if (actionName == "sorcerer_fight") leaf.Play();
        if (actionName == "dm_fight")       fire.Play();
        if (actionName == "archer_fight")   arrow.Play();
        if (actionName == "assassin_fight") dragger1.Play();
        if (actionName == "assassin_fight") dragger2.Play();
    }

    void HandleDieRoll(string dieType, int dieResult)
    {
        if (diceResultText != null)
        {
            diceResultText.text = dieType + ": " + dieResult;
        }

    }

    void HandlePlayerState(PlayerState ps)
    {
        CharacterMovement2 target = null;
        string charId = null;

        if (ps.state_type != null && ps.state_type.StartsWith("archer_"))        { target = archerMovement;   charId = "archer"; }
        else if (ps.state_type != null && ps.state_type.StartsWith("assassin_")) { target = assassinMovement; charId = "assassin"; }
        else if (ps.state_type != null && ps.state_type.StartsWith("sorcerer_")) { target = sorcererMovement; charId = "sorcerer"; }
        else if (ps.state_type != null && ps.state_type.StartsWith("dm_"))       { target = dmMovement;       charId = "dm"; }

        // SPAS OD ODLETANJA: Provjeravamo da li je ovaj paket zapravo novi roll kockice
        bool isNewDiceRoll = false;
        if (charId != null)
        {
            _lastKnownDiceResult.TryGetValue(charId, out int prev);
            if (ps.last_die_result > 0 && ps.last_die_result != prev)
            {
                isNewDiceRoll = true;
            }
        }

        // AKO JE ROLL U TOKU: Potpuno ignorišemo mrežni input za kretanje kako likovi ne bi odletjeli!
        if (target != null && !isNewDiceRoll)
        {
            target.ReceiveNetworkInput(ps.velocity, ps.rotation);
        }

        if (charId != null)
        {
            _lastKnownDiceResult.TryGetValue(charId, out int prev);
            if (ps.last_die_result > 0 && ps.last_die_result != prev)
            {
                LastDiceRollingCharacter = charId;
                LastDiceResult = ps.last_die_result;
                Debug.Log($"[GameEventListener] Dice saved — {charId} rolled {ps.last_die_result}");
                GameObject diceObject = GameObject.FindWithTag("Dice");

                if (diceObject != null)
                {
                    
                    // 2. Grab the Animator component from that 3D object
                    Animator dynamicDiceAnimator = diceObject.GetComponent<Animator>();

                    if (dynamicDiceAnimator != null)
                    {
                        // 3. Fire the animation
                        dynamicDiceAnimator.SetTrigger("roll");
                        // OR: dynamicDiceAnimator.Play("YourDiceRollAnimationClipName");
                    }
                    else
                    {
                        Debug.LogError("Found Dice object, but it is missing an Animator component!");
                    }
                } else
                {
                    Debug.LogError("Dice object");    
                }
            }
            _lastKnownDiceResult[charId] = ps.last_die_result;
            LastDiceByCharacter[charId] = ps.last_die_result;
        }
    }
}