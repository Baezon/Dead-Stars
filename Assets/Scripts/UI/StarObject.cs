using UnityEngine;
using System.Collections;
using System.Linq;

public class StarObject : MonoBehaviour
{

	public StarSystem system;
	public GameObject text;
    public GameObject indicator;

	public Shpip goodGuys, badGuys, station;

	// Use this for initialization
	public void Initialize(StarSystem _system)
	{
		system = _system;
		transform.position = system.pos;
		transform.localScale *= .03f;

		name = text.GetComponent<TextMesh>().text = system.name;

		goodGuys = transform.Find("GoodGuys").GetComponent<Shpip>().init();
		badGuys = transform.Find("BadGuys").GetComponent<Shpip>().init();
		station = transform.Find("Station").GetComponent<Shpip>().init();

        updateIndicator();
	}

	// Update is called once per frame
	void Update()
	{
		transform.rotation = GameCtrl.me.cam.transform.rotation;
	}

    public void updateIndicator()
    {
        Colony colony = null;
        foreach (Planet planet in system.deepCBodies.OfType<Planet>())
        {
            if (planet.colony != null)
            {
                if (planet.colony == planet.colony.empire.capitol)
                {
                    colony = planet.colony;
                    break;
                }
                else { colony = planet.colony; }
            }
        }
        if (colony != null)
        {
            var empireSettings = colony.empire.settings;
            indicator.GetComponent<MeshFilter>().mesh =
                Resources.Load<Mesh>("World icons/" +
                empireSettings.prefix + (colony == colony.empire.capitol ? "capitol" : "world"));
            indicator.GetComponent<MeshRenderer>().material.color = empireSettings.color;
        }
        else
        {
            indicator.GetComponent<MeshFilter>().mesh = Resources.Load<Mesh>("World icons/world");
        }
    }
}