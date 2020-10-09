using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    float c = 1;
    public List<Observer> observers;
    public Observer currentObserver;
    public Transform cameraTransform;
    float startTime;
    [HideInInspector]
    public bool simulationStarted = false;
    public float properTime {get{return Time.time - startTime;}}
    Animator animator;

    public void BeginSimulation()
    {
        startTime = Time.time;
        simulationStarted = true;
        //animator.Play("TwinParadox");
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        Vector3 cameraPosition = cameraTransform.position;
        cameraPosition.x = currentObserver.transform.position.x;
        cameraTransform.position = cameraPosition;
        cameraTransform.parent = currentObserver.transform;
        if(currentObserver.velocity != 0)
        {
            foreach(Observer observer in observers)
            {
                if(observer != currentObserver)
                {
                    observer.velocity -= currentObserver.velocity;
                }
            }
            currentObserver.velocity = 0;
        }
    }

    void Update()
    {
        if(!simulationStarted)
        {
            return;
        }
        foreach(Observer observer in observers)
        {
            if(observer == currentObserver)
            {
                observer.timer.text = properTime.ToString("F1");   
            }
            else
            {
                float distanceTraveled = observer.velocity * Time.deltaTime;
                Vector3 newObserverPosition = observer.transform.position;
                newObserverPosition.x += distanceTraveled;
                observer.transform.position = newObserverPosition;
                observer.timer.text = MovingTime(observer).ToString("F1");
            }
        }
    }

    public float LorentzFactor(float velocity)
    {
        return 1 / Mathf.Sqrt(1 - Mathf.Pow(velocity / c, 2) );
    }

    public float MovingTime(Observer observer)
    {
        float gamma = LorentzFactor(observer.velocity);
        float distance = observer.transform.position.x - currentObserver.transform.position.x;
        return gamma * (properTime - ( (observer.velocity * distance) / ( Mathf.Pow(c, 2) ) ) );
    }
}