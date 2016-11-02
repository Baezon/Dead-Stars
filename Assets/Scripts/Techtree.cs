using System.Collections.Generic;
using System;
using System.Linq;
using Hjson;

public class TechTree
{
    IList<Tech> techs;
    Dictionary<string, Func<JsonArray, Action<Entity>>> commands;

    public TechTree(JsonObject root)
    {
        techs = new List<Tech>();
        commands = new Dictionary<string, Func<JsonArray, Action<Entity>>>() {
            {"Damage", args => ship => {}}
        };
        foreach (KeyValuePair<string, JsonValue> e in root.Qo("ExtraCommands"))
        {
            JsonArray array = e.Value.Qa();
            commands[e.Key] = args => getEffects(array, args);
        }
        foreach (KeyValuePair<string, JsonValue> e in root.Qo("TechTree"))
        {
            foreach (JsonValue u in e.Value.Qa())
            {
                addTech(null, u.Qo(), e.Key);
            }
        }
    }

    void addTech(Tech parent, JsonObject node, string category)
    {
        Tech tech = new Tech();
        tech.name = node.Qs("Name");
        tech.category = category;
        tech.devCost = node.Qi("Cost");
        tech.parent = parent;
        tech.effects = getEffects(node.Qa("Effects"), null);
    }

    Action<Entity> getEffects(JsonArray k, JsonArray args)
    {
        List<Action<Entity>> list = k.Select(effect => getEffect(effect, args)).ToList();
        return e => {
            foreach (Action<Entity> f in list)
                f(e);
        };
    }

    Action<Entity> getEffect(JsonValue k, JsonArray args)
    {
        return commands[k[0].Qs()](args == null ? k.Qa() :
            new JsonArray(k.Qa().Select(arg =>
            {
                int n;
                return arg.JsonType == JsonType.String && arg.Qs()[0] == '$' &&
                    int.TryParse(arg.Qs().Substring(1), out n) ? args[n] : arg;
            })));
    }
}

public class Tech
{
    public string name;
    public string category;
    public int devCost;
    public Tech parent;
    public Action<Entity> effects;
}