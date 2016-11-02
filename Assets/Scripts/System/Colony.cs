using System.Linq;
using UnityEngine;
public class Colony
{
    /// Converts percent per year to units per second
    private const float GROWTH_RATE_FACTOR = .01f / (365 * 24 * 60 * 60);
    /// industry is limited to taking this percent per second of of its stores as input, first number is percent per day
    private const float MAX_STORE_TAKE_RATE = .03f / (24 * 60 * 60);

    public float population;
    public Planet planet;
    public Empire empire;
    public Exploration exploration;
    public Immigration immigration;
    public float[] stores;
    public float[] industries;

    public Colony(float _population, Planet _planet, Empire _empire)
    {
        population = _population;
        planet = _planet;
        empire = _empire;
        stores = new float[System.Enum.GetValues(typeof(Good)).Length];
        industries = new float[Industry.industries.Length];
        planet.colony = this;
        empire.colonies.Add(this);
        exploration = new Exploration(this);
        immigration = new Immigration(this);
    }

    public float getStores(Good good)
    {
        switch(good)
        {
            case Good.mineralAbundance: return planet.mineralAbundance;
            case Good.gasAbundance: return planet.gasAbundance;
            case Good.habitability: return planet.habitability;
            default : return stores[(int)good];
        }
    }

    public void tick(int deltaTime)
    {

        float rate = GROWTH_RATE_FACTOR * empire.settings.growthRate *
            (population / planet.carryingCapacity) * (planet.carryingCapacity - population);
        population += rate * deltaTime;
        if (exploration != null) { exploration.tick(deltaTime); }
        immigration.tick(deltaTime);
        industryTick(deltaTime);
    }

    public void industryTick(int deltaTime)
    {
        for (int i = 0; i < industries.Length; i++)
        {
            Industry industry = Industry.industries[i];
            float size = industries[i];
            float volume = Mathf.Min(industry.input.Min(kvp => getStores(kvp.Key) / kvp.Value) *(MAX_STORE_TAKE_RATE),size) * deltaTime;
            float efficiency = 1;
            stores[(int)industry.output] = getStores(industry.output) + volume * efficiency;
            foreach (var kvp in industry.input)
            {
                stores[(int)kvp.Key] = getStores(kvp.Key) - volume;
            }
            industries[i] += deltaTime;
        }

    }


    public override string ToString()
    {
        return planet.ToString();
    }

}
