using UnityEngine;
using System.Collections;

public class Prefabs {

    public GameObject starTemplate;
    public GameObject cBodyTemplate;
    public GameObject orbitalLine;
    public GameObject travelLine;
    public GameObject travelShpip;
    public GameObject moobTemplate;
    public GameObject selectionDropdownItem;
    public GameObject speedSetterChild;
    public GameObject text;
    public GameObject[] box;

    public Prefabs()
    {
        starTemplate = Resources.Load<GameObject>("Prefabs/StarSystem");
        cBodyTemplate = Resources.Load<GameObject>("Prefabs/CBody");
        moobTemplate = Resources.Load<GameObject>("Prefabs/Moob");
        selectionDropdownItem = Resources.Load<GameObject>("Prefabs/SelectionDropdownItem");
        speedSetterChild = Resources.Load<GameObject>("Prefabs/SpeedSetterChild");
        text = Resources.Load<GameObject>("Prefabs/text");
        box = new GameObject[] {Resources.Load<GameObject>("Prefabs/background"),Resources.Load<GameObject>("Prefabs/edge")};
        
        travelShpip = new GameObject("travelShpip", typeof(MeshFilter), typeof (MeshRenderer));
        travelShpip.GetComponent<Renderer>().material.shader = Shader.Find("Unlit/Color");
        travelShpip.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f);
    }
}
