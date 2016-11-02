using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class Skyball : MonoBehaviour {
    bool hover = false;
    EventTrigger currentTrigger;

    // Update is called once per frame
	void Update () {
        if (hover)
        {
            Vector3 wp = GameCtrl.me.cam.ScreenToWorldPoint(Input.mousePosition);
            Vector2 touchPos = new Vector2(wp.x, wp.y);
            Collider2D c2d = Physics2D.OverlapPoint(touchPos);
            EventTrigger trigger = c2d == null ? null : c2d.gameObject.GetComponent<EventTrigger>();
            if (currentTrigger != trigger)
            {
                if (currentTrigger != null)
                {
                    currentTrigger.OnPointerExit(null);
                }
                if (trigger != null)
                {
                    trigger.OnPointerEnter(null);
                }
                currentTrigger = trigger;
            }
            if (trigger == null)
            {
                GameCtrl.me.view.OnHover();
            }
        }
	}

	public void OnClick(BaseEventData e)
	{
		if (((PointerEventData)e).pointerId == -1)
		{
			GameCtrl.me.view.OnMouseDown();
		}
	}

    public void OnPointerEnter(BaseEventData e)
	{
		hover = true;
	}

     public void OnPointerExit(BaseEventData e)
	{
		hover = false;
        if (currentTrigger != null)
        {
            currentTrigger.OnPointerExit(null);
        }
        currentTrigger = null;
    }
}