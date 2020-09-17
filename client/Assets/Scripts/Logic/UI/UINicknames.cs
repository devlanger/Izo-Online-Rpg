using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UINicknames : MonoBehaviour
{
    [SerializeField]
    private NicknameHoverLabel hoverLabelPrefab;
    private Dictionary<int, NicknameHoverLabel> labels = new Dictionary<int, NicknameHoverLabel>();

    private void Start()
    {
        CharactersManager.Instance.OnCharacterSpawned += Instance_OnCharacterSpawned;
        CharactersManager.Instance.OnCharacterDespawned += Instance_OnCharacterDespawned;

        foreach (var item in CharactersManager.Instance.characters)
        {
            if (!labels.ContainsKey(item.Key))
            {
                Instance_OnCharacterSpawned(item.Key, item.Value);
            }
        }
    }

    private void Instance_OnCharacterDespawned(int id, Character arg2)
    {
        if (labels.ContainsKey(id))
        {
            Destroy(labels[id].gameObject);
            labels.Remove(id);
        }
    }

    private void Instance_OnCharacterSpawned(int id, Character arg2)
    {
        if (!labels.ContainsKey(id))
        {
            NicknameHoverLabel labelInst = Instantiate(hoverLabelPrefab, transform);
            labelInst.Fill(arg2);

            labels.Add(id, labelInst);
        }
    }
}
