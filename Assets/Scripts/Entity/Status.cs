using UnityEngine;
using System.Collections;


public class Status {
    private long _startTime;
    private long? _endTime;

    public virtual bool targetable
    {
        get{return true;}
    }

    public virtual long startTime
    {
        get { return _startTime; }
    }

    public virtual long? endTime
    {
        get { return _endTime; }
    }

    public virtual bool finish(Entity e)
    {
        e.status = null;
        return true;
    }

    public Status(int duration)
    {
        _startTime = GameCtrl.me.time;
        _endTime = GameCtrl.me.time + duration;
    }

    public Status()
    {
        _startTime = GameCtrl.me.time;
        _endTime = null;
    }

}

public static class StatusExtension
{
    public static bool isComplete(this Status status)
    {
        return status.endTime >= GameCtrl.me.time;
    }
}

public class Jumping : Status
{
    public CBody destination;

    public override bool targetable
    {
        get { return false; }
    }

    public Jumping(CBody _destination, int duration) : base(duration)
    {
        destination = _destination;
    }

    public override bool finish(Entity entity)
    {
        entity.location = destination;
        GameCtrl.me.view.invalidate();
        return base.finish(entity);
    }

    public override string ToString()
    {
        return "Jumping to " + destination;
    }
    

}

public class ChargingJump : Status
{
    public CBody destination;
    private long _startTime;
    private long _endTime;

    public override bool targetable
    {
        get { return true; }
    }

    public ChargingJump(CBody _destination, int duration) : base (duration)
    {
        destination = _destination;
    }

    public override bool finish(Entity entity)
    {
        entity.setStatus(new Jumping(destination, entity.travelTime(destination)));
        GameCtrl.me.view.invalidate();
        return false;
    }

    public override string ToString()
    {
        return "Charging Jump";
    }

}
public class Exploring : Status
{
    public Exploring(int duration) : base(duration){}


    public override bool finish(Entity entity)
    {
        entity.location.explore(entity.empire);
        GameCtrl.me.view.invalidate();
        return base.finish(entity);
    }


    public override string ToString()
    {
        return "Exploring";
    }

}

public class UnloadingPassengers : Status
{
    public UnloadingPassengers(int duration) : base(duration) { }


    public override bool finish(Entity entity)
    {
        Planet planet = entity.location as Planet;
        var passengerHold = ((BigEntity)entity).design.passengerHold;
        if (planet!= null && planet.colony != null)
        {
            planet.colony.population += passengerHold.currentCrew;
        }
        else
        {
            planet.colony = new Colony(passengerHold.currentCrew, planet, entity.empire);

        }
        passengerHold.currentCrew = 0;
        GameCtrl.me.view.invalidate();
        return base.finish(entity);
    }


    public override string ToString()
    {
        return "Colonizing";
    }

}
