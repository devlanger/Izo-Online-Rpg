using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Playables;

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
        { GamePacketType.DAMAGE_INFO, DamageInfoImpl },
        { GamePacketType.SYNC_STAT, SyncStatImpl },
        { GamePacketType.SYNC_INVENTORY, SyncInventoryImpl },
        { GamePacketType.EXECUTE_SKILL_TARGET, ExecuteSkillImpl },
        { GamePacketType.SET_POSITION, SetPositionImpl },
        { GamePacketType.CHAT_MESSAGE_PACKET, ChatMessageImpl },
    };

    private static void ChatMessageImpl(BinaryReader reader)
    {
        string msg = reader.ReadString();

        ChatManager.Instance.AddMessage(msg);
    }

    private static void SetPositionImpl(BinaryReader reader)
    {
        int targetId = reader.ReadInt32();
        short posX = reader.ReadInt16();
        short posZ = reader.ReadInt16();

        Character attacker = CharactersManager.Instance.GetPlayer(targetId);
        if (attacker != null)
        {
            Vector3 pos = attacker.transform.position;
            pos.x = posX;
            pos.z = posZ;

            attacker.transform.position = pos;
            attacker.MoveTo(pos);
        }
    }

    private static void ExecuteSkillImpl(BinaryReader reader)
    {
        int attackerId = reader.ReadInt32();
        int targetId = reader.ReadInt32();
        int skillId = reader.ReadInt32();

        Debug.Log(string.Format("Attacker [{0}] Used skill [{1}] on target [{2}]", attackerId, skillId, targetId));

        Character attacker = CharactersManager.Instance.GetPlayer(attackerId);
        if (attacker != null)
        {
            attacker.LastTargetId = targetId;
            SkillDataHandler data = SkillsManager.Instance.GetSkillData(skillId);
            if (data == null)
            {
                Debug.LogError("No skill data found: " + skillId);
                return;
            }

            var director = attacker.GetComponentInChildren<PlayableDirector>();
            director.time = 0;
            director.Play(data.animationClip);
        }
    }

    private static void SyncInventoryImpl(BinaryReader reader)
    {
        int characterId = reader.ReadInt32();
        Character c = CharactersManager.Instance.GetPlayer(characterId);
        ItemContainerId inventoryId = (ItemContainerId)reader.ReadByte();
        ushort count = reader.ReadUInt16();
        
        if(inventoryId == ItemContainerId.SHOP)
        {
            c = CharactersManager.Instance.GetLocalPlayer();
        }

        c.itemContainers[inventoryId].Clear();

        for (int i = 0; i < count; i++)
        {
            ushort slot = reader.ReadUInt16();
            int itemId = reader.ReadInt32();

            c.SetItem(inventoryId, slot, itemId);
        }

        c.RefreshInventory(inventoryId);

        if(inventoryId == ItemContainerId.SHOP && count != 0)
        {
            UIVendor.Instance.GetComponent<UIPanel>().Activate();
        }
    }

    private static void SyncStatImpl(BinaryReader reader)
    {
        int targetId = reader.ReadInt32();
        StatType type = (StatType)reader.ReadByte();
        Character target = CharactersManager.Instance.GetPlayer(targetId);
        switch (type)
        {
            case StatType.HEALTH:
            case StatType.MAX_HEALTH:
            case StatType.MANA:
            case StatType.EXPERIENCE:
            case StatType.GOLD:
                var val = reader.ReadInt32();
                if (target != null)
                {
                    target.SetStat(type, val);
                }
                break;
            case StatType.LEVEL:
                var valShort = reader.ReadInt16();
                if (target != null)
                {
                    target.SetStat(type, valShort);
                }
                break;

        }
    }

    private static void DamageInfoImpl(BinaryReader reader)
    {
        int targetId = reader.ReadInt32();
        ushort damageValue = reader.ReadUInt16();
        byte damageLabelColor = reader.ReadByte();

        CombatManager.Instance.DealDamage(new DamageInfo()
        {
            targetId = targetId,
            damage = damageValue,
            type = damageLabelColor
        });
    }

    private static void ExcecuteAttackTargetImpl(BinaryReader reader)
    {
        int id = reader.ReadInt32();
        int targetId = reader.ReadInt32();

        Character c = CharactersManager.Instance.GetPlayer(id);
        Character target = CharactersManager.Instance.GetPlayer(targetId);
        if (c != null)
        {
            c.GetComponentInChildren<Animator>().SetTrigger("attack");
            if (target != null)
            {
                c.LookAt(target.transform.position);
            }
        }
    }

    public static void ExecutePacket(GamePacketType type, BinaryReader reader)
    {
        try
        {
            Debug.Log("Execute packet: " + type);
            if (packetsImplementation.ContainsKey(type))
            {
                packetsImplementation[type].Invoke(reader);
            }
        }
        catch(Exception ex)
        {
            Debug.LogError(ex.ToString());
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
            baseId = reader.ReadInt32(),
            nickname = reader.ReadString(),
            posX = reader.ReadInt16(),
            posZ = reader.ReadInt16(),
            rotation = reader.ReadInt16(),
            race = reader.ReadByte(),
            @class = reader.ReadByte(),
            health = reader.ReadInt32(),
            maxHealth = reader.ReadInt32(),
        };

        CharactersManager.Instance.SpawnCharacter(data);
    }

    private static void SetDestinationImpl(BinaryReader reader)
    {
        int id = reader.ReadInt32();

        short posX = reader.ReadInt16();
        short posY = reader.ReadInt16();

        Character p = CharactersManager.Instance.GetPlayer(id);
        if (p != null)
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
        Debug.Log("Despawn player: " + id);

        CharactersManager.Instance.DespawnCharacter(id);
    }

    private static void EnterWorldImpl(BinaryReader reader)
    {
        int id = reader.ReadInt32();
        short posX = reader.ReadInt16();
        short posY = reader.ReadInt16();

        short lvl = reader.ReadInt16();
        int exp = reader.ReadInt32();
        int gold = reader.ReadInt32();

        Character c = CharactersManager.Instance.GetPlayer(id);
        c.stats[StatType.EXPERIENCE] = exp;
        c.stats[StatType.LEVEL] = lvl;
        c.stats[StatType.GOLD] = gold;

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