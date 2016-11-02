using UnityEngine;
using System.Collections.Generic;

public class Empire {

	
	public IList<Entity> entities = new List<Entity>();
	public string faction;
	public float money;
    public int manPower;
    public Colony capitol;
    public ICollection<Colony> colonies = new List<Colony>();
    public ICollection<CBody> knownCBodies = new List<CBody>();
    public EmpireSettings settings { get { return GameCtrl.me.gameSettings.getEmpire(faction); } }

	public Empire(string _faction, StarSystem system)
	{
		faction = _faction;
		money = 1;
		manPower = 1;
        genCapitol(system);
    }

    public void genCapitol(StarSystem system)
    {
        float candidateFactor = Mathf.Infinity;
        Planet candidate = null;
        float goldilocks = system.star.goldilocksCenter;
        //This gets the planet closest to the goldilocks center
        foreach (CBody cBody in system.cBodies)
        {
            if (cBody is Planet)
            {
                Planet planet = (Planet)cBody;
                float factor = cBody.aphelion / goldilocks;
                if (factor < 1)
                {
                    factor = 1 / factor;
                }
                if (factor < candidateFactor)
                {
                    candidate = planet;
                    candidateFactor = factor;
                }
            }
        }
        //If the best candidate planet is still too far away, it moves it closer
        if (candidateFactor > 1.3f)
        {
            candidate.aphelion = goldilocks * Random.Range(0.9f, 1.1f);
            candidate.rollMoons(candidate.rocky);
        }
        //If the candidate is a gas giant it makes a new moon, and makes it the candidate
        if (!candidate.rocky)
        {
            if (candidate.children.Count == 0)
            {
                Moon newCandidate = new Moon(candidate, system);
                candidate.children.Add(newCandidate);
                candidate = newCandidate;
            }
            //or just takes a random moon if it already has some
            else
            {
                candidate = (Planet)candidate.children[Random.Range(0, candidate.children.Count)];
            }
        }
        //and then makes whatever it got the capitol
        candidate.habitability = Random.Range(10.6f, 10.9f);
        candidate.desirability = candidate.baseDesirability();
        candidate.terrestrial = true;
        capitol = new Colony(.9f * candidate.carryingCapacity, candidate, this);
        candidate.genSpriteName();
        foreach (CBody cBody in candidate.system.deepCBodies)
        {
            cBody.explore(this);
        }
    }



}