using System;
using System.Collections;
using UnityEngine;
using NativeWebSocket;

public class GameEvent
{
    public string event_type;
    public string action_name;
    public string die_type;
    public int die_result;
    public string group_id;
    public float velocity;
    public float direction;
}

public class PlayerState
{
    public string user_id;
    public string group_id;
    public string state_type;
    public float velocity;
    public float rotation;
    public string last_die_type;
    public int last_die_result;
}

public class SupabaseRealtimeClient : MonoBehaviour
{
    public static SupabaseRealtimeClient Instance { get; private set; }

    public event Action<GameEvent> OnGameEvent;
    public event Action<PlayerState> OnPlayerStateChange;

    [SerializeField] private SupabaseConfig config;
    [SerializeField] private string groupId;

    private WebSocket _socket;
    private int _ref = 1;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    async void Start()
    {
        if (config == null) { Debug.LogError("[Supabase] SupabaseConfig is not assigned!"); return; }
        if (string.IsNullOrEmpty(groupId)) { Debug.LogError("[Supabase] groupId is empty!"); return; }

        string wsUrl = config.supabaseUrl
            .Replace("https://", "wss://")
            .Replace("http://", "ws://");
        wsUrl += "/realtime/v1/websocket?apikey=" + config.anonKey + "&vsn=1.0.0";

        _socket = new WebSocket(wsUrl);

        _socket.OnOpen += () =>
        {
            JoinChannel();
            StartCoroutine(HeartbeatLoop());
        };

        _socket.OnMessage += (bytes) =>
        {
            string json = System.Text.Encoding.UTF8.GetString(bytes);
            HandleMessage(json);
        };

        _socket.OnError += (error) => Debug.LogError("[Supabase] WebSocket error: " + error);
        _socket.OnClose += (code) => { };

        await _socket.Connect();
    }

    void JoinChannel()
    {
        string filter = "group_id=eq." + groupId;
        string joinMsg =
            "{" +
                "\"topic\":\"realtime:game-events-" + groupId + "\"," +
                "\"event\":\"phx_join\"," +
                "\"payload\":{" +
                    "\"config\":{" +
                        "\"broadcast\":{\"self\":false}," +
                        "\"presence\":{\"key\":\"\"}," +
                        "\"postgres_changes\":[" +
                            "{" +
                                "\"event\":\"INSERT\"," +
                                "\"schema\":\"public\"," +
                                "\"table\":\"game_events\"," +
                                "\"filter\":\"" + filter + "\"" +
                            "}," +
                            "{" +
                                "\"event\":\"INSERT\"," +
                                "\"schema\":\"public\"," +
                                "\"table\":\"player_state\"," +
                                "\"filter\":\"" + filter + "\"" +
                            "}," +
                            "{" +
                                "\"event\":\"UPDATE\"," +
                                "\"schema\":\"public\"," +
                                "\"table\":\"player_state\"," +
                                "\"filter\":\"" + filter + "\"" +
                            "}" +
                        "]" +
                    "}" +
                "}," +
                "\"ref\":\"" + _ref++ + "\"" +
            "}";

        _ = _socket.SendText(joinMsg);
    }

    IEnumerator HeartbeatLoop()
    {
        while (_socket.State == WebSocketState.Open)
        {
            yield return new WaitForSeconds(25f);
            string hb = "{\"topic\":\"phoenix\",\"event\":\"heartbeat\",\"payload\":{},\"ref\":\"" + _ref++ + "\"}";
            _ = _socket.SendText(hb);
        }
    }

    void HandleMessage(string json)
    {
        if (!json.Contains("\"event\":\"postgres_changes\"")) return;

        try
        {
            WsMessage msg = JsonUtility.FromJson<WsMessage>(json);

            if (msg == null)                       { Debug.LogWarning("[Supabase] Parse failed: msg is null."); return; }
            if (msg.payload == null)               { Debug.LogWarning("[Supabase] Parse failed: payload is null."); return; }
            if (msg.payload.data == null)          { Debug.LogWarning("[Supabase] Parse failed: data is null."); return; }
            if (msg.payload.data.record == null)   { Debug.LogWarning("[Supabase] Parse failed: record is null. Raw: " + json); return; }

            WsData d = msg.payload.data;
            WsRecord r = d.record;

            if (d.table == "player_state")
            {
                OnPlayerStateChange?.Invoke(new PlayerState
                {
                    user_id = r.user_id,
                    group_id = r.group_id,
                    state_type = r.state_type,
                    velocity = r.velocity,
                    rotation = r.rotation,
                    last_die_type = r.last_die_type,
                    last_die_result = r.last_die_result
                });
            }
            else
            {
                OnGameEvent?.Invoke(new GameEvent
                {
                    event_type = r.event_type,
                    action_name = r.action_name,
                    die_type = r.die_type,
                    die_result = r.die_result,
                    group_id = r.group_id
                });
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("[Supabase] Parse exception: " + e.Message + "\nRaw: " + json);
        }
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        _socket?.DispatchMessageQueue();
#endif
    }

    async void OnApplicationQuit()
    {
        if (_socket != null)
            await _socket.Close();
    }

    [Serializable] private class WsMessage { public WsPayload payload; }
    [Serializable] private class WsPayload { public WsData data; }
    [Serializable] private class WsData { public string table; public WsRecord record; }
    [Serializable]
    private class WsRecord
    {
        // game_events fields
        public string group_id;
        public string event_type;
        public string action_name;
        public string die_type;
        public int die_result;
        // player_state fields
        public string user_id;
        public string state_type;
        public float velocity;
        public float rotation;
        public string last_die_type;
        public int last_die_result;
    }
}
