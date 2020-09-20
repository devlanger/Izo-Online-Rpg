using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapController : MonoBehaviour
{
    [SerializeField]
    private RectTransform mapRect;

    [SerializeField]
    private RectTransform playerRect;

    [SerializeField]
    private Transform target;

    [SerializeField]
    private Text coordsText; 

    private IEnumerator Start()
    {
        PlayerController.Instance.OnLocalPlayerChanged += Instance_OnLocalPlayerChanged;
    
        while(true)
        {
            yield return new WaitForSeconds(0.25f);
            UpdateMap();
        }
    }

    private void Instance_OnLocalPlayerChanged(Character obj)
    {
        target = obj.transform;
    }

    private void UpdateMap()
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

        coordsText.text = string.Format("x: {0} y: {1}", Mathf.RoundToInt(target.position.x), Mathf.RoundToInt(target.position.z));
    }
}
