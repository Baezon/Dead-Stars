using UnityEngine;
using System.Collections;
using System;

public class BigEntity : Entity
{

    public Design baseDesign;
    public Design design;

    public override float currentHP
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    public override float maxHP
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    public override ShipClass getClass()
    {
        return design.getClass();
    }

    public BigEntity(string _name, CBody _location, Empire _empire, bool _military, Design _baseDesign)
        : base(_name, _location, _empire, _military)
    {
        design = new Design(baseDesign = _baseDesign);
    }

    public override bool canJump(CBody destination)
    {
        Subsystem drive = design.get(SubsystemType.Drive);
        return drive != null && drive.currentHP != 0f;
    }


    public override int jumpChargeTime(CBody destination)
    {
        return 600;
    }

    public override int travelTime(CBody destination)
    {
        return 6000;
    }
}

