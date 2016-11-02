using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class SpeedSetterChild : MonoBehaviour {

    public int num;
    public bool active = false;
    bool hovering = false;
    Image image;

    public void init()
    {
        image = GetComponent<Image>();
        loadImage(0);
    }
    
    public void OnDown(BaseEventData e)
    {
        active = true;
        loadImage(4);
        SpeedSetter.check(num);
    }

    public void OnUp(BaseEventData e)
    {
        loadImage(hovering ? 3 : 1);
    }
    public void OnEnter(BaseEventData e)
    {
        hovering = true;
        loadImage(active ? 3 : 2);
    }

    public void OnExit(BaseEventData e)
    {
        hovering = false;
        loadImage(active ? 1 : 0);
    }

    public void enable()
    {
        active = true;
        loadImage(1);
    }

    public void disable()
    {
        active = false;
        loadImage(0);
    }

    void loadImage(int num2)
    {
        image.sprite = Resources.Load<Sprite>(("UI/Pause Button/pause" + num) + num2);
        image.eventAlphaThreshold = .5f;
    }
}
