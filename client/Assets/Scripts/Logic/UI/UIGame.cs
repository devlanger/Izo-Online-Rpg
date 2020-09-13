using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGame : MonoBehaviour
{
    public void Logout()
    {
        Connection.Instance.Disconnect();
    }
}
