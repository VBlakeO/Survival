using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class InterpolationController : MonoBehaviour
{
    private float[] lastFixedUpdateTimes;
    private int newTimeIndex;

    private static float  InterpolationFactor;

    public static float interpolationFactor
    {
        get{ return InterpolationFactor;}
    }

    void Start()
    {
        lastFixedUpdateTimes = new float[2];
        newTimeIndex = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        newTimeIndex = OldTimeIndex();
        lastFixedUpdateTimes[newTimeIndex] = Time.fixedTime;
    }

    private void Update() {
        float newerTime = lastFixedUpdateTimes[newTimeIndex];
        float olderTime = lastFixedUpdateTimes[OldTimeIndex()];

        if(newerTime != olderTime)
        {
            InterpolationFactor = (Time.time - newerTime) / (newerTime - olderTime);
        }
        else
        {
            InterpolationFactor = 1;
        }
    }

    private int OldTimeIndex()
    {
        return (newTimeIndex == 0 ? 1 : 0);
    }
}
