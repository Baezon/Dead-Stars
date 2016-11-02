using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;

public class Shpip : MonoBehaviour {

	SpriteRenderer sr;
    BoxCollider2D bc2d;
    BoxCollider bc;
    public IList<Entity> entities = new List<Entity>();
	FleetType fleetType = FleetType.None;
    GameObject dropdownMenu;
    bool isHovering=false;
    List<GameObject> dropdownItems = new List<GameObject>();
    Dictionary<CBodyObject, LineRenderer> travelLines1 = new Dictionary<CBodyObject, LineRenderer>();
    Dictionary<CBodyObject, LineRenderer> travelLines2 = new Dictionary<CBodyObject, LineRenderer>();

	public Shpip init() {
		sr = GetComponent<SpriteRenderer>();
        bc2d = GetComponent<BoxCollider2D>();
        bc = GetComponent<BoxCollider>();


        return this;
	}

	public void clearEntities()
	{
		entities.Clear();
		updateFleetType();
        foreach (LineRenderer line in travelLines1.Values)
        {
            Destroy(line);
        }
        foreach (LineRenderer line in travelLines2.Values)
        {
            Destroy(line);
        }
        travelLines1.Clear();
        travelLines2.Clear();
	}

	public static Shpip getShpip(Shpip goodGuys, Shpip badGuys, Shpip station, Entity entity)
	{
		Shpip s = (entity.getClass() == ShipClass.Station) ? station
			: entity.empire == GameCtrl.me.player ? goodGuys : badGuys;
        return s;
	}

	public void updateFleetType()
	{
		fleetType = FleetType.None;
		foreach (Entity entity in entities)
		{
			switch (entity.getClass())
			{
			case ShipClass.Station:
				fleetType = FleetType.BigStation;
				break;

			case ShipClass.Destroyer:
				switch (fleetType)
				{
                case FleetType.None:
                case FleetType.PassengerVessel:
                case FleetType.ExplorationVessel:
					fleetType = FleetType.Destroyer;
					break;

				case FleetType.CruiserBattleship:
					fleetType = FleetType.DestroyerBattleship;
					break;

				case FleetType.CruiserCarrier:
					fleetType = FleetType.DestroyerCarrier;
					break;

				case FleetType.Battleship:
					fleetType = FleetType.DestroyerBattleship;
					break;

				case FleetType.Carrier:
					fleetType = FleetType.DestroyerCarrier;
					break;

				case FleetType.Cruiser:
					fleetType = FleetType.DestroyerCruiser;
					break;
				}
				break;


			case ShipClass.Cruiser:
				switch (fleetType)
				{
                case FleetType.None:
                case FleetType.PassengerVessel:
                case FleetType.ExplorationVessel:
					fleetType = FleetType.Cruiser;
					break;

				case FleetType.Destroyer:
					fleetType = FleetType.DestroyerCruiser;
					break;

				case FleetType.Battleship:
					fleetType = FleetType.CruiserBattleship;
					break;

				case FleetType.Carrier:
					fleetType = FleetType.CruiserCarrier;
					break;
				}
				break;

			case ShipClass.Battleship:
				switch (fleetType)
				{
                case FleetType.None:
                case FleetType.PassengerVessel:
                case FleetType.ExplorationVessel:
					fleetType = FleetType.Battleship;
					break;
				
				case FleetType.Destroyer:
					fleetType = FleetType.DestroyerBattleship;
					break;

				case FleetType.Cruiser:
					fleetType = FleetType.CruiserBattleship;
					break;

				case FleetType.DestroyerCruiser:
					fleetType = FleetType.DestroyerBattleship;
					break;
				}
				break;

			case ShipClass.Carrier:
				switch (fleetType)
				{
                case FleetType.None:
                case FleetType.PassengerVessel:
                case FleetType.ExplorationVessel:
                    fleetType = FleetType.Carrier;
					break;

				case FleetType.Cruiser:
					fleetType = FleetType.CruiserCarrier;
					break;

				case FleetType.Destroyer:
					fleetType = FleetType.DestroyerCarrier;
					break;
				
				case FleetType.Battleship:
					fleetType = FleetType.Carrier;
					break;

				case FleetType.CruiserBattleship:
					fleetType = FleetType.CruiserCarrier;
					break;

				case FleetType.DestroyerCruiser:
					fleetType = FleetType.DestroyerCarrier;
					break;

				case FleetType.DestroyerBattleship:
					fleetType = FleetType.DestroyerCarrier;
					break;
				}
				break;

            case ShipClass.ExplorationVessel:
                switch (fleetType)
                {
                    case FleetType.None:
                    case FleetType.PassengerVessel:
                        fleetType = FleetType.ExplorationVessel;
                        break;
                }
                break;

            case ShipClass.PassengerVessel:
                switch (fleetType)
                {
                    case FleetType.None:
                        fleetType = FleetType.PassengerVessel;
                        break;
                }
                break;

			default:
			throw new System.Exception("what the hell is this fleet and why am i breaking when i try to put a shpip to it");
			}

		}
		if (sr.enabled = fleetType != FleetType.None)
		{
			sr.sprite = Resources.Load<Sprite>("Entity icons/"+fleetType.name());
		}
        if (bc != null) bc.enabled = sr.enabled;
        if (bc2d != null) bc2d.enabled = sr.enabled;
    }

