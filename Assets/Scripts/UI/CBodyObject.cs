using UnityEngine;
using System.Collections.Generic;

public class CBodyObject : MonoBehaviour {

	public CBody cBody;
	public float angularVelocity;
	LineRenderer line;
	const int ORBITAL_LINE_VERTICES = 1000;
    public CBodyInfo cBodyInfo;
	SpriteRenderer sr;
	public GameObject fleetObject;
	IList<SpriteRenderer> moonPips;

	public Shpip goodGuys, badGuys, station;

    Color defaultGrey = new Color(.6f, .6f, .6f);

	static Vector3[] PIP_POSITIONS = new Vector3[]{
		new Vector3(-.20f,-.13f,0),
		new Vector3(-.12f,-.22f,0),
		new Vector3(0,-.22f,0),
		new Vector3(.12f,-.22f,0),
		new Vector3(.20f,-.13f,0)};


	// Use this for initialization
	public void Initialize (CBody _cBody, Dictionary<CBody,CBodyObject> dictionary) {
		cBody = _cBody;
		goodGuys = transform.Find("GoodGuys").GetComponent<Shpip>().init();
		badGuys = transform.Find("BadGuys").GetComponent<Shpip>().init();
		station = transform.Find("Station").GetComponent<Shpip>().init();
        cBodyInfo = transform.Find("CBodyInfo").GetComponent<CBodyInfo>().init(cBody);

		//Initializing moon pips
        IList<Empire> colonizedMoons = new List<Empire>();
        foreach (CBody child in cBody.children)
        {
            if (((Planet)child).colony != null)
            {
                colonizedMoons.Add(((Planet)child).colony.empire);
            }
        }
		moonPips = new List<SpriteRenderer>();
		int i = 0;
		foreach (CBody child in cBody.children) {
			GameObject obj = Instantiate(GameCtrl.me.prefabs.cBodyTemplate);
			obj.transform.parent = transform;
			CBodyObject cBodyObj = obj.GetComponent<CBodyObject>();
			dictionary[child] = cBodyObj;
			cBodyObj.Initialize(child,dictionary);
			if(i<5){
                GameObject pip = Instantiate(GameCtrl.me.prefabs.moobTemplate);
				pip.transform.parent = transform;
				SpriteRenderer pipsr= pip.GetComponent<SpriteRenderer>();
				moonPips.Add(pipsr);
                if (i < colonizedMoons.Count)
                {
                    pipsr.color = colonizedMoons[i].settings.color;
                }
                else
                {
                    pipsr.color = defaultGrey;
                }
				pipsr.transform.localPosition = PIP_POSITIONS[i++];
            }
        }

		transform.rotation=Quaternion.Euler(0,0,0);
		sr = GetComponent<SpriteRenderer>();
		name = cBody.name;
		sr.sprite = Resources.Load<Sprite>("CBody icons/"+cBody.getPlanetSprite());
		if (cBody.aphelion > 0) {
			angularVelocity = cBody.angularVelocity(cBody.parent.mass);
		}

		//Initializing orbital lines
        GameObject orbitalLine = new GameObject("orbitalLine", typeof(LineRenderer));
        orbitalLine.GetComponent<Renderer>().material.shader = Shader.Find("Unlit/Color");
        orbitalLine.GetComponent<Renderer>().material.color = new Color(.4f, .4f, .4f);
        line = orbitalLine.GetComponent<LineRenderer>();
        orbitalLine.transform.parent = transform.parent;
		line.SetWidth(0,0);
		line.SetVertexCount(ORBITAL_LINE_VERTICES+2);
		line.useWorldSpace = false;
        float radius = 100f * cBody.aphelion;
        Vector3 v = radius * Vector3.left;
		Quaternion q=Quaternion.Euler(0,0,360f/ORBITAL_LINE_VERTICES);
		for (int j = 0;j <= ORBITAL_LINE_VERTICES+1;j++){
			line.SetPosition(j,v);
			v = q*v;
		}


	}

    // shh, dont tell anyone, i get called by systemview's update, 
    // so that my script still gets called here even though this gameobject might be disabled
    public void fakeUpdate()
    {
        if (GameCtrl.me.systemView == null)
        {
            return;
        }
        Vector3 oldPos = transform.position;
        float radius = 100f * cBody.aphelion;
        transform.position = transform.parent.position + new Vector3(
            radius * Mathf.Cos(cBody.angle),
            radius * Mathf.Sin(cBody.angle), 0);
        if (GameCtrl.me.systemView.focus == this)
        {
            GameCtrl.me.cam.transform.position += transform.position - oldPos;
        }

        float wScreenHeightUnits = GameCtrl.me.cam.orthographicSize * 2f;
        float scale = wScreenHeightUnits / Screen.height;
        bool active = cBody.visible(scale);
        gameObject.SetActive(active);
        line.enabled = active;

        if (active)
        {
            Vector3 spriteScale = Vector3.one * 100*scale;
            transform.localScale = spriteScale / transform.parent.localScale.x;
            line.SetWidth(scale, scale);
            line.transform.localScale = Vector3.one / transform.parent.localScale.x;
            line.transform.localPosition = Vector3.zero;
        }
        if (moonPips.Count > 0)
        {
            int pips = 0;
            foreach (Transform child in transform)
            {
                if (child.GetComponent<CBodyObject>() != null && !child.gameObject.activeSelf && child.GetComponent<CBodyObject>().cBody.parent.revealed)
                {
                    pips++;
                }
            }
            for (int i = 0; i < moonPips.Count; i++)
            {
                moonPips[i].enabled = i < pips;
            }
        }
    }

    public void updateColor()
    {
        sr.material.color = (cBody is Planet && ((Planet)cBody).colony != null) ? ((Planet)cBody).colony.empire.settings.color : defaultGrey;
    }
}