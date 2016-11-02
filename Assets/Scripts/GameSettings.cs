using UnityEngine;
using System.Collections.Generic;
using Hjson;

public class GameSettings
{

    JsonObject settings;


    public void loadSettings()
    {
        settings = HjsonValue.Parse(Resources.Load<TextAsset>("GameSettings").text).Qo();
    }

    public JsonArray firstHalfList
    {
        get { return settings.Qo("SystemNames").Qa("firsthalf"); }
    }
    public JsonArray secondHalfList
    {
        get { return settings.Qo("SystemNames").Qa("secondhalf"); }

    }
    public JsonArray aloneList
    {
        get { return settings.Qo("SystemNames").Qa("alone"); }
    }
    public JsonArray empireList
    {
        get { return settings.Qo("GameSettings").Qa("EmpireList"); }
        set { settings.Qo("GameSettings")["EmpireList"] = value; }
    }
    public EmpireSettings getEmpire(string faction)
    {
        return new EmpireSettings(settings.Qo("Empires").Qo(faction));
    }

    public bool fogOfWar
    {
        get { return settings.Qo("GameSettings").Qb("FogOfWar"); }
        set { settings.Qo("GameSettings")["FogOfWar"] = value; }
    }
}

public class EmpireSettings
{
    JsonObject node;
    public EmpireSettings(JsonObject _node)
    {
        node = _node;
    }

    public Color color
    {
        get { return node["Color"].Qcolor(); }
        set { node["Color"] = value.ToJson(); }
    }

    public string prefix
    {
        get { return node["PrefixName"]; }
    }
    public string adjectival
    {
        get { return node["Adjectival"]; }
    }
    public float growthRate
    {
        get { return (float)node.Qd("GrowthRate"); }
    }
    public string shipPrefix
    {
        get { return node["ShipPrefix"]; }
    }
    public float colonizationPenalty
    {
        get { return (float)node.Qd("ColonizationPenalty"); }
    }
}


