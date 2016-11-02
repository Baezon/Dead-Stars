using System;

//add a sequential mission
public abstract class Mission {

    protected Entity entity;
    public Mission parent;

    public bool proceed()
    {
        if (entity == null) { throw new System.Exception("mission has no entity"); }
        try
        {
            if (doProceed())
            {
                entity.mission = parent;
                return parent == null || parent.proceed();
            }
            return false;
        }
        catch (MissionFailedException)
        {
            entity.mission = null;
            throw;
        }
    }
    public abstract string ToString();

    protected abstract bool doProceed();

    public virtual Mission set(Entity _entity, Mission _parent)
    {
        entity = _entity;
        parent = _parent;
        return entity.mission = this;
    }
    public Mission set(Mission parent)
    {
        return set(parent.entity, parent);
    }
    public Mission set(Entity entity)
    {

        return set(entity, null);
    }
}

public class JumpMission : Mission
{
    CBody destination;

    public JumpMission(CBody _destination)
    {
        destination = _destination;
    }

    protected override bool doProceed()
    {
        if (entity.location == destination)
        {
            return true;
        }
        if (entity is BigEntity && ((BigEntity)entity).design.get(SubsystemType.Drive) == null)
        {
            throw new MissionFailedException("Fission Mailed, no jump drive");
        }
        if (entity.canJump(destination) && !(
            entity.status is ChargingJump && ((ChargingJump)entity.status).destination == destination || 
            entity.status is Jumping && ((Jumping)entity.status).destination == destination))
        {
            entity.setStatus(new ChargingJump(destination, entity.jumpChargeTime(destination)));
        }
        return false;
    }

    public override string ToString()
    {
        return "Jumping to " + destination;
    }

}

public class ExploreMission : Mission
{
    CBody target;
    bool exploreChildren;

    public ExploreMission(CBody _target, bool _exploreChildren)
    {
        target = _target;
        exploreChildren = _exploreChildren;
    }

    protected override bool doProceed()
    {
        if(!target.revealedBy(entity.empire))
        {
            if (entity.location != target)
            {
                return new JumpMission(target).set(this).proceed();
            }
            entity.setStatus(new Exploring(3600));
            return false;
        }
        if (exploreChildren)
        {
            foreach (CBody child in target.starChildren)
            {
                if (!child.deepRevealedBy(entity.empire))
                {
                    return new ExploreMission(child, true).set(this).proceed();
                }
            }
        }
        return true;
        
    }

    public override string ToString()
    {
        return "Exploring " + target;
    }
}

public class ImmigrateMission : Mission
{
    Planet target;
    CBody origin;

    public ImmigrateMission(Planet _target)
    {
        target = _target;
    }

    protected override bool doProceed()
    {
        if (((BigEntity)entity).design.passengerHold.currentCrew!=0)
        {
            if (entity.location != target)
            {
                return new JumpMission(target).set(this).proceed();
            }
            if (target.colony == null || target.colony.empire == entity.empire)
            {
                entity.setStatus(new UnloadingPassengers(86400));
                return false;
            }
            target = origin as Planet;
            return proceed();
        }
        return true;

    }

    public override Mission set(Entity _entity, Mission _parent)
    {
        origin = _entity.location;
        return base.set(_entity,_parent);
    }

    public override string ToString()
    {
        return target.colony != null ? "Unloading Passengers" : "Colonizing " + target;
    }
}

public class SequentialMission : Mission
{
    Mission[] missions;
    int i=-1;

    public SequentialMission(params Mission[] _missions)
    {
        missions = _missions;
    }

    protected override bool doProceed()
    {
        while(++i < missions.Length) {
            if (!missions[i].set(this).proceed())
            {
                return false;
            }
        }
        return true;
    }
    public override string ToString()
    {
        return missions[i].ToString();
    }
}

public class DieMission : Mission
{
    protected override bool doProceed()
    {
        entity.die();
        return true;
    }
    public override string ToString()
    {
        return "die";
    }
}

public class MissionFailedException : Exception 
{
    public MissionFailedException(string s) : base(s){}
}
