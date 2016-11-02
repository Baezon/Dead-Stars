using UnityEngine;
using System.Collections.Generic;

public class SystemView : View
{

    const float TARGET_SIZE = 64 * 64;
    public CBodyObject focus;
    float lastClicked;
    const float SOI_LIMIT = 10000f;
    public Dictionary<CBody, CBodyObject> dictionary;
    float leaveSystemCounter = 0f;
    float maxZoom;
    const float MIN_ZOOM = 1f;
    const float CAM_Z = -2000f;
    CBodyObject cBodyHover;
    Shpip shpipHover;
    public StarSystem system;
    const float DOUBLE_CLICK_TIME = .4f;

    public void initialize(StarSystem _system)
    {
        system = _system;
        dictionary = new Dictionary<CBody, CBodyObject>();
        GameCtrl.me.cam.transform.position = CAM_Z * Vector3.forward;
        GameCtrl.me.cam.transform.LookAt(Vector3.zero);
        float d = MIN_ZOOM;
        foreach (CBody cBody in system.cBodies)
        {
            GameObject obj = Instantiate(GameCtrl.me.prefabs.cBodyTemplate);
            obj.transform.parent = transform;
            CBodyObject cBodyObj = obj.GetComponent<CBodyObject>();
            cBodyObj.Initialize(cBody, dictionary);
            dictionary.Add(cBody, cBodyObj);
            d = Mathf.Max(d, 100*cBody.aphelion);
        }
        GameCtrl.me.cam.orthographicSize = maxZoom = 1.05f*d;
        //transform.rotation = Quaternion.Euler(90, 0, 0);
        invalidate();
    }

    

    public override void OnDrag()
    {
        float scale = .05f * GameCtrl.me.cam.orthographicSize;
        Vector3 camPos = GameCtrl.me.cam.transform.position;
        camPos.y -= scale * Input.GetAxis("Mouse Y");
        camPos.x -= scale * Input.GetAxis("Mouse X");
        camPos.z = 0;
        if(camPos.sqrMagnitude>maxZoom*maxZoom)
        {
            camPos = maxZoom * camPos.normalized;
        }
        camPos.z = CAM_Z;
        GameCtrl.me.cam.transform.position = camPos;
        if (focus != null)
        {
            Vector3 displacement = focus.transform.position - camPos;
            displacement.z = 0;
            if (displacement.sqrMagnitude >= SOI_LIMIT)
            {
                focus = null;
            }
        }
    }




    void Start()
    {

    }

    public override void Update()
    {
        base.Update();
        float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
        if (scrollWheel != 0)
        {

            GameCtrl.me.cam.orthographicSize *= Mathf.Exp(-2 * scrollWheel);

            if (GameCtrl.me.cam.orthographicSize > maxZoom)
            {
                GameCtrl.me.cam.orthographicSize = maxZoom;
                leaveSystemCounter -= scrollWheel;
                if (leaveSystemCounter > .5f)
                {
                    GameCtrl.me.goToGalaxyView();
                }
            }
            if (GameCtrl.me.cam.orthographicSize < MIN_ZOOM)
            {
                GameCtrl.me.cam.orthographicSize = MIN_ZOOM;
            }
            if (scrollWheel > 0)
            {
                leaveSystemCounter = 0;
            }
        }
        if (Input.GetKeyDown("a") && cBodyHover != null)
        {
            BigEntity bb = new BigEntity("USS Padoodle", cBodyHover.cBody, GameCtrl.me.empires["Conglomerate"], true,
                new Design().add(SubsystemType.Drive, 1).add(SubsystemType.SmallGuns, 1));
            invalidate();
            try
            {
                new JumpMission(bb.location.parent).set(bb).proceed();
            }
            catch (MissionFailedException e)
            {
                Debug.Log(e.Message);
            }
        }
        if (Input.GetKeyDown("s") && cBodyHover != null)
        {
            BigEntity bb = new BigEntity("Destroyer", cBodyHover.cBody, GameCtrl.me.empires["Autocracy"], true,
                new Design().add(SubsystemType.Drive, 1).add(SubsystemType.SmallGuns, 1));
            invalidate();
            try
            {
                new JumpMission(bb.location.parent).set(bb).proceed();
            }
            catch (MissionFailedException e)
            {
                Debug.Log(e.Message);
            }
        }

        foreach (CBody cBody in system.deepCBodies)
        {
            dictionary[cBody].fakeUpdate();
        }
        foreach (CBodyObject cBodyObj in dictionary.Values)
        {
            cBodyObj.goodGuys.updateTravelLines();
            cBodyObj.badGuys.updateTravelLines();
            cBodyObj.station.updateTravelLines();
        }

    }

