using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecisionTree : MonoBehaviour {

    DecisionTree instance;

    private static bool playerDetectedAlarm, objectiveStolenAlarm, objectiveExposedAlarm;
    private static int remainingObjectives;

	void Start () {
		
	}
	
	//void Update () {
 //       Decide();
	//}

 //   public DecisionTree GetInstance()
 //   {
 //       if (instance == null)
 //       {
 //           instance = new DecisionTree();
 //       }
 //           return instance;
 //   }

 //   private void Decide()
 //   {
 //       if (playerDetectedAlarm)
 //       {
 //           playerDetectedDecision();
 //           playerDetectedAlarm = false;
 //           return;
 //       }
 //       else if (objectiveStolenAlarm)
 //       {
 //           ObjectiveStolenDecision();
 //           objectiveStolenAlarm = false;
 //           return;
 //       }
 //       else if (objectiveExposedAlarm)
 //       {
 //           ObjectiveExposedDecision();
 //           objectiveExposedAlarm = false;
 //           return;
 //       }
 //       return;
 //   }

 //   private void playerDetectedDecision()
 //   {
 //       if (availableAttackers > 0)
 //       {
 //           SendAttacker(detectedPosition);
 //           return;
 //       }
 //       else return;
 //   }

 //   private void ObjectiveStolenDecision()
 //   {   
 //       if (remainingObjectives > 0)
 //       {
 //           RelocateUnitsDecision();
 //           return;
 //       }
 //       else
 //       {
 //           Concede();
 //           return;
 //       }
 //   }

 //   private void RelocateUnitsDecision()
 //   {
 //       if (unitsAtStolenObjective > 0)
 //       {
 //           RelocateUnits();
 //           return;
 //       }
 //       else return;
 //   }

 //   private void ObjectiveExposedDecision()
 //   {
 //       if (availableReinforcements > 0)
 //       {
 //           SendReinforcements();
 //           return;
 //       }
 //       else
 //       {
 //           ReinforcementsFromRemainingObjectivesDecision();
 //           return;                            
 //       }
 //   }

 //   private ReinforcementsFromRemainingObjectivesDecision()
 //   {
 //       foreach (Objective in objectiveStolenAlarm{
 //           if (guardiansAtObjective > 1)
 //           {
 //               RelocateGuardian();
 //               return;
 //           }
 //       }
 //       return;
 //   }

}
