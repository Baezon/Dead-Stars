using UnityEngine;
using System.Linq;
public class Exploration {
    
    const int SHIP_COST = 360000;
    
    Colony colony;
    int explorationDemand;

    public Exploration(Colony _colony)
    {
        colony = _colony;
        explorationDemand = 360000;
    }

    public void tick(int deltaTime) {
        explorationDemand += deltaTime;
        
        if (explorationDemand >= SHIP_COST)
        {
            explorationDemand -= SHIP_COST;
            Vector3 pos = colony.planet.system.pos;
            StarSystem closestSystem = GameCtrl.me.systems.Where(system => !system.star.deepRevealedBy(colony.empire)).MinBy(system => (pos - system.pos).sqrMagnitude);
            if (closestSystem == null)
            {
                //Explored everything
                colony.exploration = null;
                return;
            }
            BigEntity ship = getExplorationShip();
            new SequentialMission( 
                new ExploreMission(closestSystem.star, true),
                new JumpMission(colony.planet),
                new DieMission()).set(ship).proceed();
        }
    }

    public BigEntity getExplorationShip()
    {
        return new BigEntity("Exploration Vessel", colony.planet, colony.empire, false,
            new Design().add(SubsystemType.Drive, 1).add(SubsystemType.Sensors, 1));
    }

}