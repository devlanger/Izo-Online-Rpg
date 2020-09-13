using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginController : MonoBehaviour
{
    public static LoginController Instance { get; set; }

    public bool LoggedIn { get; set; }

    public event Action OnLoggedIn = delegate { };

    void Awake()
    {
        Instance = this;
    }

    public void InvokeLogin()
    {
        LoggedIn = true;
        OnLoggedIn();
    }
}
