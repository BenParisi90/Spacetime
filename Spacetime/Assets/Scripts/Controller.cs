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
        SetRestFrame(me);
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
            Vector3 newPosition = observer.transform.position;
            newPosition.x += observer.velocity * Time.deltaTime;
            observer.transform.position = newPosition;

            //observer.observedTime += Time.deltaTime / LorentzFactor(observer.velocity);
            observer.observedTime = LorentzTransformTime(observer.velocity, GetPosition(me, observer), properTime);
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

    public void SetRestFrame(Observer newFrame)
    {
        Debug.Log("---- SET REST FRAME : " + newFrame + "----");
        currentObserverText.text = "Current Observer: " + newFrame.name;

        Vector3 cameraPosition = cameraTransform.position;
        cameraPosition.x = newFrame.position;
        cameraTransform.position = cameraPosition;
        cameraTransform.parent = newFrame.transform;

        //float targetVelocity = newFrame.velocity;

        int i = 0;
        List<float> newTimes = new List<float>();
        List<float> newPositions = new List<float>();
        List<float> newVelocities = new List<float>();

        for(i = 0; i < observers.Count; i ++)
        {
            newTimes.Add(LorentzTransformTime(newFrame.velocity, GetPosition(me, observers[i]), properTime));
            newPositions.Add(LorentzTransformPosition(newFrame.velocity, GetPosition(me, observers[i]), properTime));
            newVelocities.Add(VelocityAddition(-newFrame.velocity, observers[i].velocity));
        }

        for(i = 0; i < observers.Count; i ++)
        {
            Debug.Log("Set time " + observers[i].name + ": " + newTimes[i]);
            observers[i].observedTime = newTimes[i];
            Debug.Log("Set " + observers[i].name + " position relative to " + me.name + ": " + newPositions[i]);
            SetPosition(me, observers[i], newPositions[i]);
            Debug.Log("Set velocity " + observers[i].name + ": " + newVelocities[i]);
            observers[i].velocity = newVelocities[i];
        }

        me = newFrame;
        properTime = newFrame.observedTime;
    }

    /*public void ChangeVelocity(Observer target, float delta)
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
    }*/

    void SetPosition(Observer observer, Observer observed, float position)
    {
        Vector3 newPosition = observed.transform.position;
        float newX = observer.transform.position.x + position;
        newPosition.x = newX;
        observed.transform.position = newPosition;
    }

    float GetPosition(Observer observer, Observer observed)
    {
        float relativePosition = observed.transform.position.x - observer.transform.position.x;
        //Debug.Log("Position " + observed.name + " relative to " + observer.name + " = " + relativePosition);        
        return relativePosition;
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

    float VelocityAddition(float firstFrameVelocity, float observedFrameVelocity)
    {
        return (observedFrameVelocity + firstFrameVelocity)/(1 + ((observedFrameVelocity * firstFrameVelocity)/Mathf.Pow(c, 2)));
    }
}