    public void OnPointerEnter(BaseEventData e)
    {
        isHovering = true;
        if (GameCtrl.me.systemView != null)
        {
            GameCtrl.me.systemView.CBodyHover(null);
        }
        if (dropdownMenu == null)
        {
            dropdownMenu = new GameObject("dropdownMenu");
            dropdownMenu.transform.parent = transform;
            dropdownMenu.transform.rotation = transform.parent.rotation;
            dropdownMenu.transform.localScale = Vector3.one * .08f;
            dropdownMenu.transform.localPosition = Vector3.zero;
            int i = 0;
            foreach (Entity entity in entities)
            {
                GameObject dropdownItem = Instantiate(GameCtrl.me.prefabs.selectionDropdownItem);
                SelectionDropdownItem item = dropdownItem.GetComponent<SelectionDropdownItem>();
                item.entity = entity;
                dropdownItem.transform.SetParent(dropdownMenu.transform, false);
                dropdownItem.transform.localPosition = new Vector3(-4.5f, -3.2f * i++, -1);
                dropdownItem.GetComponent<TextMesh>().text = entity.name;
                dropdownItems.Add(dropdownItem);
            }
            int pos = 1;
            foreach (GameObject item in dropdownItems)
            {
                SpriteRenderer itemsr = item.GetComponentInChildren<SpriteRenderer>();
                int count = dropdownItems.Count;
                string image; 
                if(count == 1)
                {
                    image = "box";
                }
                else if(pos == 1)
                {
                    image = "boxtop";
                }
                else if (pos == count)
                {
                    image = "boxbot";
                }
                else
                {
                    image = "boxmid";
                }
                item.GetComponent<SelectionDropdownItem>().init(pos++, image);
            }
        }
    }

    public void OnPointerExit(BaseEventData e)
    {
        isHovering = false;
    }

    void Update()
    {
        int i = 0;
        if (isHovering)
        {
            i++;
        }
        foreach (GameObject item in dropdownItems)
        {
            if (item.GetComponent<SelectionDropdownItem>().isHovering)
            {
                i++;
                break;
            }
        }
        if (i == 0)
        {
            Destroy(dropdownMenu);
            dropdownMenu = null;
            dropdownItems.Clear();
        }

       
    }

    public void addTravelLine(CBodyObject cBodyObject)
    {
        if (travelLines1.ContainsKey(cBodyObject)) return;
        GameObject travelLine1 = new GameObject("travelLine", typeof(LineRenderer));
        GameObject travelLine2 = new GameObject("travelLine", typeof(LineRenderer));
        LineRenderer line1 = travelLine1.GetComponent<LineRenderer>();
        LineRenderer line2 = travelLine2.GetComponent<LineRenderer>();
        line1.useWorldSpace = line2.useWorldSpace = true;
        line1.material.shader = line2.material.shader = Shader.Find("Unlit/Color");
        line1.material.color = new Color(.25f, .25f, .25f);
        line2.material.color = new Color(.15f, .15f, .15f);
        travelLine1.transform.parent = travelLine2.transform.parent = GameCtrl.me.view.transform;
        line1.SetVertexCount(2);
        line2.SetVertexCount(2);
        travelLines1.Add(cBodyObject, line1);
        travelLines2.Add(cBodyObject, line2);
        updateTravelLine(cBodyObject);
    }

    public void updateTravelLines()
    {
        foreach (var cBodyObj in travelLines1.Keys)
        {
            updateTravelLine(cBodyObj);
        }
    }

    public void updateTravelLine(CBodyObject cBodyObj)
    {
        float wScreenHeightUnits = GameCtrl.me.cam.orthographicSize * 2f;
        float scale = wScreenHeightUnits / Screen.height;
        float travelFrac = entities.Select(e => e.travelFrac).Max();
        LineRenderer line1 = travelLines1[cBodyObj];
        LineRenderer line2 = travelLines2[cBodyObj];
        Vector3 mid = Vector3.Lerp(transform.position, cBodyObj.transform.position, travelFrac);
        float scalemid = Mathf.Lerp(9 * scale, 3 * scale, travelFrac);
        line1.SetPosition(0, transform.position);
        line1.SetPosition(1, mid);
        line2.SetPosition(0, mid);
        line2.SetPosition(1, cBodyObj.transform.position);
        line1.SetWidth(9 * scale, scalemid); 
        line2.SetWidth(scalemid, 3 * scale);
    }
}