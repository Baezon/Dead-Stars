using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;
public class FleetInfo : MonoBehaviour
{

    SpriteRenderer window;
    public GameObject[] lineObj;
    Text name, location;
    bool open = true;

    public void Start()
    {
        Font bold = Resources.Load<Font>("Fonts/B");
        Font med = Resources.Load<Font>("Fonts/M");
        //                             name,          x pos,y pos,width,font, anchor
        name = makeTextField("Name", -99, -133, 180, bold, TextAnchor.UpperLeft);
        location = makeTextField("Location", 249, -133, 180, med, TextAnchor.UpperRight);

        setVisible(false);
    }

    public void Update()
    {
        ICollection<Entity> ships = GameCtrl.me.selected.entities;
        setVisible(ships.Count >= 2);
        if (ships.Count >= 2)
        {
            name.text = ships.Count + " Ships";
            List<CBody> locations = new List<CBody>();
            foreach (Entity ship in ships)
            {
                if (!locations.Contains(ship.location) && !(ship.status is Jumping))
                {
                    locations.Add(ship.location);
                }
            }
            if (locations.Count > 1)
            {
                location.text = locations.Count + " Locations";
            }
            else
            {
                location.text = locations[0].ToString();
            }
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
        if (open == vis) { return; }
        GetComponent<Image>().enabled = open = vis;
        foreach (Transform t in transform)
        {
            t.gameObject.SetActive(vis);
        }
    }
}
