using UnityEngine;
using System.Collections;

public class SelectionDropdownItem : MonoBehaviour {

    public Entity entity;
	TextMesh text;
    public bool isHovering=false;
    public int pos;
    SpriteRenderer sr;
    public string image;

	public void init(int pos, string image){
        this.pos = pos;
        this.image = image;
        sr = GetComponentInChildren<SpriteRenderer>();
		text = GetComponent<TextMesh> ();
        
		if (GameCtrl.me.selected.ContainsKey(entity)) 
        {
			select ();
		} else { setImage(image); }
	}

    void OnMouseEnter()
    {
        isHovering = true;
        setImage((GameCtrl.me.selected.entities.Contains(entity) ? "boxfill" : image) + "hover");
    }

    void OnMouseExit()
    {
        isHovering = false;
        setImage(GameCtrl.me.selected.entities.Contains(entity) ? "boxfill" : image);
    }

    void setImage(string image) 
    {
        sr.sprite = Resources.Load<Sprite>("Selection dropdown icons/" + image);
    }

    void OnMouseDown()
    {
        bool toggle = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
        if(!toggle){
			GameCtrl.me.selected.Clear();
        }
        if (GameCtrl.me.selected.ContainsKey(entity))
        {
            if (toggle)
            {
                GameCtrl.me.selected.Remove(entity);
            }
        }else{ select(); }
    }

	void select() {
        
		GameCtrl.me.selected[entity] = () => {
            if (this)
            {
                text.color = Color.white;
                setImage(image);
                this.transform.GetChild(0).transform.localPosition -= .1f * Vector3.back;
            }
		};
		text.color = Color.black;
        setImage("boxfill");
        this.transform.GetChild(0).transform.localPosition += .1f * Vector3.back;
	}
}
