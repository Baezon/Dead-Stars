using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class Selection : IDictionary<Entity, Action>
{
    private Dictionary<Entity, Action> selected = new Dictionary<Entity,Action>();

	public ICollection<Entity> entities
	{
		get
		{
			return Keys;
		}
		set
		{
			Clear();
			foreach (Entity e in value) {
				Add(e, null);
			}
		}
	}

    public Entity entity
    {
        get
        {
            if (entities.Count == 1)
            {
                var i = entities.GetEnumerator();
                i.MoveNext();
                return i.Current;
            }
            return null;
        }
        set
        {
			entities = new List<Entity>{value};
        }

    }

	public void Clear () {
		foreach (Entity e in new List<Entity>(Keys)) {
			Remove (e);
		}
	}
	
	public bool Remove (Entity key)
	{
		Action value;
		if (TryGetValue (key, out value)) {
			if (value!=null) value();
			return selected.Remove (key);
		} else {
			return false;
		}
	}

    public void Add(Entity key, Action value)
    {
        selected.Add(key, value);
    }

    public bool ContainsKey(Entity key)
    {
        return selected.ContainsKey(key);
    }

    public ICollection<Entity> Keys
    {
        get { return selected.Keys; }
    }

    public bool TryGetValue(Entity key, out Action value)
    {
        return selected.TryGetValue(key, out value);
    }

    public ICollection<Action> Values
    {
        get { return selected.Values; }
    }

    public Action this[Entity key]
    {
        get
        {
            return selected[key];
        }
        set
        {
            selected[key] = value;
        }
    }

    public IEnumerator GetEnumerator()
    {
        return selected.GetEnumerator();
    }

    public void Add(KeyValuePair<Entity, Action> item)
    {
        ((ICollection<KeyValuePair<Entity, Action>>)selected).Add(item);
    }

    public bool Contains(KeyValuePair<Entity, Action> item)
    {
        return ((ICollection<KeyValuePair<Entity, Action>>)selected).Contains(item);
    }

    public void CopyTo(KeyValuePair<Entity, Action>[] array, int arrayIndex)
    {
        ((ICollection<KeyValuePair<Entity, Action>>)selected).CopyTo(array,arrayIndex);
    }

    public int Count
    {
        get { return selected.Count; }
    }

    public bool IsReadOnly
    {
        get { return false; }
    }

    public bool Remove(KeyValuePair<Entity, Action> item)
    {
        return Remove(item.Key);
    }

    IEnumerator<KeyValuePair<Entity, Action>> IEnumerable<KeyValuePair<Entity, Action>>.GetEnumerator()
    {
        return ((IEnumerable<KeyValuePair<Entity, Action>>)selected).GetEnumerator();
    }
}