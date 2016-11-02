using UnityEngine;
using System.Collections.Generic;
using Hjson;

public class GameCtrl : MonoBehaviour {

	//VERSION 0.21

	public IList<StarSystem> systems = new List<StarSystem>();
	public const int MAX_SYSTEMS = 100;
	public static GameCtrl me;
    public GameObject canvas;
	public Camera cam;
    public View view;
	public StarSystem focus;
	public float aroundAngle;
	public float upAngle;
    public float maxDistance = GalaxyView.DEFAULT_DISTANCE;
	[HideInInspector] public TimeCtrl timeCtrl;
	[HideInInspector] public int speedFactor;
	public Font gameFont;
	[HideInInspector] public Dictionary<string,Empire> empires = new Dictionary<string,Empire>();
	public Selection selected = new Selection();
	public Empire player;
    public Prefabs prefabs;
    public Skyball skyball;
    public PauseButton pauseButton;
    public GameSettings gameSettings;
   /* public IList<CBody> anyCBodies = new List<CBody>();
    public IList<CBody> habCBodies7 = new List<CBody>();
    public IList<CBody> habCBodies9 = new List<CBody>();
    public IList<CBody> habCBodies11 = new List<CBody>();*/
    bool console = false;

    public long time
    {
        get { return timeCtrl.time; }
    }

    public GalaxyView galaxyView
    {
        get
        {
            return view is GalaxyView ? (GalaxyView)view : null;
        }
        set
        {
            view = value;
        }

    }

    public SystemView systemView
    {
        get
        {
            return view is SystemView ? (SystemView)view : null;
        }
        set
        {
            view = value;
        }

    }


	// Use this for initialization
	void Start () {
		me = this;

		GameObject temp = new GameObject("GalaxyView");
		temp.transform.parent = transform;
		galaxyView = temp.AddComponent<GalaxyView>();
        pauseButton = FindObjectOfType<PauseButton>();

        prefabs = new Prefabs();
        timeCtrl = new TimeCtrl();
        gameSettings = new GameSettings();
        gameSettings.loadSettings();

        genSystems(MAX_SYSTEMS);
        int i = 0;
        foreach (JsonValue empire in gameSettings.empireList)
        {
            Empire e = new Empire(empire, systems[i++]);
            empires.Add(empire,e);
            player = e;
        }
        foreach (StarSystem system in systems)
        {
            system.NameNSort();
        }
        focus = player.capitol.planet.system;
        galaxyView.init();
       // cBodiesCalc();

	}

  

	public SystemView goToSystemView (StarSystem system) {
        if (galaxyView != null)
        {
            Destroy(galaxyView.gameObject);
            galaxyView = null;
            cam.orthographic = true;
        }
        else { Destroy(systemView.gameObject); }
        GameObject temp = new GameObject("SystemView");
        temp.transform.parent = transform;
        systemView = temp.AddComponent<SystemView>();
		systemView.initialize(system);
        return systemView;
	}

	public void goToGalaxyView () {
		Destroy(systemView.gameObject);
		systemView = null;
		cam.orthographic=false;
		GameObject temp = new GameObject("GalaxyView");
		temp.transform.parent = transform;
		galaxyView = temp.AddComponent<GalaxyView>();
        galaxyView.init();
		galaxyView.startOutAnimation();
	}

	void Update () {
		if (!timeCtrl.paused)
		{
			timeCtrl.tick(speedFactor * Time.deltaTime);
		}
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            if (console)
            {
                console = false;
            }
            else
            {
                console = true;
                timeCtrl.paused = true;
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            factorial(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            factorial(2);
        } 
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            factorial(3);
        } 
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            factorial(4);
        } 
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            factorial(5);
        } 
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            factorial(6);
        } 
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            factorial(7);
        } 
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            factorial(8);
        } 
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            factorial(9);
        }
        
        
	}

	void OnGUI () {
		GUI.skin.font = gameFont;
		GUI.Label(new Rect(135,Screen.height-22,400,20), timeString(timeCtrl.time));
        timeCtrl.OnGUI();
	}

    public string timeString(long time)
    {
        long temp = time;
        int second = (int)(temp % 60);
        temp /= 60;
        int minute = (int)(temp % 60);
        temp /= 60;
        int hour = (int)(temp % 24);
        temp /= 24;
        int day = (int)(temp % 30) + 1;
        temp /= 30;
        int month = (int)(temp % 12) + 1;
        temp /= 12;
        int year = (int)(temp) + 1;
        return 
            string.Format("{0:0000}", year) + "-" +
            string.Format("{0:00}", month) + "-" +
            string.Format("{0:00}", day) + " " +
            string.Format("{0:00}", hour) + ":" +
            string.Format("{0:00}", minute) + ":" +
            string.Format("{0:00}", second);
    }

    void genSystems (int maxSystems)
    {
        JsonValue firstHalfList = gameSettings.firstHalfList;
        JsonValue secondHalfList = gameSettings.secondHalfList;
        JsonArray aloneList = gameSettings.aloneList;
        IList<string> takenNames = new List<string>();
        for (int i = 0; i < maxSystems; i++)
        {
            string name;
            if (Random.value > .85f)
            {
                do {
                    string firstHalf = firstHalfList[Random.Range(0, firstHalfList.Count)];
                    string secondHalf = secondHalfList[Random.Range(0, secondHalfList.Count)];
                    name = firstHalf + " " + secondHalf;
                }
                while (takenNames.Contains(name));
            }
            else
            {
                do {
                    name = aloneList[Random.Range(0, aloneList.Count)]; 
                }
                while (takenNames.Contains(name));
            }
            Vector3 pos = 100 * Random.insideUnitSphere;
            StarSystem system = new StarSystem(pos, name);
            systems.Add(system);
            takenNames.Add(name);
        }
    }
    
    /*void cBodiesCalc()
    {
        foreach (StarSystem system in systems)
        {
            foreach (CBody cbody in system.deepCBodies) 
            {
                if (!(cbody is Star))
                {
                    anyCBodies.Add(cbody);
                    if (((Planet)cbody).habitability > 7)
                    {
                        habCBodies7.Add(cbody);
                    }
                    if ( ((Planet)cbody).habitability > 9)
                    {
                        habCBodies9.Add(cbody);
                    }
                    if (((Planet)cbody).habitability > 11)
                    {
                        habCBodies11.Add(cbody);
                    }
                }
            } 
        }
    }*/
}