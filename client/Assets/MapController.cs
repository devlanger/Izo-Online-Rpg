using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    [SerializeField]
    private RectTransform mapRect;

    [SerializeField]
    private RectTransform playerRect;

    [SerializeField]
    private Transform target;

    private void Start()
    {
        PlayerController.Instance.OnLocalPlayerChanged += Instance_OnLocalPlayerChanged;
    }

    private void Instance_OnLocalPlayerChanged(Character obj)
    {
        target = obj.transform;
    }

    private void Update()
    {
        if(target == null)
        {
            return;
        }

        Vector2 mapSize = new Vector2(1000, 1000);
        Vector2 mapRectSize = mapRect.sizeDelta;

        Vector2 delta = mapRectSize / mapSize;
        Vector2 targetPos = new Vector2(-target.position.x, -target.position.z);

        Vector2 finalOffset = targetPos * delta;

        mapRect.anchoredPosition = finalOffset;
    }
}
