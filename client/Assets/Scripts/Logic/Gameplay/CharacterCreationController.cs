using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCreationController : MonoBehaviour
{
    public static CharacterCreationController Instance { get; set; }

    public int heroClass = 0;
    public int race = 0;

    [SerializeField]
    private InputField nicknameField;

    [SerializeField]
    private Button enterWorldButton;

    private void Awake()
    {
        Instance = this;
        enterWorldButton.onClick.AddListener(EnterWorld);
    }

    public void SetClass(int heroClass)
    {
        this.heroClass = heroClass;
    }

    public void SetRace(int race)
    {
        this.race = race;
    }

    public void EnterWorld()
    {
        if(nicknameField.text.Length < 3)
        {
            return;
        }

        Packet packet = new Packet();
        packet.writer.Write((byte)2);
        packet.writer.Write((string)nicknameField.text);
        packet.writer.Write((byte)heroClass);
        packet.writer.Write((byte)race);

        Connection.Instance.SendData(packet);
    }
}
