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
    public float observedTime;
}
