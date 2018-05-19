using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective : MonoBehaviour {

    [SerializeField]
    private float protectionRange = 1;
    private bool exposed;
    private int guardians = 0;
    [SerializeField]
    GameObject exposedHighlight;

    private void Start () {
        exposed = false;
        exposedHighlight.SetActive(true);
    }

    void Update () {
        DetectGuards();
    }

    private void DetectGuards()
    {
        guardians = GetNumberOfGuardians(protectionRange);
        if (guardians == 0)
        {
            exposed = true;
            //AlertAI();
            exposedHighlight.SetActive(false);
        }
        else
        {
            exposed = false;
            exposedHighlight.SetActive(true);
        }
    }


    protected int GetNumberOfGuardians(float protectionRange)
    {
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("Enemy");
        int numberOfGuardians = objectsWithTag.Length;
        for (int i = 0; i < objectsWithTag.Length; i++)
        {
            print(Vector3.Distance(transform.position, objectsWithTag[i].transform.position));
            if (Vector3.Distance(transform.position, objectsWithTag[i].transform.position) > protectionRange)
            {
                numberOfGuardians--;
            }
        }
        return numberOfGuardians;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ally") && exposed)
        {
            this.gameObject.SetActive(false);
            //AlertAI();
        }
    }

}
