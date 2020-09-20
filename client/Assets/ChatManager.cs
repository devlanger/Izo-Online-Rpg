using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatManager : Singleton<ChatManager>
{
    public event Action<string> OnMessageReceived = delegate { };

    private void Awake()
    {
        Instance = this;
    }

    public void AddMessage(string message)
    {
        OnMessageReceived(message);
    }
}
