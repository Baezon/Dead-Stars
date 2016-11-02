using System;

public class Design {

    static ShipClass[] classTypes = { ShipClass.Carrier, ShipClass.ECM, ShipClass.NWACs, ShipClass.Interdictor,
		ShipClass.Battleship, ShipClass.Destroyer, ShipClass.Cruiser, ShipClass.Station, ShipClass.TradeShip, 
        ShipClass.ExplorationVessel, ShipClass.PassengerVessel,};
    static SubsystemType[] subsystemTypes = {  SubsystemType.Hangar, SubsystemType.ECM, SubsystemType.NWACs,SubsystemType.Interdictor, 
        SubsystemType.LargeGuns, SubsystemType.SmallGuns, SubsystemType.MediumGuns , SubsystemType.Shipyard, SubsystemType.CargoBay,
        SubsystemType.Sensors, SubsystemType.PassengerHold };

    int[] devLevels;
    Subsystem[] subsystems;

    public Design()
    {
        devLevels = new int[System.Enum.GetValues(typeof(Developments)).Length];
        subsystems = new Subsystem[System.Enum.GetValues(typeof(SubsystemType)).Length];
    }
    public Design(Design baseDesign)
    {
        devLevels = (int[])baseDesign.devLevels.Clone();
        subsystems = new Subsystem[baseDesign.subsystems.Length];
        for (int i = 0; i < subsystems.Length; i++)
        {
            Subsystem subsystem = baseDesign.subsystems[i];
            subsystems[i] = subsystem == null ? null : subsystem.copy(); 
        }
    }

    public ShipClass getClass()
    {
        for (int i = 0; i < classTypes.Length; i++)
        {
            if (get(subsystemTypes[i]) != null)
            {
                return classTypes[i];
            }
        }
        throw new System.Exception("eyy check that for loop in getClass in Design, nigga");
    }

    public Subsystem get(SubsystemType type)
    {
        return subsystems[(int)type];
    }
    public Design add(SubsystemType type, int size)
    {
        subsystems[(int)type] = Subsystem.get(type,size);
        return this;
    }
    public PassengerHoldSubsystem passengerHold { get { return (PassengerHoldSubsystem)get(SubsystemType.PassengerHold); } }

}
