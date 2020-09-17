using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageLabelsController : MonoBehaviour
{
    [SerializeField]
    private ListHandler<DamageInfoLabel> labelsHandler;

    private void Start()
    {
        CombatManager.Instance.OnDamageReceived += Instance_OnDamageReceived;
    }

    private void Instance_OnDamageReceived(DamageInfo obj)
    {
        Character target = CharactersManager.Instance.GetPlayer(obj.targetId);
        if(target != null)
        {
            var label = labelsHandler.SpawnItem();
            label.Fill(obj);
            Vector3 pos = Camera.main.WorldToScreenPoint(target.transform.position);
            label.transform.position = pos;
            labelsHandler.Destroy(label, 0.65f);

            label.transform.DOMoveY(pos.y + 200, 0.5f);
            label.transform.DOPunchScale(Vector3.one, 0.2f);
        }
    }
}
