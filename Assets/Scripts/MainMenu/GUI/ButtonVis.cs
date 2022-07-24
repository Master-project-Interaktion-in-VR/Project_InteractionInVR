using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonVis : MonoBehaviour
{

    [SerializeField]
    private Color normal;

    [SerializeField]
    private Color hover;

    [SerializeField]
    private Color pressed;

    [SerializeField]
    private Color disabled;

    private EventTrigger _trigger;

    void Awake()
    {
        _trigger = GetComponent<EventTrigger>();

        SubscribeEvent(EventTriggerType.PointerEnter, OnHover);
        SubscribeEvent(EventTriggerType.PointerExit, OnUnhover);
        SubscribeEvent(EventTriggerType.PointerDown, OnDown);
        SubscribeEvent(EventTriggerType.PointerUp, OnUp);
    }

    private void SubscribeEvent(EventTriggerType eventType, Action<PointerEventData> callbackFunction)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = eventType;
        entry.callback.AddListener((data) => { callbackFunction((PointerEventData)data); });
        if(_trigger != null)
            _trigger.triggers.Add(entry);
    }

    public void OnHover(PointerEventData data)
    {
        gameObject.GetComponent<Image>().color = hover;
        gameObject.GetComponentInChildren<TMPro.TextMeshProUGUI>().color = hover;
    }

    public void OnUnhover(PointerEventData data)
    {
        gameObject.GetComponent<Image>().color = normal;
        gameObject.GetComponentInChildren<TMPro.TextMeshProUGUI>().color = normal;
    }

    public void OnDown(PointerEventData data)
    {
        gameObject.GetComponent<Image>().color = pressed;
        gameObject.GetComponentInChildren<TMPro.TextMeshProUGUI>().color = pressed;
    }
    public void OnDown()
    {
        gameObject.GetComponent<Image>().color = pressed;
        gameObject.GetComponentInChildren<TMPro.TextMeshProUGUI>().color = pressed;
    }

    public void OnUp(PointerEventData data)
    {
        gameObject.GetComponent<Image>().color = normal;
        gameObject.GetComponentInChildren<TMPro.TextMeshProUGUI>().color = normal;
    }
    public void OnUp()
    {
        gameObject.GetComponent<Image>().color = normal;
        gameObject.GetComponentInChildren<TMPro.TextMeshProUGUI>().color = normal;
    }
}
