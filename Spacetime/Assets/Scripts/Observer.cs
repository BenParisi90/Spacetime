using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Observer : MonoBehaviour
{
    public TextMeshPro timer;
    [Range(-.99f, .99f)]
    public float velocity;

    void Update()
    {
        if(!Controller.instance.simulationStarted)
        {
            return;
        }
        if(this == Controller.instance.currentObserver)
        {
            timer.text = Controller.instance.properTime.ToString("F1");   
        }
        else
        {
            float distanceTraveled = velocity * Time.deltaTime;
            Vector3 newObserverPosition = transform.position;
            newObserverPosition.x += distanceTraveled;
            transform.position = newObserverPosition;
            timer.text = Controller.instance.MovingTime(this).ToString("F1");
        }
    }
}
