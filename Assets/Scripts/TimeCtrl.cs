using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public class TimeCtrl {
    long MAX_TICKS = (long)(Stopwatch.Frequency * Time.maximumDeltaTime);
    private bool _paused = true;
    public long time;
    private float timeFraction;
    SortedDictionary<long, List<Event>> events = new SortedDictionary<long, List<Event>>();
    float throttling = 1;

    public bool paused
    {
        get { return _paused; }
        set
        {
            if (_paused != value)
            {
                _paused = value;
                GameCtrl.me.pauseButton.GetComponent<Image>().sprite = Resources.Load<Sprite>(value ?
                    "UI/Pause Button/play" : "UI/Pause Button/pause");
            }
        }
    }

    public void tick(float requestedDeltaTime)
    {
        timeFraction += requestedDeltaTime;

        int deltaTime = (int)timeFraction;
        timeFraction -= deltaTime;
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        requestedDeltaTime = deltaTime;
        while (deltaTime > 0 && stopwatch.ElapsedTicks < MAX_TICKS) { deltaTime -= tryTick(deltaTime); }
        throttling = 1-( deltaTime > 0 ? deltaTime / requestedDeltaTime : 0);
    }

    int tryTick(int deltaTime)
    {
        var i = events.GetEnumerator();
        List<Event> happeningNow = null;
        if (i.MoveNext())
        {
            long eventTime = i.Current.Key;

            if (eventTime < time + deltaTime)
            {
                happeningNow = i.Current.Value;
                events.Remove(eventTime);
                deltaTime = (int)(eventTime - time);
            }
        }
        time += deltaTime;
        if (happeningNow != null)
        {
            foreach (Event thing in happeningNow)
            {
                thing.f();
            }
        }
        foreach (StarSystem system in GameCtrl.me.systems)
        {
            foreach (CBody cBody in system.cBodies)
            {
                //cbody calls colony's tick, which then call its children's ticks
                cBody.tick(deltaTime);
            }
        }

        return deltaTime;
    }

    public void OnGUI()
    {
        float y = 10;
        foreach (Colony colony in GameCtrl.me.player.colonies.OrderByDescending(colony => colony.population))
        {
            if (y > 700) break;
            GUI.Label(new Rect(60, y += 20, 4000, 26), string.Format("{0} : pop={1:#,###0}, hab={2}, min={3}, des={4}",
                colony, colony.population, colony.planet.habitability, colony.planet.mineralAbundance, colony.planet.desirability));
        }
        if (throttling < 1)
        {
            GUI.Label(new Rect(130, Screen.height - 45, 400, 26), string.Format("Throttling {0}", throttling));
        }
    }

    public void addEvent(Event e)
    {

        List<Event> list;
        if (events.TryGetValue(e.time, out list))
        {
            list.Add(e);
        }
        else
        {
            events[e.time] = new List<Event> { e };
        }
    }

    public void removeEvent(Event e)
    {
        List<Event> list;

        if (events.TryGetValue(e.time, out list))
        {
            list.Remove(e);
        }
    }

    public void postponeEvent(Event e, long toTime)
    {
        removeEvent(e);
        addEvent(new Event(toTime<time ? time : toTime, e.f));
    }
}

public struct Event
{
    public System.Action f;
    public long time;

    public Event(long _time, System.Action _f)
    {
        time = _time;
        f = _f;
    }

}
