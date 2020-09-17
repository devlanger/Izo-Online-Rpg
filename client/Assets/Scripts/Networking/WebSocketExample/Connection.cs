using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NativeWebSocket;
using System.IO;
using UnityEngine.SceneManagement;

public class Connection : MonoBehaviour
{
    public static Connection Instance { get; private set; }

    public WebSocket websocket;

    [SerializeField]
    private string ip = "127.0.0.1:3000";

    [SerializeField]
    private string remoteIp = "127.0.0.1:3000";

    [SerializeField]
    private bool useRemoteIp = false;

    [SerializeField]
    private GameObject connectionPrefab;

    private Coroutine routine;
    private bool connected = false;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        //StartConnection();
    }

    private async void StartConnection()
    {
        await websocket.Connect();
    }

    private IEnumerator ReconnectionRoutine()
    {
        while(!connected)
        {
            if (!connected)
            {
                TryConnect();
            }

            yield return new WaitUntil(() => websocket.State != WebSocketState.Connecting);
            yield return new WaitForSeconds(0.25f);
        }
    }

    private void Start()
    {
        StartCoroutine(ReconnectionRoutine());
    }


    async void TryConnect()
    {
        string ip = useRemoteIp ? remoteIp : this.ip;
        websocket = new WebSocket(ip);
        Debug.Log("Connect to: " + ip);
        websocket.OnOpen += () =>
        {
            connected = true;
            if(routine != null)
            {
                StopCoroutine(routine);
            }
            Debug.Log("Connection open!");
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                SceneManager.LoadScene(1);
            });
        };

        websocket.OnError += (e) =>
        {
            Debug.Log("Error! " + e);
        };

        websocket.OnClose += (e) =>
        {
            connected = false;

            Debug.Log("Connection closed!");
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                if(SceneManager.GetActiveScene().buildIndex == 1)
                {
                    Destroy(gameObject);
                    SceneManager.LoadScene(0);
                }
                else
                {
                    //Destroy(gameObject);
                    //Instantiate(connectionPrefab);
                }
            });
        };

        websocket.OnMessage += (bytes) =>
        {
            // Reading a plain text message
            // var message = System.Text.Encoding.UTF8.GetString(bytes);
            // Debug.Log("OnMessage! " + message);

            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                MemoryStream stream = new MemoryStream(bytes);
                BinaryReader reader = new BinaryReader(stream);

                while (stream.Position < reader.BaseStream.Length)
                {
                    byte packetType = reader.ReadByte();
                    GamePacketType packet = (GamePacketType)packetType;

                    GamePacketsImpl.ExecutePacket(packet, reader);
                }
            });
        };

        //routine = StartCoroutine(ReconnectionRoutine());
        await websocket.Connect();
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        if (websocket != null)
        {
            websocket.DispatchMessageQueue();
        }
#endif
    }

    public void SendData(byte[] data)
    {
        SendWebSocketMessage(data);
    }

    public void SendData(Packet packet)
    {
        SendWebSocketMessage(packet.Data);
    }

    async void SendWebSocketMessage(byte[] data)
    {
        if (websocket.State == WebSocketState.Open)
        {
            await websocket.Send(data);
        }
    }

    private void OnDestroy()
    {
        websocket.CancelConnection();
    }

    public void Disconnect()
    {
        websocket.CancelConnection();
    }
}
