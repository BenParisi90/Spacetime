using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Controller : MonoBehaviour
{
    public static string ROUNDING_RULE = "F2";
    public static Controller instance;
    float c = 1;
    public List<Observer> observers;
    [SerializeField]
    Observer startingFrame;
    Observer me;
    public TextMeshPro restFrameText;
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
            float newPosition = observer.transform.position.x + (observer.velocity * Time.deltaTime);
            observer.SetPosition(newPosition);

            observer.SetTime(observer.observedTime + (Time.deltaTime / LorentzFactor(observer.velocity)));
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
        properTime = 0;
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

        UnityEngine.Debug.Log("---- SET REST FRAME : " + newFrame + "----");
        restFrameText.text = "Rest Frame:\n" + newFrame.name;

        /*Vector3 cameraPosition = cameraTransform.position;
        cameraPosition.x = newFrame.position;
        cameraTransform.position = cameraPosition;
        cameraTransform.parent = newFrame.transform;*/

        if(me == null)
        {
            me = newFrame;
            return;
        }

        float newFramePosition = 0;
        float newFrameTime = 0;
        int i = 0;
        List<float> newTimes = new List<float>();
        List<float> newPositions = new List<float>();
        List<float> newVelocities = new List<float>();

        for(i = 0; i < observers.Count; i ++)
        {
            float newVelocity = NewVelocity(observers[i].velocity, newFrame.velocity);
            //float newVelocity = VelocityAddition(-newFrame.velocity, observers[i].velocity);
            newVelocities.Add(newVelocity);

            //newTimes.Add(LorentzTransformTime(newFrame.velocity, GetPosition(me, observers[i]), observers[i].observedTime));
            //newTimes.Add(LorentzTransformTime(newVelocity, GetPosition(newFrame, observers[i]), observers[i].observedTime));
            float newTime = NewTime(newFrame.velocity, observers[i].position, observers[i].observedTime);
            newTimes.Add(newTime);
            if(observers[i] == newFrame)
            {
                newFrameTime = newTime;
            }

            //newPositions.Add(LorentzTransformPosition(newFrame.velocity, GetPosition(me, observers[i]), observers[i].observedTime) - newFrame.position);
            //newPositions.Add(LorentzTransformPosition(newVelocity, GetPosition(newFrame, observers[i]), observers[i].observedTime));
            float newPosition = NewPosition(newFrame.velocity, observers[i].position, newTime);
            newPositions.Add(newPosition);
            if(observers[i] == newFrame)
            {
                newFramePosition = newPosition;
            }

            //newPositions.Add(LengthContraction(newFrame.velocity, GetPosition(me, observers[i])));
        }

        float positionOffset = newFramePosition;
        float timeOffset = newFrameTime - properTime;
        newFrame.SetPosition(0);
        me = newFrame;
        properTime = newFrame.observedTime;

        /*Debug.Log("NewTime = " + newTimes);
        Debug.Log("NewPositions = " + newPositions);*/

        for(i = 0; i < observers.Count; i ++)
        {
            UnityEngine.Debug.Log("Set time " + observers[i].name + ": " + (newTimes[i] - timeOffset));
            observers[i].SetTime(newTimes[i] - timeOffset);
            UnityEngine.Debug.Log("Set " + observers[i].name + " position relative to " + newFrame.name + ": " + (newPositions[i] - newFramePosition));
            SetPosition(newFrame, observers[i], (newPositions[i] - newFramePosition));
            UnityEngine.Debug.Log("Set velocity " + observers[i].name + ": " + newVelocities[i]);
            observers[i].SetVelocity(newVelocities[i]);
        }

        
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
        return (time + (Beta(velocity) * position)) / LorentzFactor(velocity);
    }

    float LorentzTransformPosition(float velocity, float position, float time)
    {
        //return LorentzFactor(velocity) * (position - (Beta(velocity) * time));
        //return (position / LorentzFactor(velocity)) + (Beta(velocity) * time);
        //return ((time/LorentzFactor(velocity)) + (Beta(velocity) * (position/LorentzFactor(velocity))))/(1 - Beta(velocity));
        return (position + (Beta(velocity) * time)) / LorentzFactor(velocity);
    }

    float VelocityAddition(float firstFrameVelocity, float observedFrameVelocity)
    {
        return (observedFrameVelocity + firstFrameVelocity)/(1 + ((observedFrameVelocity * firstFrameVelocity)/Mathf.Pow(c, 2)));
    }

    float NewVelocity(float oldVelocity, float newFrameVelocity)
    {
        return (oldVelocity - newFrameVelocity)/(1 - ((oldVelocity * newFrameVelocity)/(Mathf.Pow(c, 2))));
    }

    //what I think the new frames time will be based on how far away and the rockets proper time
    float NewTime(float velocity, float position, float time)
    {
        return (time / LorentzFactor(velocity)) + (Beta(velocity) * position);
    }

    float NewPosition(float velocity, float position, float time)
    {
        return LorentzFactor(velocity) * (position - (Beta(velocity) * time));
    }

    /*float LengthContraction(float velocity, float position)
    {
        return position / LorentzFactor(velocity);
    }*/
}