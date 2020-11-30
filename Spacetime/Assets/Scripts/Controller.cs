using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Controller : MonoBehaviour
{
    public static Controller instance;
    float c = 1;
    public List<Observer> observers;
    [SerializeField]
    Observer startingFrame;
    Observer me;
    public TextMeshPro currentObserverText;
    public Transform cameraTransform;
    [HideInInspector]
    public bool simulationStarted = false;
    public float properTime = 0;

    void Start()
    {
        instance = this;
        SetRestFrame(startingFrame);
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

            observer.SetObservedTime(observer.observedTime + (Time.deltaTime / LorentzFactor(observer.velocity)));
            //observer.SetObservedTime(LorentzTransformTime(observer.velocity, GetPosition(me, observer), properTime));
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

    public void ResetSimulation()
    {
        SetRestFrame(startingFrame);
        foreach(Observer observer in observers)
        {
            observer.Reset();
        }
    }

    public void SetRestFrame(Observer newFrame)
    {
        if(newFrame == me)
        {
            return;
        }

        Debug.Log("---- SET REST FRAME : " + newFrame + "----");
        currentObserverText.text = "Current Observer:\n" + newFrame.name;

        /*Vector3 cameraPosition = cameraTransform.position;
        cameraPosition.x = newFrame.position;
        cameraTransform.position = cameraPosition;
        cameraTransform.parent = newFrame.transform;*/

        if(me == null)
        {
            me = newFrame;
            return;
        }

        int i = 0;
        List<float> newTimes = new List<float>();
        List<float> newPositions = new List<float>();
        List<float> newVelocities = new List<float>();

        for(i = 0; i < observers.Count; i ++)
        {
            float newVelocity = VelocityAddition(-newFrame.velocity, observers[i].velocity);
            newVelocities.Add(newVelocity);

            newTimes.Add(LorentzTransformTime(-newFrame.velocity, GetPosition(me, observers[i]), observers[i].observedTime));
            //newTimes.Add(LorentzTransformTime(newVelocity, GetPosition(newFrame, observers[i]), observers[i].observedTime));

            newPositions.Add(LorentzTransformPosition(-newFrame.velocity, GetPosition(me, observers[i]), observers[i].observedTime) - newFrame.position);
            //newPositions.Add(LorentzTransformPosition(newVelocity, GetPosition(newFrame, observers[i]), observers[i].observedTime));

            //newPositions.Add(LengthContraction(newFrame.velocity, GetPosition(me, observers[i])));
        }

        newFrame.SetPosition(0);

        for(i = 0; i < observers.Count; i ++)
        {
            Debug.Log("Set time " + observers[i].name + ": " + newTimes[i]);
            observers[i].SetObservedTime(newTimes[i]);
            Debug.Log("Set " + observers[i].name + " position relative to " + newFrame.name + ": " + newPositions[i]);
            SetPosition(newFrame, observers[i], newPositions[i]);
            Debug.Log("Set velocity " + observers[i].name + ": " + newVelocities[i]);
            observers[i].SetVelocity(newVelocities[i]);
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
        /*Vector3 newPosition = observed.transform.position;
        float newX = observer.transform.position.x + position;
        newPosition.x = newX;
        observed.transform.position = newPosition;*/
        float newPosition = observer.transform.position.x + position;
        observed.SetPosition(newPosition);
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
        //return LorentzFactor(velocity) * (time - (Beta(velocity) * position));
        //return (time / LorentzFactor(velocity)) + (Beta(velocity) * position);
        //return ((position/LorentzFactor(velocity)) + (Beta(velocity) * (time/LorentzFactor(velocity))))/(1 - Beta(velocity));
        return (time - (Beta(velocity) * position)) / LorentzFactor(velocity);
    }

    float LorentzTransformPosition(float velocity, float position, float time)
    {
        //return LorentzFactor(velocity) * (position - (Beta(velocity) * time));
        //return (position / LorentzFactor(velocity)) + (Beta(velocity) * time);
        //return ((time/LorentzFactor(velocity)) + (Beta(velocity) * (position/LorentzFactor(velocity))))/(1 - Beta(velocity));
        return (position - (Beta(velocity) * time)) / LorentzFactor(velocity);
    }

    float VelocityAddition(float firstFrameVelocity, float observedFrameVelocity)
    {
        return (observedFrameVelocity + firstFrameVelocity)/(1 + ((observedFrameVelocity * firstFrameVelocity)/Mathf.Pow(c, 2)));
    }

    /*float LengthContraction(float velocity, float position)
    {
        return position / LorentzFactor(velocity);
    }*/
}