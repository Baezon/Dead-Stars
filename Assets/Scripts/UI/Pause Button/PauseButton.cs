using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class PauseButton : MonoBehaviour {


    public void Start()
    {
        GetComponent<Image>().eventAlphaThreshold = .5f;
    }

	public void OnPauseClick(BaseEventData e)
	{
        GameCtrl.me.timeCtrl.paused ^= true;
	}

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameCtrl.me.timeCtrl.paused ^= true;
        }
    }


}
