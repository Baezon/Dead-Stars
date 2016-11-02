
using UnityEngine;
using System.Collections.Generic;

public class CBodyInfo : MonoBehaviour
{
    private const float LINE_HEIGHT = .17f;
    private const float DOTS_LINE_HEIGHT = .07f;
    private const float BORDER = .03f;
    private const float PADDING = .04f;
    private const float VERTICAL_DISPLACEMENT = .40f;
    private const float CHAR_WIDTH = .088f;

    public CBody cBody;
    float time;
    public IList<GameObject> box;
    public IList<TextMesh> line;
    int lines = 9;
    int lineIndex;
    float lineY;

    public CBodyInfo init(CBody _cBody)
    {
        cBody = _cBody;
        transform.localPosition = VERTICAL_DISPLACEMENT * Vector3.down;
        box = new List<GameObject>();
        line = new List<TextMesh>();
        refresh();
        return this;
    }

    void boxify(int lines)
    {
        if (this.lines == lines)
        {
            return;
        }
        this.lines = lines;
        foreach (GameObject o in box)
        {
            Destroy(o);
        }
        box.Clear();
        int maxLen = 0;
        for (int i = 0; i < line.Count; i++)
        {
            if (i < lines)
            {
                maxLen = Mathf.Max(maxLen, line[i].text.Length);
            }
            else
            {
                line[i].text = "";
            }
        }
        foreach (GameObject o in GameCtrl.me.prefabs.box)
        {
            box.Add(createBox(Instantiate(o), maxLen));
        }
    }

    public void setVisible(bool vis)
    {
        time = 0;
        gameObject.SetActive(vis);
    }

    private void setLine(string text, float lineHeight = LINE_HEIGHT)
    {
        if (lineIndex == line.Count)
        {
            GameObject lineObj = Instantiate(GameCtrl.me.prefabs.text);
            lineObj.transform.SetParent(transform, false);
            line.Add(lineObj.GetComponent<TextMesh>());
        }
        line[lineIndex].transform.localPosition = (lineY + lineHeight * .5f) * Vector3.down;
        line[lineIndex].text = text;
        lineIndex++;
        lineY += lineHeight;
    }

    private void refresh()
    {
        lineIndex = 0;
        lineY = BORDER + PADDING;
        setLine(cBody.name);
        if (cBody is Planet && ((Planet)cBody).colony != null)
        {
            line[0].color = ((Planet)cBody).colony.empire.settings.color;
        }
        bool bigInfo = time > 3f;
        if (cBody is Star)
        {
            Star star = (Star)cBody;
            setLine(string.Format("Temperature: {0:#,###0} K", star.temp));
            setLine(string.Format("Mass: {0:f3} M☉", star.solarMass));
            if (bigInfo)
            {
                setLine(string.Format("Radius: {0:#,###0} km", star.size));
                setLine(string.Format("Luminosity: {0:f3} L☉", star.luminosity / 3.846e26f));
                setLine(string.Format("Goldilocks: {0:f3} AU", star.goldilocksCenter));
            }
        }
        else if (cBody is Planet)
        {
            Planet planet = (Planet)cBody;
            float period = 6.28f / (60 * 60 * 24) / planet.angularVelocity(planet.parent.mass);
            if (planet.colony != null)
            {
                if (bigInfo)
                {
                    setLine(planet.colony.empire.settings.adjectival
                        + (planet.colony.empire.capitol == planet.colony ? " Capitol" : " Colony"));
                }
                setLine(string.Format("Population: {0:#,###0}", planet.colony.population));
            }

            if (bigInfo)
            {
                setLine("Orbiting " + cBody.parent);
                if (cBody.revealed) { setLine(string.Format("Habitability: {0:f3}", planet.habitability)); }
                setLine(cBody is Moon ?
                    string.Format("Aphelion: {0:#,###0} km", planet.aphelion * 1.496e8) :
                    string.Format("Aphelion: {0:f3} AU", planet.aphelion));
                setLine(string.Format("Radius: {0:#,###0} km", planet.size));
                setLine(period > 730 ?
                    string.Format("Period: {0:f1} years", period / 365) :
                    string.Format("Period: {0:f0} days", period));
                setLine(string.Format("Mass: {0:f3} M☉", planet.mass / 6e12f));
                int numMoons = cBody.children.Count;
                if(numMoons > 0)
                {
                    setLine(numMoons + (numMoons > 1 ? " Moons" : " Moon"));
                }
            }
            else
            {
                if (planet.colony == null)
                {
                    setLine(planet.habitability > 6f ?
                        string.Format("Habitability: {0:f3} ", planet.habitability) :
                        period > 730 ?
                            string.Format("Period: {0:f1} years", period / 365) :
                            string.Format("Period: {0:f0} days", period));
                }
                setLine(cBody is Moon ?
                    string.Format("Aphelion: {0:#,###0} km", planet.aphelion * 1.496e8) :
                    string.Format("Aphelion: {0:f3} AU", planet.aphelion));
            }
        }
        if (!bigInfo)
        {
            if (time > 2.25f)
            {
                setLine("···", DOTS_LINE_HEIGHT);
            }
            else if (time > 1.5f)
            {
                setLine("··", DOTS_LINE_HEIGHT);
            }
            else if (time > .75f)
            {
                setLine("·", DOTS_LINE_HEIGHT);
            }
            else
            {
                setLine("", DOTS_LINE_HEIGHT);
            }
        }
        boxify(lineIndex);
    }

    void Update()
    {
        time += Time.deltaTime;
        refresh();
    }

    GameObject createBox(GameObject box, int maxLen)
    {
        box.transform.SetParent(transform, false);
        Mesh mesh = box.GetComponent<MeshFilter>().mesh;
        Vector3[] verts = mesh.vertices;
        float width = Mathf.RoundToInt(maxLen * CHAR_WIDTH / .01f)*.01f + 2 * (BORDER + PADDING);
        float height = lineY + (BORDER + PADDING);
        for (int i = 0; i < verts.Length; i++)
        {
            verts[i] = new Vector3(nineSlice(verts[i].x, -width / 2, width, BORDER), nineSlice(verts[i].y, -height, height, BORDER), -.01f);
        }
        mesh.vertices = verts;
        return box;
    }

    float nineSlice(float t, float left, float width, float border)
    {
        return left + (t <= 1 ? border * t : t <= 2 ? border * (3 - 2 * t) + width * (t - 1) : width + border * (t - 3));
    }
}
