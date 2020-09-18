using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CharactersManager : MonoBehaviour
{
    public static CharactersManager Instance { get; set; }

    public Dictionary<int, Character> characters = new Dictionary<int, Character>();

    public Character characterPrefab;

    public event Action<int, Character> OnCharacterSpawned = delegate { };
    public event Action<int, Character> OnCharacterDespawned = delegate { };

    private void Awake()
    {
        Instance = this;
    }

    public void SpawnCharacter(CharacterData spawnData)
    {
        if(characters.ContainsKey(spawnData.id))
        {
            return;
        }

        Character character = Instantiate(characterPrefab, new Vector3(spawnData.posX, 1, spawnData.posZ), Quaternion.identity, transform);
        character.Data = spawnData;

        OnCharacterSpawned(spawnData.id, character);
        characters.Add(spawnData.id, character);
    }

    public Character GetPlayer(int id)
    {
        if(characters.ContainsKey(id))
        {
            return characters[id];
        }

        return null;
    }

    public Character GetLocalPlayer()
    {
        return PlayerController.Instance.Target;
    }

    public void DespawnCharacter(int id)
    {
        if(!characters.ContainsKey(id))
        {
            return;
        }

        OnCharacterDespawned(id, characters[id]);
        Destroy(characters[id].gameObject, 1);
        characters.Remove(id);
    }
}

[System.Serializable]
public class CharacterData
{
    public int id;
    public string nickname;
    public byte lvl;
    public short posX;
    public short posZ;
    public byte race;
    public byte @class;
}