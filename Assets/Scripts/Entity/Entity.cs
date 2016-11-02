using UnityEngine;
using System.Collections;

public abstract class Entity {

    public string name;
    public CBody location;
    public Empire empire;
	public bool military;
    public Status status= null;
    public Mission mission;
    public Mission topMission
    {
        get
        {
            Mission m = mission;
            if (m == null)
            {
                return null;
            }
            while (m.parent != null) { m = m.parent; }
            return m;
        }
    }

	public Entity(string _name, CBody _location, Empire _empire, bool _military)
	{
        name = _name;
		location = _location;
		empire = _empire;
		military = _military;
        empire.entities.Add(this);
        mission = null;
        GameCtrl.me.view.invalidate();
	}
	
	public abstract ShipClass getClass();

	public abstract float maxHP
	{
		get;	
	}
	public abstract float currentHP
	{
		get;
	}

    public abstract bool canJump(CBody destination);

    public void setStatus(Status status)
    {
        this.status = status;
        if(status!=null&&status.endTime.HasValue){
            GameCtrl.me.timeCtrl.addEvent(new Event(status.endTime.Value, () =>
            {
                if (status.finish(this) && mission != null)
                {
                    mission.proceed();
                }
            }));
        }
    }

    public abstract int jumpChargeTime(CBody destination);

    public abstract int travelTime(CBody destination);

    public virtual float travelFrac 
    { 
        get { return status is Jumping? 
            (float)(GameCtrl.me.time - status.startTime)/
            (status.endTime.Value - status.startTime) : 0f; } 
    }

    public void die()
    {
        GameCtrl.me.view.invalidate();
        empire.entities.Remove(this);
        GameCtrl.me.selected.Remove(this);

    }

    public string ToString()
    {
        return name;
    }
}