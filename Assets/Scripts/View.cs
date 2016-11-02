using UnityEngine;
using System.Collections;

public abstract class View : MonoBehaviour {
    private Vector3 startDrag;
    private bool dragging;
    bool dirty;

    /*public abstract void select();*/

    public abstract void OnMouseDown();

    public abstract void OnHover();

    public abstract void OnDrag();

    public void invalidate()
    {
        dirty = true;
    }
    protected abstract void updateObjects();

    public virtual void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            startDrag = Input.mousePosition;
            dragging = false;
        }
        if (Input.GetMouseButton(1))
        {
            if (dragging |= (Input.mousePosition - startDrag).sqrMagnitude > 30)
                OnDrag();
        }
        if (!dragging && Input.GetMouseButtonUp(1))
        {
            DoContextMenu();
        }
        if (dirty)
        {
            dirty = false;
            updateObjects();

        }
    }

    public virtual void DoContextMenu() { }
}
