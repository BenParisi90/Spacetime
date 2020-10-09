using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public static Controller instance;
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
        instance = this;
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

    public float LorentzFactor(float velocity)
    {
        return 1 / (Mathf.Sqrt(1 - ( Mathf.Pow(velocity, 2) / Mathf.Pow(c, 2) ) ) );
    }

    public float MovingTime(Observer observer)
    {
        float gamma = LorentzFactor(observer.velocity);
        float distance = observer.transform.position.x - currentObserver.transform.position.x;
        return gamma * (properTime - ( (observer.velocity * distance) / ( Mathf.Pow(c, 2) ) ) );
    }
}