    void LateUpdate()
    {
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            invalidate();
        }
    }

    protected override void updateObjects()
    {
        foreach (CBodyObject cBodyObj in dictionary.Values)
        {
            cBodyObj.goodGuys.clearEntities();
            cBodyObj.badGuys.clearEntities();
            cBodyObj.station.clearEntities();
        }
        float wScreenHeightUnits = GameCtrl.me.cam.orthographicSize * 2f;
        float scale = wScreenHeightUnits / Screen.height;
        foreach (Empire empire in GameCtrl.me.empires.Values)
        {
            foreach (Entity entity in empire.entities)
            {
                CBody src = entity.location;
                while (!src.visible(scale)) { src = src.parent; }
                CBody dest = src;
                if (entity.status is Jumping)
                {
                    dest = ((Jumping)entity.status).destination;
                    while (!dest.visible(scale)) { dest = dest.parent; }
                }
                
                if (dest != src)
                {
                }
                if (src.system == system)
                {
                    CBodyObject o = dictionary[src];
                    Shpip s = Shpip.getShpip(o.goodGuys, o.badGuys, o.station, entity);
                    s.entities.Add(entity);
                    s.updateFleetType();
                    if (dest != src)
                    {
                        if (dest.system == system)
                        {
                            s.addTravelLine(dictionary[dest]);
                        }
                    }
                }
            }
        }
        foreach (CBody cBody in system.deepCBodies)
        {
            dictionary[cBody].updateColor();
        }
    }

    void OnGUI()
    {
        var entity = GameCtrl.me.selected.entity;
        if (entity != null)
        {
            int y = -10;
            GUI.Label(new Rect(10, y +=20, 400, 26), entity.location.name);
            GUI.Label(new Rect(10, y += 20, 400, 26), "" + entity.topMission);
        }
        if (cBodyHover != null)
        {

            if(!(cBodyHover.cBody is Star) && ((Planet)cBodyHover.cBody).colony != null)
            {
                Colony colony = ((Planet)cBodyHover.cBody).colony;
                int y = 200;
                int i = 0;
                foreach (Good g in System.Enum.GetValues(typeof(Good)))
                {
                    if (!g.special())
                    {
                        GUI.Label(new Rect(10, y += 20, 400, 26), g.ToString() + ", " + colony.getStores(g));
                    }
                }
                
            }
        }
    }
    //voronoi shit
    //vv
    Transform nearestChild(Transform root, Vector3 mousePosition)
    {
        if (root.GetComponent<CBodyObject>() == null || !root.gameObject.activeSelf)
        {
            return null;
        }
        Vector3 screenPos = GameCtrl.me.cam.WorldToScreenPoint(root.position);
        screenPos.z = 0;
        Vector3 displacement = mousePosition - screenPos;
        Transform target = root;
        float minDistance = displacement.sqrMagnitude;
        foreach (Transform child in root)
        {
            Transform nearest = nearestChild(child, mousePosition);
            if (nearest != null)
            {
                Vector3 screenPos2 = GameCtrl.me.cam.WorldToScreenPoint(nearest.position);
                screenPos2.z = 0;
                Vector3 displacement2 = mousePosition - screenPos2;
                float magnitude = displacement2.sqrMagnitude;
                if (magnitude < minDistance)
                {
                    target = nearest;
                    minDistance = magnitude;
                }
            }
        }
        return target;
    }
    CBodyObject nearestChild(out float minDistance)
    {
        minDistance = Mathf.Infinity;
        Vector3 mousePosition = Input.mousePosition;
        Transform target = null;
        foreach (Transform child in transform)
        {
            Transform nearest = nearestChild(child, mousePosition);
            if (nearest != null)
            {
                Vector3 screenPos = GameCtrl.me.cam.WorldToScreenPoint(nearest.position);
                screenPos.z = 0;
                Vector3 displacement = mousePosition - screenPos;
                float magnitude = displacement.sqrMagnitude;
                if (magnitude < minDistance)
                {
                    target = nearest;
                    minDistance = magnitude;
                }
            }
        }
        return target.GetComponent<CBodyObject>();
    }
    //^^
    //voronoi shit



    public override void OnHover()
    {
        float minDistance;
        CBodyObject target = nearestChild(out minDistance);
        CBodyHover(minDistance < TARGET_SIZE ? GetVisibleAncestor(target) : null);
    }

    public void CBodyHover(CBodyObject target)
    {
        if (cBodyHover != target)
        {
            if (cBodyHover != null)
            {
                cBodyHover.GetComponent<SpriteRenderer>().sprite
                    = Resources.Load<Sprite>("CBody icons/" + cBodyHover.cBody.getPlanetSprite());
                cBodyHover.cBodyInfo.setVisible(false);
            }
            cBodyHover = target;
            if (cBodyHover != null)
            {
                cBodyHover.GetComponent<SpriteRenderer>().sprite
                    = Resources.Load<Sprite>("CBody icons/" + cBodyHover.cBody.getPlanetSprite() + "hover");
                cBodyHover.cBodyInfo.setVisible(true);
            }
        }
    }

    public CBodyObject GetVisibleAncestor(CBodyObject target)
    {
        return target.GetComponent<SpriteRenderer>().enabled ? target : target.transform.parent.GetComponent<CBodyObject>();
    }

    public override void OnMouseDown()
    {
        float minDistance;
        CBodyObject target = nearestChild(out minDistance);
        if (minDistance >= TARGET_SIZE)
        {
            if (Time.time - lastClicked < DOUBLE_CLICK_TIME)
            {
                GameCtrl.me.goToGalaxyView();
            }
        }
        else
        {
            if (Time.time - lastClicked < DOUBLE_CLICK_TIME)
            {
                setFocus(target);
            }
        }
        lastClicked = Time.time;
    }

    public void setFocus(CBodyObject cBody)
    {
        if (cBody.cBody is Moon)
        {
            cBody = cBody.transform.parent.GetComponent<CBodyObject>();
        }
        if (cBody != focus)
        {
            focus = cBody;
            Vector3 camPos = GameCtrl.me.cam.transform.position;
            camPos.x = focus.transform.position.x;
            camPos.y = focus.transform.position.y;
            GameCtrl.me.cam.transform.position = camPos;
        }
    }

    public override void DoContextMenu()
    {
    }
}
