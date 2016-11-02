using UnityEngine;
using System.Collections;

public enum SubsystemType
{
    Engines, SmallGuns, MediumGuns, LargeGuns, Hangar,
    Shields, Armor, LifeSupport, NWACs, ECM, Interdictor, Shipyard,
    CargoBay, Drive, PowerPlant, PassengerHold, Sensors
}

public abstract class Subsystem
{
    public int level;
    public int currentCrew;
    public float currentHP;
    SubsystemType type;

    public Subsystem() { }

    public static Subsystem get(SubsystemType type, int level)
    {
        Subsystem s;
        switch (type)
        {
            case SubsystemType.Drive: s = new DriveSubsystem(); break;
            case SubsystemType.SmallGuns: s = new SmallGunsSubsystem(); break;
            case SubsystemType.PassengerHold: s = new PassengerHoldSubsystem(); break;
            case SubsystemType.Sensors: s = new SensorsSubsystem(); break;
            default: throw new System.Exception("eyy check that switch in Subsystem, nigga");
        }
        s.currentCrew = 1;
        s.currentHP = 1;
        s.type = type;
        s.level = level;
        return s;
    }

    public Subsystem copy()
    {
        Subsystem s;
        switch (type)
        {
            case SubsystemType.Drive: s = new DriveSubsystem(); break;
            case SubsystemType.SmallGuns: s = new SmallGunsSubsystem(); break;
            case SubsystemType.PassengerHold: s = new PassengerHoldSubsystem(); break;
            case SubsystemType.Sensors: s = new SensorsSubsystem(); break;
            default: throw new System.Exception("eyy check that switch in Subsystem, nigga");
        }
        s.type = type;
        s.level = level;
        s.currentCrew = currentCrew;
        s.currentHP = currentHP;
        return s;

    }
}

public class DriveSubsystem : Subsystem
{
    public float speed { get { return 2f; } }
}

public class SmallGunsSubsystem : Subsystem
{
}

public class PassengerHoldSubsystem : Subsystem
{
}
public class SensorsSubsystem : Subsystem
{
}