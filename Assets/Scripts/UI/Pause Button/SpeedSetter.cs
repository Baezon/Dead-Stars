using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpeedSetter : MonoBehaviour
{
    
    static int[] SPEED_FACTORS = { 60, 180, 600, 1800, 7200, 28800, 86400, 604800 };
    static IList<GameObject> children = new List<GameObject>();

    void Start()
    {
        for (int i=0;i<SPEED_FACTORS.Length; i++)
        {
            GameObject child = Instantiate(GameCtrl.me.prefabs.speedSetterChild);
            child.transform.SetParent(transform,false);
            child.GetComponent<SpeedSetterChild>().num = i + 1;
            children.Add(child);
            child.GetComponent<SpeedSetterChild>().init();
            if (i == 0)
            {
                child.GetComponent<SpeedSetterChild>().enable();
            }
        }
        GameCtrl.me.speedFactor = SPEED_FACTORS[0];
    }

    public static void check(int callerNum)
    {
        GameCtrl.me.speedFactor = SPEED_FACTORS[callerNum - 1];

        foreach (GameObject child in children)
        {
            if (child.GetComponent<SpeedSetterChild>().num > callerNum)
            {
                child.GetComponent<SpeedSetterChild>().disable();
            }
            else if (child.GetComponent<SpeedSetterChild>().num < callerNum)
            {
                child.GetComponent<SpeedSetterChild>().enable();
            }
        }
        GameCtrl.me.timeCtrl.paused = false;
    }
}