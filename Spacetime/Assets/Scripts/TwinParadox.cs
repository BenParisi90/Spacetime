using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwinParadox : MonoBehaviour
{
    public Observer home;
    public Observer rocket;
    public Observer destination;

    bool simulationStarted = false;

    void Start()
    {
        //Controller.instance.ChangeVelocity(rocket, .9f);
        //Controller.instance.ChangeVelocity(destination, .9f);
    }

    public void BeginSimulation()
    {
        if(simulationStarted)
        {
            return;
        }
        simulationStarted = true;
        StartCoroutine(TwinParadoxCoroutine());
    }

    IEnumerator TwinParadoxCoroutine()
    {
        while(rocket.transform.position.x < destination.transform.position.x)
        {
            yield return null;
        }
        Debug.Log("Reach destination!");
        Debug.Log("home: " + home.observedTime);
        Debug.Log("rocket: " + rocket.observedTime);
        Debug.Log("destination: " + destination.observedTime);
    }
}
