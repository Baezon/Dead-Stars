using UnityEngine;
using System.Collections.Generic;

public enum Good {food, ore, metal, gas, fuel, constructionMat, warMat, habitability, mineralAbundance, gasAbundance}

public struct Industry {

    public string name;
    public Good output;
    public Dictionary<Good, float> input;

    public Industry(string _name, Good _output)
    {
        name = _name;
        output = _output;
        input = new Dictionary<Good, float>();
    }

    public Industry add(Good good, float quantity)
    {
        input.Add(good, quantity);
        return this;
    }

    public string ToString()
    {
        return name;
    }



    public static Industry[] industries = new Industry[] { 
        new Industry("Farm", Good.food).add(Good.habitability, 1), 
        new Industry("Mine", Good.ore).add(Good.mineralAbundance, 1), 
        new Industry("Extractor", Good.gas).add(Good.gasAbundance, 1), 
        new Industry("Smeltery", Good.metal).add(Good.ore, 1), 
        new Industry("War Factory", Good.warMat).add(Good.metal, 1), 
        new Industry("Factory", Good.constructionMat).add(Good.metal, 1), 
        new Industry("Refinery", Good.fuel).add(Good.gas,1)
    };
}


