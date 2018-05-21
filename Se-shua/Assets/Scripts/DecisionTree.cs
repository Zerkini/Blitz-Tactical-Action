using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DecisionTree : MonoBehaviour {

    private static DecisionTree instance;

    private static bool playerDetectedAlarm, objectiveStolenAlarm, objectiveExposedAlarm, playerDestroyedAlarm, gameEnded = false;
    private static int remainingObjectives, exposedObjectiveId, stolenObjectiveId, remainingPlayers;
    private static Vector2 detectedPlayerPosition;

    [SerializeField]
    private SpecialChaser specialChaser1, specialChaser2, specialChaser3;
    [SerializeField]
    private SpecialSentinel specialSentinel1, specialSentinel2, specialSentinel3;
    [SerializeField]
    private Objective objective1, objective2, objective3;
    [SerializeField]
    private PlayerAlly player1, player2, player3;
    [SerializeField]
    private GameObject gameOverText, victoryText;
    private SpecialChaser[] specialChasersArray;
    private SpecialSentinel[] specialSentinelsArray;
    private Objective[] objectivesArray;
    private PlayerAlly[] playersArray;
    private List<SpecialChaser> specialChasers;
    private List<SpecialSentinel> specialSentinels;
    private List<Objective> objectives;
    private List<PlayerAlly> players;
    private int availableSpecialChasers, availableSpecialSentinels;

    private void Start() {
        players = new List<PlayerAlly>();
        objectives = new List<Objective>();
        specialChasers = new List<SpecialChaser>();
        specialSentinels = new List<SpecialSentinel>();

        playersArray = GetComponents<PlayerAlly>();
        objectivesArray = GetComponents<Objective>();
        specialChasersArray = GetComponents<SpecialChaser>();
        specialSentinelsArray = GetComponents<SpecialSentinel>();
        
        players = playersArray.ToList();
        objectives = objectivesArray.ToList();
        specialChasers = specialChasersArray.ToList();
        specialSentinels = specialSentinelsArray.ToList();

        CountSentinelsAndChasers();
        CountObjectives();
        CountPlayers();
        gameEnded = false;
        instance = this;
    }

    void Update()
    {
        if (!gameEnded) {
            Decide();
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                instance.SetGameEnded(false);
                SceneManager.LoadScene(0);
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                instance.SetGameEnded(false);
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
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
        if (playerDestroyedAlarm)
        {
            instance.PlayerDestroyedDecision();
            return;
        }
        else if (playerDetectedAlarm)
        {
            instance.playerDetectedDecision();
            instance.SetPlayerDetectedAlarm(false);
            return;
        }
        else if (objectiveStolenAlarm)
        {
            instance.ObjectiveStolenDecision();
            instance.SetObjectiveStolenAlarm(false);
            return;
        }
        else if (objectiveExposedAlarm)
        {
            instance.ExposedReinforcementsDecision();
            instance.SetObjectiveExposedAlarm(false);
            return;
        }
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

    private void ObjectiveStolenDecision()
    {
        if (remainingObjectives > 0)
        {
            StolenReinforcementsDecision();
            return;
        }
        else
        {
            Concede();
            return;
        }
    }

    private void ExposedReinforcementsDecision()
    {
        Objective reinforcedObjective = objectives[exposedObjectiveId];
        if (availableSpecialSentinels > 0 && !reinforcedObjective.reinforced)
        {
            SpecialSentinel currentSpecialSentinel = specialSentinels[availableSpecialSentinels - 1];
            currentSpecialSentinel.standingBy = false;
            currentSpecialSentinel.moveToPosition(reinforcedObjective.reinforcementPoint.position, reinforcedObjective.reinforcementPatrolPoint.position);
            reinforcedObjective.reinforced = true;
            availableSpecialSentinels--;
        }
    }

    private void StolenReinforcementsDecision()
    {
        Objective stolenObjective = objectives[stolenObjectiveId];
        objectives.RemoveAt(stolenObjectiveId);
        foreach (Objective objective in objectives) {
            if (availableSpecialSentinels > 0 && !objective.reinforced)
            {
                SpecialSentinel currentSpecialSentinel = specialSentinels[availableSpecialSentinels - 1];
                currentSpecialSentinel.standingBy = false;
                currentSpecialSentinel.moveToPosition(objective.reinforcementPoint.position, objective.reinforcementPatrolPoint.position);
                objective.reinforced = true;
                availableSpecialSentinels--;
            }
        }
    }

    private void PlayerDestroyedDecision()
    {
        if (remainingPlayers < 1)
        {
            AnnounceVictory();
        }
        return;
    }

    private void AnnounceVictory()
    {
        gameOverText.SetActive(true);
        instance.SetGameEnded(true);
    }
    private void Concede()
    {
        victoryText.SetActive(true);
        instance.SetGameEnded(true);
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

    private void CountPlayers()
    {
        if (player1 != null)
        {
            players.Add(player1);
        }
        if (player2 != null)
        {
            players.Add(player2);
        }
        if (player3 != null)
        {
            players.Add(player3);
        }
        remainingPlayers = players.Count;
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

    public static void ObjectiveStolenAlert(int objectiveId)
    {
        instance.SetObjectiveStolenAlarm(true);
        instance.SetStolenObjectiveId(objectiveId);
        instance.DecrementRemainingObjectives();
    }

    public static void PlayerDestroyedAlert()
    {
        instance.DecrementRemainingPlayers();
        instance.SetPlayerDestroyedAlarm(true);
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

    private void SetObjectiveStolenAlarm(bool alarm)
    {
        objectiveStolenAlarm = alarm;
    }

    private void SetStolenObjectiveId(int objectiveId)
    {
        stolenObjectiveId = objectiveId;
    }
    private void DecrementRemainingObjectives()
    {
        remainingObjectives--;
    }
    private void DecrementRemainingPlayers()
    {
        remainingPlayers--;
    }

    private void SetPlayerDestroyedAlarm(bool alarm)
    {
        playerDestroyedAlarm = alarm;
    }

    private void SetGameEnded(bool end)
    {
        gameEnded = end;
    }
}

