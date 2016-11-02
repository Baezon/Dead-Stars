using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class SlidyButton : MonoBehaviour {

    public float slideOutPos;
    float time;
    bool hovering;
    Vector3 oldPos;
    Vector3 currentPos;

    void Start()
    {
        oldPos = transform.position;
        GetComponent<Image>().eventAlphaThreshold = .5f;
    }

	public void OnEnter(BaseEventData e)
    {
        hovering = true;
    }

    public void OnExit(BaseEventData e)
    {
        hovering = false;
    }

    public void Update()
    {
        if (hovering)
        {
            time += 6*Time.deltaTime;
            if (time > 1)
            {
                time = 1;
            }
            currentPos = oldPos + new Vector3(slideOutPos * time, 0, 0);
        }
        else 
        {
            time -= 6*Time.deltaTime;
            if (time < 0)
            {
                time = 0;
            }
            currentPos = oldPos + new Vector3(slideOutPos * time, 0, 0);
        }
        transform.position = currentPos;
    }
}
