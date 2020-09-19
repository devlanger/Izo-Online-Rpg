using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;

public class CharactersManager : MonoBehaviour
{
    public static CharactersManager Instance { get; set; }

    public Dictionary<int, Character> characters = new Dictionary<int, Character>();

    public Character characterPrefab;

    public event Action<int, Character> OnCharacterSpawned = delegate { };
    public event Action<int, Character> OnCharacterDespawned = delegate { };

    public List<MobData> mobs { get; private set; } = new List<MobData>();

    private void Awake()
    {
        Instance = this;

        mobs = Resources.LoadAll<MobData>("Data/Mobs").ToList();
    
    }

    public void SpawnCharacter(CharacterData spawnData)
    {
        if(characters.ContainsKey(spawnData.id))
        {
            return;
        }

        Character character = Instantiate(characterPrefab, new Vector3(spawnData.posX, 1, spawnData.posZ), Quaternion.identity, transform);
        character.Data = spawnData;

        MobData mobData = GetMobData(spawnData.baseId);
        if(mobData != null)
        {
            GameObject model = Instantiate(mobData.characterModel, character.transform.position - Vector3.up, character.transform.rotation, character.transform);

            Destroy(character.baseModel);
            character.baseModel = model;
        }

        Animator modelAnimator = character.baseModel.GetComponent<Animator>();
        //character.Animator.runtimeAnimatorController = modelAnimator.runtimeAnimatorController;
        //character.Animator.avatar = modelAnimator.avatar;
        //character.Animator.Rebind();

        character.Animator = modelAnimator;
        var pd = character.baseModel.AddComponent<PlayableDirector>();
        var tr = character.baseModel.AddComponent<TimelineReceiver>();
        tr.user = character;
        

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

    public MobData GetMobData(int id)
    {
        return mobs.Find(m => m.id == id);
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
    public int baseId;
    public string nickname;
    public byte lvl;
    public short posX;
    public short posZ;
    public byte race;
    public byte @class;
}