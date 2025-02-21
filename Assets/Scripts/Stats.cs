using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Stats : MonoBehaviour
{
    int sanity;
    int health;

    float decreaseInterval = 3f;


    //Marcadores - UI
    public TextMeshProUGUI sanityCounter;
    public TextMeshProUGUI healthCounter;


    private void Start()
    {
        sanity = 100;
        health = 100;

        StartCoroutine(DecreaseSanity());

    }

    private void Update()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        sanityCounter.text = sanity.ToString();
        healthCounter.text = health.ToString();
    }


    IEnumerator DecreaseSanity()
    {
        while (true)
        { 
            yield return new WaitForSeconds(decreaseInterval); 

            if (sanity > 0)
            {
                sanity--;
            }
            else if (health > 0) 
            {
                health--;
            }
        }
    }
}
