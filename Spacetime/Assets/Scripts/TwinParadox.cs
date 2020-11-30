using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TwinParadox : MonoBehaviour
{
    public Observer home;
    public Observer rocket;
    public Observer destination;

    bool simulationStarted = false;
    bool destinationReached = false;

    [SerializeField]
    TextMeshPro arrivalText;

    void Start()
    {
        arrivalText.text = "";
    }

    void Update()
    {
        if(destinationReached)
        {
            return;
        }
        if(rocket.transform.position.x >= destination.transform.position.x)
        {
            Debug.Log("-----Reach destination!-----");
            Debug.Log("home: " + home.observedTime);
            Debug.Log("rocket: " + rocket.observedTime);
            Debug.Log("destination: " + destination.observedTime);
            arrivalText.text = "Arrival:\nrocket:" + rocket.observedTime.ToString("F2") + "\ndestination:" + destination.observedTime.ToString("F2");
            destinationReached = true;
        }
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

    public void Reset()
    {
        simulationStarted = false;
        destinationReached = false;
        arrivalText.text = "";
    }

    IEnumerator TwinParadoxCoroutine()
    {
        while(rocket.transform.position.x < destination.transform.position.x)
        {
            yield return null;
        }
        
    }
}
