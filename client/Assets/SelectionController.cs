using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionController : MonoBehaviour
{
    public static SelectionController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void RequestSelectTarget(int targetId, byte action)
    {
        Connection.Instance.SendData(new SelectTargetPacket(targetId, action));
    }
}
