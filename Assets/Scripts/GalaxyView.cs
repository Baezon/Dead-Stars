using UnityEngine;
using System.Collections.Generic;

public class GalaxyView : View
{
	public StarObject focus;
	float distance = DEFAULT_DISTANCE;
	float animationDirection = 1;
	Vector3 track;
	const float MIN_DISTANCE = 3.5f;
	const float ENTER_EXIT_SPEED = .90f;
	float lastClicked;
    public const float DEFAULT_DISTANCE = 10f;
    const float DISTANCE_BOUND = 200f;
    Dictionary<StarSystem, StarObject> dictionary;



	public void init () {
		dictionary = new Dictionary<StarSystem, StarObject>();
        foreach (StarSystem system in GameCtrl.me.systems) {
			GameObject obj = Instantiate(GameCtrl.me.prefabs.starTemplate);
			obj.transform.parent = transform;
			StarObject star = obj.GetComponent<StarObject>();
			star.Initialize(system);
			dictionary.Add(system, star);
			if (GameCtrl.me.focus == star.system) {
				focus = star;
			}

		}

		GameCtrl.me.focus = focus.system;
		track = focus.transform.position;
		if (GameCtrl.me.maxDistance <= MIN_DISTANCE) {
            GameCtrl.me.maxDistance = DEFAULT_DISTANCE;
		}
		invalidate();
	}


	public void startOutAnimation () {
		distance = MIN_DISTANCE;
		animationDirection = 1/ENTER_EXIT_SPEED;
	}


    public override void OnHover() { }

	public override void OnDrag()
	{
		if (animationDirection == 1)
		{
			GameCtrl.me.upAngle += 3 * Input.GetAxis("Mouse Y");
			GameCtrl.me.aroundAngle += 3 * Input.GetAxis("Mouse X");
			if (GameCtrl.me.upAngle > 89.9f)
			{
				GameCtrl.me.upAngle = 89.9f;
			}
			if (GameCtrl.me.upAngle < -89.9f)
			{
				GameCtrl.me.upAngle = -89.9f;
			}
		}

	}
	// Update is called once per frame
	public override void Update () {
        base.Update();
		track += (focus.transform.position-track)*.08f;
		GameCtrl.me.cam.transform.position 
			= Quaternion.Euler(GameCtrl.me.upAngle, GameCtrl.me.aroundAngle, 0) 
			* new Vector3(0,0,distance)
			+ track;
		GameCtrl.me.cam.transform.LookAt (track);
		distance *= animationDirection;
		if (distance < MIN_DISTANCE) {
			GameCtrl.me.goToSystemView(focus.system);
		}
		else if (distance > GameCtrl.me.maxDistance) {
			distance = GameCtrl.me.maxDistance;
			animationDirection = 1;
		}
		if(animationDirection==1){
			GameCtrl.me.maxDistance*=Mathf.Exp(-2*Input.GetAxis("Mouse ScrollWheel"));
            if (GameCtrl.me.maxDistance > DISTANCE_BOUND)
			{
                GameCtrl.me.maxDistance = DISTANCE_BOUND;
            }
			distance = GameCtrl.me.maxDistance;
		}
		

	}
	
	protected override void updateObjects()
	{
        foreach (StarObject starObj in dictionary.Values)
        {
            starObj.goodGuys.clearEntities();
            starObj.badGuys.clearEntities();
            starObj.station.clearEntities();
        }
		foreach (Empire empire in GameCtrl.me.empires.Values)
		{
			foreach (Entity entity in empire.entities)
			{
                StarSystem src = entity.location.system;
				StarSystem dest = src;
                if (entity.status is Jumping)
                {
                    dest = ((Jumping)entity.status).destination.system;
                }
				if (dest != src)
				{
					
				}
				StarObject o = dictionary[src];
                Shpip s = Shpip.getShpip(o.goodGuys, o.badGuys, o.station, entity);
                s.entities.Add(entity);
                s.updateFleetType();
			}
		}
        foreach (StarSystem system in GameCtrl.me.systems)
        {
            dictionary[system].updateIndicator();
        }
	}

	public void startGoToSystemView () {
		animationDirection = ENTER_EXIT_SPEED;
	}

	public void setFocus (StarObject star) {
		if (star == focus) {
			startGoToSystemView();
		}
		else {
			focus = star;
			GameCtrl.me.focus = focus.system;
		}
	}
	//Voronoi shit vv
	public override void OnMouseDown () {
		Vector3 mousePosition = Input.mousePosition;
		float minDistance=Mathf.Infinity;
		Transform target=null;
		foreach (Transform child in transform){
			Vector3 screenPos = GameCtrl.me.cam.WorldToScreenPoint(child.position);
			if(screenPos.z>=0.01f){
				screenPos.z=0;
				Vector3 displacement=mousePosition-screenPos;
				float magnitude = displacement.sqrMagnitude;
				if(magnitude<minDistance){
					target=child;
					minDistance=magnitude;
				}
			}
		}
		StarObject thingy= target.gameObject.GetComponent<StarObject>();
		if (Time.time - lastClicked < .4f){
			setFocus(thingy);
		}
		lastClicked = Time.time;
	}
	//Voronoi shit ^^

	void OnGUI()
	{
        float y = 10f;
       /* GUI.Label(new Rect(60, y += 20, 400, 26), string.Format("{0} total planets", GameCtrl.me.anyCBodies.Count));
        GUI.Label(new Rect(60, y += 20, 400, 26), string.Format("{0} planets greater than 7", GameCtrl.me.habCBodies7.Count));
        GUI.Label(new Rect(60, y += 20, 400, 26), string.Format("{0} planets greater than 9", GameCtrl.me.habCBodies9.Count));
        GUI.Label(new Rect(60, y += 20, 400, 26), string.Format("{0} planets greater than 11", GameCtrl.me.habCBodies11.Count));*/
	}

    
}
