using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective : MonoBehaviour {

    [SerializeField]
    private float protectionRange = 1;
    [SerializeField]
    public int id;
    [SerializeField]
    GameObject exposedHighlight;
    [SerializeField]
    public Transform reinforcementPoint, reinforcementPatrolPoint;
    private bool exposed;
    public bool reinforced = false, stolen = false;
    private int guardians = 0;
    

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
            ExposedAlert();
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
            DecisionTree.ObjectiveStolenAlert(id);
            stolen = true;
            Destroy(gameObject);
        }
    }

    private void ExposedAlert()
    {
        if (!reinforced)
        {
            DecisionTree.ExposedAlert(id);
        }
    }


}
