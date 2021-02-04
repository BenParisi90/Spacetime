using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Observer : MonoBehaviour
{
    public TextMeshPro timeText;
    public TextMeshPro velocityText;
    public TextMeshPro positionText;

    public float velocity;
    public float observedTime;
    public float position {get{return transform.position.x;}}

    public float startVelocity;
    public float startObservedTime;
    public float startPosition;

    SpriteRenderer sr;

    public void SetVelocity(float newVelocity)
    {
        velocity = newVelocity;
        velocityText.text = "V=" + velocity.ToString(Controller.ROUNDING_RULE);
    }

    public void SetTime(float newObservedTime)
    {
        observedTime = newObservedTime;
        timeText.text = "T=" + observedTime.ToString(Controller.ROUNDING_RULE);   
    }

    public void SetPosition(float newPosition)
    {
        Vector3 currentPosition = transform.position;
        currentPosition.x = newPosition;
        transform.position = currentPosition;
        positionText.text = "X=" + transform.position.x.ToString(Controller.ROUNDING_RULE);
    }

    void Start()
    {
        sr = transform.GetChild(0).GetComponent<SpriteRenderer>();
        SetVelocity(velocity);
        SetTime(observedTime);
        SetPosition(transform.position.x);
        startVelocity = velocity;
        startObservedTime = observedTime;
        startPosition = position;
    }

    public void Reset()
    {
        SetVelocity(startVelocity);
        SetTime(startObservedTime);
        SetPosition(startPosition);
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
        Controller.instance.SetRestFrame(this);
    }
}
