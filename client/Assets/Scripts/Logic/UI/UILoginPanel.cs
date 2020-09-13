using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILoginPanel : MonoBehaviour
{
    [SerializeField]
    private InputField usernameField;

    [SerializeField]
    private InputField passwordField;

    [SerializeField]
    private Button loginButton;

    [SerializeField]
    private Toggle saveUsernameToggle;

    private void Start()
    {
        usernameField.onEndEdit.AddListener(SaveUsername);
        loginButton.onClick.AddListener(Login);
        saveUsernameToggle.onValueChanged.AddListener(SaveUsernameToggleChanged);

        if (PlayerPrefs.GetInt("save_username", 0) == 1)
        {
            saveUsernameToggle.isOn = true;
            usernameField.text = PlayerPrefs.GetString("username", "");
        }
        else
        {
            saveUsernameToggle.isOn = false;
            usernameField.text = "";
        }

        LoginController.Instance.OnLoggedIn += Instance_OnLoggedIn;
    }

    private void SaveUsernameToggleChanged(bool isOn)
    {
        PlayerPrefs.SetInt("save_username", isOn ? 1 : 0);
    }

    private void SaveUsername(string username)
    {
        if (saveUsernameToggle.isOn)
        {
            PlayerPrefs.SetString("username", username);
        }
    }

    private void Instance_OnLoggedIn()
    {
        GetComponent<UIPanel>().Deactivate();
    }

    private void Login()
    {
        LoginRequestPacket packet = new LoginRequestPacket(usernameField.text, passwordField.text);
        Connection.Instance.SendData(packet.Data);
    }
}
