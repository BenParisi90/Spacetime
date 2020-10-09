using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Observer : MonoBehaviour
{
    public TextMeshPro timer;
    [Range(-.99f, .99f)]
    public float velocity;
    public float previousMovingTime = 0;
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
}
