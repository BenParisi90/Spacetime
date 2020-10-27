using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
    public static Controller instance;
    float c = 1;
    public List<Observer> observers;
    public Observer me;
    public Text currentObserverText;
    public Transform cameraTransform;
    [HideInInspector]
    public bool simulationStarted = false;
    public float properTime = 0;

    void Start()
    {
        instance = this;
        SetReferenceFrame(me);
    }

    void Update()
    {
        if(!simulationStarted)
        {
            return;
        }
        properTime += Time.deltaTime;
        foreach(Observer observer in observers)
        {
            if(observer == me)
            {
                observer.observedTime = properTime;
            }
            else
            {
                observer.observedTime = LorentzTransformTime(observer.velocity, GetPosition(observer), properTime);

                Vector3 newPosition = observer.transform.position;
                newPosition.x += observer.velocity * Time.deltaTime;
                observer.transform.position = newPosition;
            }
        }
    }

    public void BeginSimulation()
    {
        simulationStarted = true;
    }

    public void PauseSimulation()
    {
        simulationStarted = false;
    }

    public void SetReferenceFrame(Observer target)
    {
        currentObserverText.text = "Current Observer: " + target.name;

        Vector3 cameraPosition = cameraTransform.position;
        cameraPosition.x = target.position;
        cameraTransform.position = cameraPosition;
        cameraTransform.parent = target.transform;

        float targetVelocity = target.velocity;
        me = target;

        foreach(Observer observer in observers)
        {
            observer.velocity -= targetVelocity;
            observer.observedTime = LorentzTransformTime(observer.velocity, GetPosition(observer), properTime);
        }

        
    }

    public void ChangeVelocity(Observer target, float delta)
    {
        if(target != me)
        {
            target.velocity += delta;
        }
        else
        {
            foreach(Observer observer in observers)
            {
                if(observer != me)
                {
                    observer.velocity -= delta;
                }
            }
        }
    }

    void SetPosition(Observer target, float position)
    {
        Vector3 newPosition = target.transform.position;
        newPosition.x = me.transform.position.x + position;
        target.transform.position = newPosition;
    }

    float GetPosition(Observer target)
    {
        return target.transform.position.x - me.transform.position.x;
    }

    float Beta(float velocity)
    {
        return velocity/c;
    }

    float LorentzFactor(float velocity)
    {
        return 1 / Mathf.Sqrt(1 - Mathf.Pow(Beta(velocity), 2) );
    }

    float LorentzTransformTime(float velocity, float position, float time)
    {
        return LorentzFactor(velocity) * (time -(Beta(velocity) * position));
    }

    float LorentzTransformPosition(float velocity, float position, float time)
    {
        return LorentzFactor(velocity) * (position - (Beta(velocity) * time));
    }
}