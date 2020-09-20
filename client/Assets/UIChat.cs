using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIChat : MonoBehaviour
{
    [SerializeField]
    private Text messagePrefab;

    [SerializeField]
    private InputField inputField;

    [SerializeField]
    private Button sendButton;

    [SerializeField]
    private Transform parent;

    [SerializeField]
    private ScrollRect scrollRect;

    private void Awake()
    {
        sendButton.onClick.AddListener(Send);
    }

    private void Send()
    {
        if(inputField.text.Length == 0)
        {
            return;
        }

        Connection.Instance.SendData(new ChatMessagePacket(inputField.text));
        inputField.text = "";
        EventSystem.current.SetSelectedGameObject(null);
    }

    private void Start()
    {
        ChatManager.Instance.OnMessageReceived += Instance_OnMessageReceived;
    }

    private void Instance_OnMessageReceived(string obj)
    {
        var inst = Instantiate(messagePrefab, parent);
        inst.text = obj;

        inst.transform.parent = parent;

        StartCoroutine(Scroll());
    }

    private IEnumerator Scroll()
    {
        yield return 0;
        scrollRect.normalizedPosition = new Vector2(0, 0);
    }
}
