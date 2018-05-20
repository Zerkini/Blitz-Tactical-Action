using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecisionTree : MonoBehaviour {

    private static DecisionTree instance;

    private static bool playerDetectedAlarm, objectiveStolenAlarm, objectiveExposedAlarm;
    private static int remainingObjectives, exposedObjectiveId;
    private static Vector2 detectedPlayerPosition;

    [SerializeField]
    private SpecialChaser specialChaser1, specialChaser2, specialChaser3;
    [SerializeField]
    private SpecialSentinel specialSentinel1, specialSentinel2, specialSentinel3;
    [SerializeField]
    private Objective objective1, objective2, objective3;
    private List<SpecialChaser> specialChasers = new List<SpecialChaser>();
    private List<SpecialSentinel> specialSentinels = new List<SpecialSentinel>();
    private List<Objective> objectives = new List<Objective>();


    private int availableSpecialChasers, availableSpecialSentinels;

	private void Start () {
        CountSentinelsAndChasers();
        CountObjectives();
        instance = this;
    }

    void Update()
    {
        Decide();
    }

    public DecisionTree GetInstance()
    {
        if (instance == null)
        {
            instance = this;
        }
        return instance;
    }

    private void Decide()
    {
        if (playerDetectedAlarm)
        {
            playerDetectedDecision();
            instance.SetPlayerDetectedAlarm(false);
            return;
        }
        //    else if (objectiveStolenAlarm)
        //    {
        //        ObjectiveStolenDecision();
        //        objectiveStolenAlarm = false;
        //        return;
        //    }
        //else 
        if (objectiveExposedAlarm)
        {
            ReinforcementsDecision();
            instance.SetObjectiveExposedAlarm(false);
            return;
        }
        return;
    }

    private void playerDetectedDecision()
    {
        if (availableSpecialChasers > 0)
        {
            if (availableSpecialChasers > 0 && detectedPlayerPosition != null)
            {
                SpecialChaser currentSpecialChaser = specialChasers[availableSpecialChasers - 1];
                currentSpecialChaser.standingBy = false;
                currentSpecialChaser.moveToPosition(detectedPlayerPosition);
                availableSpecialChasers--;
                return;
            }
            return;
        }
        else return;
    }

    //private void ObjectiveStolenDecision()
    //{
    //    if (remainingObjectives > 0)
    //    {
    //        ReinforcementsDecision();
    //        return;
    //    }
    //    else
    //    {
    //        //Concede();
    //        return;
    //    }
    //}


    private void ReinforcementsDecision()
    {
        Objective reinforcedObjective = objectives[exposedObjectiveId];
        if (availableSpecialSentinels > 0 && !reinforcedObjective.reinforced)
        {
            SpecialSentinel currentSpecialSentinel = specialSentinels[availableSpecialSentinels - 1];
            currentSpecialSentinel.standingBy = false;
            currentSpecialSentinel.moveToPosition(reinforcedObjective.reinforcementPoint.position, reinforcedObjective.reinforcementPatrolPoint.position);
            reinforcedObjective.reinforced = true;
            availableSpecialSentinels--;
            return;
        }
        else return;
    }

    private void CountSentinelsAndChasers()
    {

        if (specialChaser1 != null && specialChaser1.standingBy)
        {
            specialChasers.Add(specialChaser1);
        }
        if (specialChaser2 != null && specialChaser2.standingBy)
        {
            specialChasers.Add(specialChaser2);
        }
        if (specialChaser3 != null && specialChaser3.standingBy)
        {
            specialChasers.Add(specialChaser3);
        }
        availableSpecialChasers = specialChasers.Count;

        if (specialSentinel1 != null && specialSentinel1.standingBy)
        {
            specialSentinels.Add(specialSentinel1);
        }
        if (specialSentinel2 != null && specialSentinel2.standingBy)
        {
            specialSentinels.Add(specialSentinel2);
        }
        if (specialSentinel3 != null && specialSentinel3.standingBy)
        {
            specialSentinels.Add(specialSentinel3);
        }
        availableSpecialSentinels = specialSentinels.Count;
    }

    private void CountObjectives()
    {
        if (objective1 != null)
        {
            objectives.Add(objective1);
        }
        if (objective2 != null)
        {
            objectives.Add(objective2);
        }
        if (objective3 != null)
        {
            objectives.Add(objective3);
        }
        remainingObjectives = objectives.Count;
    }

    public static void ExposedAlert(int objectiveId)
    {
        instance.SetObjectiveExposedAlarm(true);
        instance.SetObjectiveExposedId(objectiveId);
    }

    public static void PlayerDetectedAlert(Vector2 position)
    {
        instance.SetPlayerDetectedAlarm(true);
        instance.SetDetectedPlayerPosition(position);
    }

    private void SetObjectiveExposedAlarm(bool alarm)
    {
        objectiveExposedAlarm = alarm;
    }

    private void SetObjectiveExposedId(int objectiveId)
    {
        exposedObjectiveId = objectiveId;
    }

    private void SetDetectedPlayerPosition(Vector2 detectedPosition)
    {
        detectedPlayerPosition = detectedPosition;
    }

    private void SetPlayerDetectedAlarm(bool alarm){
        playerDetectedAlarm = alarm;
    }

}

