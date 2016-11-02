using UnityEngine;
using System.Collections;



public enum Developments { }

public enum ShipClass { Destroyer, Cruiser, Battleship, Carrier, ECM, NWACs, Interdictor, Station, TradeShip, ExplorationVessel, PassengerVessel }

public enum FleetType { None, Destroyer, Cruiser, Battleship, Carrier,
	Fighter, Bomber, BigStation, DestroyerCruiser, DestroyerBattleship,
	DestroyerCarrier, CruiserBattleship, CruiserCarrier, Wing, PassengerVessel, ExplorationVessel }



public static class Enums
{
	

    public static string name(this ShipClass classID)
    {
        return System.Enum.GetName(typeof(ShipClass), classID);
    }

    public static string name(this FleetType classID)
    {
        return System.Enum.GetName(typeof(FleetType), classID);
    }
}


