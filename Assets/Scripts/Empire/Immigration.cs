using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Immigration {

    public float airportPeople { get { return 3e-11f*colony.population; } }
    Colony colony;
    Dictionary<Planet,float> waitingListCBody = new Dictionary<Planet, float>();
    float waitingListGeneral;
    float waitingListTotal;
    private const float AIRPORT_THRESHOLD = 3000000;
    const float STAY_WEIGHT = 1.5f;

    public Immigration(Colony _colony)
    {
        colony = _colony;
    }

    public void tick(int deltaTime)
    {
        waitingListGeneral += airportPeople * deltaTime;
        if (waitingListGeneral + waitingListTotal >= AIRPORT_THRESHOLD)
        {
            var desirabilities = new Dictionary<Planet, float>();
            float totalDesirability = 0;
            var e = colony.empire.knownCBodies.OfType<Planet>().Where(planet => planet.colony == null || planet.colony.empire == colony.empire);
            foreach (Planet planet in e)
            {
                totalDesirability += desirabilities[planet] =
                    planet.desirabilityFor(colony.empire) * (planet == colony.planet ? STAY_WEIGHT : 1 / colony.planet.travelCost(planet));
            }
            foreach (Planet planet in e)
            {
                if (!waitingListCBody.ContainsKey(planet)) { waitingListCBody[planet] = 0; }
                waitingListCBody[planet] += desirabilities[planet] / totalDesirability * waitingListGeneral;
            }
            waitingListTotal += waitingListGeneral - waitingListCBody[colony.planet];
            waitingListGeneral = 0;
            waitingListCBody.Remove(colony.planet);


            var k = waitingListCBody.MaxBy(c => c.Value);
            BigEntity ship = getImmigrationShip(k.Value);
            new SequentialMission(new ImmigrateMission(k.Key), new DieMission()).set(ship).proceed();
            colony.population -= k.Value;
            waitingListTotal -= k.Value;
            waitingListCBody.Remove(k.Key);
        }
    }

    public BigEntity getImmigrationShip(float passengers)
    {
        var design = new Design().add(SubsystemType.Drive, 1).add(SubsystemType.PassengerHold, 1);
        design.get(SubsystemType.PassengerHold).currentCrew = (int)passengers;
        return new BigEntity("Passenger Vessel", colony.planet, colony.empire, false,design);
    }

}
