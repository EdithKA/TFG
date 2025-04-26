using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class PS2Controller : MonoBehaviour
{
    // Start is called before the first frame update

    public Light led;
    void Start()
    {
        led.color = Color.red;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void insertDVD(string dvdName)
    {
        if(dvdName == "Crash")
        {
            Debug.Log("Crash Bandicoot inserted");
            led.color = Color.green;
        }
    }
}
