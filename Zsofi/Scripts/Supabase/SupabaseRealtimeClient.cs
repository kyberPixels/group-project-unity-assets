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

public class SupabaseRealtimeClient : MonoBehaviour
{
    public static SupabaseRealtimeClient Instance { get; private set; }

    public event Action<GameEvent> OnGameEvent;

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
        string wsUrl = config.supabaseUrl
            .Replace("https://", "wss://")
            .Replace("http://", "ws://");
        wsUrl += "/realtime/v1/websocket?apikey=" + config.anonKey + "&vsn=1.0.0";

        _socket = new WebSocket(wsUrl);

        _socket.OnOpen += () =>
        {
            Debug.Log("Supabase Realtime connected");
            JoinChannel();
            StartCoroutine(HeartbeatLoop());
        };

        _socket.OnMessage += (bytes) =>
        {
            string json = System.Text.Encoding.UTF8.GetString(bytes);
            HandleMessage(json);
        };

        _socket.OnError += (error) => Debug.LogError("Supabase WS error: " + error);
        _socket.OnClose += (code) => Debug.Log("Supabase WS closed: " + code);

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
                        "\"postgres_changes\":[{" +
                            "\"event\":\"INSERT\"," +
                            "\"schema\":\"public\"," +
                            "\"table\":\"game_events\"," +
                            "\"filter\":\"" + filter + "\"" +
                        "}]" +
                    "}" +
                "}," +
                "\"ref\":\"" + _ref++ + "\"" +
            "}";

        _ = _socket.SendText(joinMsg);
        Debug.Log("Supabase subscribed for group: " + groupId);
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
            if (msg == null || msg.payload == null || msg.payload.data == null || msg.payload.data.record == null)
                return;

            WsRecord r = msg.payload.data.record;
            OnGameEvent?.Invoke(new GameEvent
            {
                event_type = r.event_type,
                action_name = r.action_name,
                die_type = r.die_type,
                die_result = r.die_result,
                group_id = r.group_id
            });
        }
        catch (Exception e)
        {
            Debug.LogWarning("Failed to parse game event: " + e.Message + "\nRaw: " + json);
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
    [Serializable] private class WsData { public WsRecord record; }
    [Serializable]
    private class WsRecord
    {
        public string group_id;
        public string event_type;
        public string action_name;
        public string die_type;
        public int die_result;
    }
}
