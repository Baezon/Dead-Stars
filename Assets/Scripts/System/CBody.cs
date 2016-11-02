using UnityEngine;
using System;
using System.Collections.Generic;

public abstract class CBody : IComparable<CBody>
{
	public string name;
	/// The orbital distance of the celestial body in AU.
	public float aphelion;
	/// The random position along it's orbit.
	public float angle;
	/// The radius of the celestial body in kilometers.
	public float size;
	/// The average density of the celestial body in grams per cubic centimeter.
	public float density;
	/// What am I orbiting?
	public CBody parent;
	public StarSystem system;
 
	public List<CBody> children;
    HashSet<string> revealedSet;

	public CBody (float _aphelion, float _angle, float _size, float _density, CBody _parent, StarSystem _system)
	{
		aphelion = _aphelion;
		angle = _angle;
		size = _size;
		density = _density;
		parent = _parent;
		system = _system;
		children = new List<CBody>();
        revealedSet = new HashSet<string>();
	}

	public abstract string getPlanetSprite ();


	public bool visible(float scale)
	{
        return aphelion == 0 || scale < aphelion * 8f && 
            (!GameCtrl.me.gameSettings.fogOfWar || parent.revealedBy(GameCtrl.me.player)); 

    }


	/// mass in units 10^12 kilograms.
	public float mass { get {
			return (4 / 3f * Mathf.PI) * density * Mathf.Pow (size, 3);
		} }
    public float solarMass { get { return mass / 2e18f; } }
    public float earthMass { get { return mass / 6e12f; } }

	public float angularVelocity (float parentMass) {
		return 1.42e-16f*Mathf.Sqrt(parentMass) *Mathf.Pow (aphelion, -1.5f);
	}

	public float hillSphere (float parentMass)
	{
		return aphelion * Mathf.Pow (mass / parentMass / 3, 1 / 3f);
	}
    public virtual IEnumerable<CBody> starChildren
    {
        get { return children; }
    }
    public bool revealedBy(Empire e)
    {
        return revealedSet.Contains(e.faction);
    }

    public bool deepRevealedBy(Empire e)
    {
        if (!revealedBy(e))
        {
            return false;
        }
        foreach (CBody child in starChildren)
        {
            if (!child.deepRevealedBy(e))
            {
                return false;
            }
        }
        return true;
    }

    public bool revealed
    {
        get { return !GameCtrl.me.gameSettings.fogOfWar || revealedBy(GameCtrl.me.player); }
    }

	public int CompareTo(CBody other)
	{
		if (other == null) return 1;
		return aphelion.CompareTo(other.aphelion);
	}

    public override string ToString()
    {
        return name;
    }

    public virtual void tick(int deltaTime)
    {
        if (parent != null)
        {
            angle += deltaTime * angularVelocity(parent.mass);
        }
        foreach (CBody child in children)
        {
            child.tick(deltaTime);
        }
    }

    public void explore(Empire empire)
    {
        revealedSet.Add(empire.faction);
        empire.knownCBodies.Add(this);
    }

    public float travelCost(CBody target)
    {
        return target.system == system ? .66f : system.distance(target.system)/2;
    }
}

