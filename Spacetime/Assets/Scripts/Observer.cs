using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Observer : MonoBehaviour
{
    public TextMeshPro timer;
    public float velocity;
    float _observedTime;
    public float observedTime
    {
        get
        {
            return _observedTime;
        }
        set
        {
            _observedTime = value;
            timer.text = _observedTime.ToString("F1");   
        }
    }
    public float position {get{return transform.position.x;}}

    SpriteRenderer sr;

    void Start()
    {
        sr = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    void OnMouseEnter()
    {
        sr.color = new Color(.5f, 1, .5f);
    }

    void OnMouseExit()
    {
        sr.color = new Color(1, 1, 1);
    }

    void OnMouseDown()
    {
        Controller.instance.SetReferenceFrame(this);
    }
}
