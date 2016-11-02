using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;
public class ShipInfo : MonoBehaviour {

    SpriteRenderer window;
    public GameObject[] lineObj;
    Text name, location, topMission, bottomMission, shipClass,statusTime;
    bool open= true;

    public void Start() 
    {
        Font bold = Resources.Load<Font>("Fonts/B");
        Font med = Resources.Load<Font>("Fonts/M");
        //                             name,          x pos,y pos,width,font, anchor
        name          = makeTextField("Name",           -99, -133, 180,bold, TextAnchor.UpperLeft);
        location      = makeTextField("Location",       249, -133, 180, med, TextAnchor.UpperRight);
        topMission    = makeTextField("TopMission",    -149, -185, 240, med, TextAnchor.UpperLeft);
        bottomMission = makeTextField("BottomMission", -149, -205, 240, med, TextAnchor.UpperLeft);
        shipClass     = makeTextField("ShipClass",     -124, -159, 200, med, TextAnchor.UpperLeft);
        statusTime    = makeTextField("StatusTime",    -149, -225, 260, med, TextAnchor.UpperLeft);

        EventTrigger.TriggerEvent eventt = new EventTrigger.TriggerEvent();
        eventt.AddListener(locationWarp);
        location.gameObject.AddComponent<EventTrigger>().triggers = new List<EventTrigger.Entry> {
            new EventTrigger.Entry { eventID = EventTriggerType.PointerClick, callback = eventt } };

        setVisible(false);
    }

    public void Update()
    {
        Entity ship = GameCtrl.me.selected.entity;
        setVisible(ship!=null);
        if (ship != null)
        {
            ship = GameCtrl.me.selected.entity;
            name.text = ship.empire.settings.shipPrefix + ship.name;
            location.text = ship.status is Jumping ? "In Transit" : ship.location.name;
            topMission.text = ship.topMission != null ? ship.topMission.ToString() : "Idle";
            bottomMission.text = ship.topMission == ship.mission ? "" : ship.mission.ToString();
            shipClass.text = ship.empire.settings.adjectival + " " + ship.getClass().name();
            statusTime.text = ship.status != null && ship.status.endTime.HasValue ? 
                GameCtrl.me.timeString(ship.status.endTime.Value) : "";
        }
    }

    public void locationWarp(BaseEventData e)
    {
        Entity ship = GameCtrl.me.selected.entity;
        if (GameCtrl.me.selected.entities.Count == 1)
        {
            SystemView systemView = GameCtrl.me.systemView;
            if (systemView == null || systemView.system != ship.location.system)
            {
                systemView = GameCtrl.me.goToSystemView(ship.location.system);
                GameCtrl.me.focus = ship.location.system;
            }
            systemView.setFocus(systemView.dictionary[ship.location]);
        }
    }

    Text makeTextField(string name, float xPos, float yPos, float width, Font font, TextAnchor alignment)
    {
        GameObject obj = new GameObject(name, typeof(CanvasRenderer), typeof(Text));
        obj.transform.SetParent(transform);
        obj.layer = LayerMask.NameToLayer("UI");
        Text text = obj.GetComponent<Text>();
        text.font = font;
        text.fontSize = 16;
        RectTransform rectT = obj.GetComponent<RectTransform>();
        rectT.anchoredPosition = new Vector2(xPos, yPos);
        Vector2 displacement = rectT.sizeDelta = new Vector2(width, 20);
        switch (text.alignment = alignment)
        {
            case TextAnchor.UpperLeft: displacement.Scale(new Vector2(1, -1)); break;
            case TextAnchor.UpperRight: displacement.Scale(new Vector2(-1, -1)); break;
            default: throw new System.Exception();
        }
        rectT.anchoredPosition = new Vector2(xPos, yPos) + displacement / 2;
        text.text = "New Text";
        return text;
    }

    void setVisible(bool vis)
    {
        if(open == vis){return;}
        GetComponent<Image>().enabled = open = vis;
        foreach (Transform t in transform)
        {
            t.gameObject.SetActive(vis);
        }
    }
}
