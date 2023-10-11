using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    private Light lightobj;

    private float waitTime = 0;
    private float waitTime2 = 0;
    private float waitTime3 = 0;
    private float waitTimeSeizure = 0;
    private float multiplier;
    float turnOffTime;
    bool offTimer = false;
    bool seizureOver = false;
    private void Start()
    {
        multiplier = Random.Range(1f, 2f);
        lightobj = GetComponent<Light>();
        turnOffTime = Random.Range(1f, 4f);
        waitTimeSeizure = Random.Range(2f, 5f);
    }

    private void Update()
    {
        // For Seizure lightswitch
        waitTime3 += Time.deltaTime;
        if (waitTime3 > waitTimeSeizure)
        {
            Seizure();
            if (seizureOver)
            {
                seizureOver = false;
                waitTime3 = 0;
                waitTimeSeizure = Random.Range(2f, 5f);
            }
        }


        waitTime2 += Time.deltaTime;
        if (waitTime2 > turnOffTime && !offTimer)
        {
            offTimer = true;
            lightobj.enabled = false;
            waitTime2 = 0;
            turnOffTime = Random.Range(1f, 4f);
        }
        if (offTimer && waitTime2 > turnOffTime) {
            SetLightOn();
            offTimer = false;
        }
    }

    private void Seizure()
    {
        waitTime += Time.deltaTime;
        float seizureTime = 0.15f;

        if (waitTime > seizureTime)
        {
            lightobj.enabled = false;
            seizureTime *= multiplier;
        }
        if (waitTime > seizureTime)
        {
            SetLightOn();
            seizureTime *= multiplier;
        }
        if (waitTime > seizureTime)
        {
            lightobj.enabled = false;
            seizureTime *= multiplier;
        }
        if (waitTime > seizureTime)
        {
            SetLightOn();
            waitTime = 0;
            seizureOver = true;
        }
    }

    private void SetLightOn()
    {
        // Play Sound
        lightobj.enabled = true;
    }
}
