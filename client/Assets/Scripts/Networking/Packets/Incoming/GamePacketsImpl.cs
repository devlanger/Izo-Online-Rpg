using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class GamePacketsImpl
{
    private static Dictionary<GamePacketType, Action<BinaryReader>> packetsImplementation = new Dictionary<GamePacketType, Action<BinaryReader>>()
    {
        { GamePacketType.ENTER_GAME_WORLD, EnterWorldImpl },
        { GamePacketType.CREATE_CHARACTER, CreateCharacterImpl },
        { GamePacketType.SPAWN_CHARACTER, SpawnCharacterImpl },
        { GamePacketType.DESPAWN_CHARACTER, DespawnCharacterImpl },
        { GamePacketType.EXECUTE_ATTACK_TARGET, ExcecuteAttackTargetImpl},
        { GamePacketType.SET_DESTINATION, SetDestinationImpl },
    };

    private static void ExcecuteAttackTargetImpl(BinaryReader reader)
    {
        int id = reader.ReadInt32();
        int targetId = reader.ReadInt32();

        Character c = SpawnManager.Instance.GetPlayer(id);
        if(c != null)
        {
            c.GetComponentInChildren<Animator>().SetTrigger("attack");
        }
    }

    public static void ExecutePacket(GamePacketType type, BinaryReader reader)
    {
        Debug.Log("Execute packet: " + type);
        if(packetsImplementation.ContainsKey(type))
        {
            packetsImplementation[type].Invoke(reader);
        }
    }

    private static void CreateCharacterImpl(BinaryReader obj)
    {
        CharacterCreationController.Instance.GetComponent<UIPanel>().Activate();
    }

    private static void SpawnCharacterImpl(BinaryReader reader)
    {
        CharacterData data = new CharacterData()
        {
            id = reader.ReadInt32(),
            nickname = reader.ReadString(),
            posX = reader.ReadInt16(),
            posZ = reader.ReadInt16(),
            race = reader.ReadByte(),
            @class = reader.ReadByte(),
        };
        Debug.Log(data.id + " " + data.nickname);
        SpawnManager.Instance.SpawnCharacter(data);
    }

    private static void SetDestinationImpl(BinaryReader reader)
    {
        int id = reader.ReadInt32();

        short posX = reader.ReadInt16();
        short posY = reader.ReadInt16();

        Character p = SpawnManager.Instance.GetPlayer(id);
        if(p != null)
        {
            Vector3 destination = new Vector3(posX, 0, posY);
            Vector3 pos = p.transform.position;
            pos.y = 0;

            p.transform.rotation = Quaternion.LookRotation(destination - pos);
            p.MoveTo(destination);
        }
    }

    private static void DespawnCharacterImpl(BinaryReader reader)
    {
        int id = reader.ReadInt32();

        SpawnManager.Instance.DespawnCharacter(id);
    }

    private static void EnterWorldImpl(BinaryReader reader)
    {
        int id = reader.ReadInt32();
        short posX = reader.ReadInt16();
        short posY = reader.ReadInt16();

        Character c = SpawnManager.Instance.GetPlayer(id);
        Debug.Log("Set player: " + c.Data.id);
        PlayerController.Instance.SetPlayer(c);

        Vector3 pos = PlayerController.Instance.Target.transform.position;
        pos.x = posX;
        pos.z = posY;

        PlayerController.Instance.Target.transform.position = pos;
        PlayerController.Instance.Target.MoveTo(pos);

        CharacterCreationController.Instance.GetComponent<UIPanel>().Deactivate();
        LoginController.Instance.InvokeLogin();
    }
}