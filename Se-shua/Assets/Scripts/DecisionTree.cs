﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DecisionTree : MonoBehaviour {

    #region static fields
    private static DecisionTree instance;
    private static bool playerDetectedAlarm, objectiveStolenAlarm, objectiveExposedAlarm, playerDestroyedAlarm, gameEnded = false, playerWon;
    private static int remainingObjectives, exposedObjectiveId, stolenObjectiveId, remainingPlayers, remainingEnemies, playerScoreForObjectives = 0, AIScoreForObjectives = 150;
    private static Vector2 detectedPlayerPosition;
    #endregion

    #region serialized fields
    [SerializeField]
    private SpecialChaser specialChaser1, specialChaser2, specialChaser3;
    [SerializeField]
    private SpecialSentinel specialSentinel1, specialSentinel2, specialSentinel3;
    [SerializeField]
    private Objective objective1, objective2, objective3;
    [SerializeField]
    private PlayerAlly player1, player2, player3;
    [SerializeField]
    private GameObject gameOverText, victoryText, Unit1HP, Unit2HP, Unit3HP;
    [SerializeField]
    private int startingEnemiesNumber;
    #endregion

    #region arrays and lists
    private SpecialChaser[] specialChasersArray;
    private SpecialSentinel[] specialSentinelsArray;
    private Objective[] objectivesArray;
    private PlayerAlly[] playersArray;
    private List<GameObject> playerHPTexts;
    private List<SpecialChaser> specialChasers;
    private List<SpecialSentinel> specialSentinels;
    private List<Objective> objectives;
    private List<PlayerAlly> players;
    #endregion
    private int availableSpecialChasers, availableSpecialSentinels;

    private void Start() {
        players = new List<PlayerAlly>();
        objectives = new List<Objective>();
        specialChasers = new List<SpecialChaser>();
        specialSentinels = new List<SpecialSentinel>();
        playerHPTexts = new List<GameObject>();

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
        CountHP();
        remainingEnemies = startingEnemiesNumber;


        gameEnded = false;
        instance = this;
    }

    void Update()
    {
        UpdateHP();
        if (!gameEnded) {
            Decide();
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                instance.SetGameEnded(false);
                Application.Quit();
            }
            //else if (Input.GetKeyDown(KeyCode.R))
            //{
            //    instance.SetGameEnded(false);
            //    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            //}
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
            instance.SetPlayerDestroyedAlarm(false);
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
        int playerRewardForStealingObjective = 50;
        for (int i = 1; i < remainingPlayers; i++)
        {
            playerRewardForStealingObjective += 15;
        }
        instance.IncreasePlayerScoreForObjectives(playerRewardForStealingObjective);
        instance.DecreaseAIScoreForObjectives(50);

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
        foreach (Objective objective in objectives) {
            if (availableSpecialSentinels > 0 && !objective.reinforced && !objective.stolen)
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
        instance.SetPlayerWon(false);
        gameOverText.GetComponent<TextMesh>().text = DisplayScores(gameOverText.GetComponent<TextMesh>().text);
        gameOverText.SetActive(true);
        instance.SetGameEnded(true);
    }
    private void Concede()
    {
        instance.SetPlayerWon(true);
        victoryText.GetComponent<TextMesh>().text = DisplayScores(victoryText.GetComponent<TextMesh>().text);
        victoryText.SetActive(true);
        instance.SetGameEnded(true);
    }

    private void UpdateHP()
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (playerHPTexts[i] != null && players[i] != null)
            {
                playerHPTexts[i].GetComponent<TextMesh>().text = "Unit 1: " + players[i].healthPoints;
            }
            else
            {
                playerHPTexts[i].SetActive(false);
            }
        }
    }

    private string DisplayScores(string text)
    {
        int playerScore = calculatePlayerScore(), AIScore = calculateAIScore();
        string finalString = text;
        finalString = finalString + "Your score: " + playerScore + "\n Enemy score: " + AIScore + "\n\n Escape - exit game";
        return finalString;
    }
    
    private int calculatePlayerScore()
    {
        int score = instance.GetPlayerScoreForObjectives();
        if (instance.GetPlayerWon())
        {
            score += 60;
        }
        return score;
    }

    private int calculateAIScore()
    {
        int score = instance.GetAIScoreForObjectives();
        if (!instance.GetPlayerWon())
        {
            score += 60;
        }
        double percentage = ((double)remainingEnemies / startingEnemiesNumber) * 90;
        score += (int)percentage;
        return score;
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
    private void CountHP()
    {
        if (Unit1HP != null)
        {
            playerHPTexts.Add(Unit1HP);
        }
        if (players.Count > 1 && Unit2HP != null )
        {
            playerHPTexts.Add(Unit2HP);
        }
        else
        {
            Unit2HP.SetActive(false);
            Unit3HP.SetActive(false);
            return;
        }
        if (players.Count > 2 && Unit3HP != null)
        {
            playerHPTexts.Add(Unit3HP);
        }
        else
        {
            Unit3HP.SetActive(false);
        }
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

    public static void EnemyDestroyedAlert()
    {
        instance.DecrementRemainingEnemies();
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

    private void DecrementRemainingEnemies()
    {
        remainingEnemies--;
    }

    private void SetPlayerDestroyedAlarm(bool alarm)
    {
        playerDestroyedAlarm = alarm;
    }

    private void SetGameEnded(bool end)
    {
        gameEnded = end;
    }

    private void SetPlayerWon(bool won)
    {
        playerWon = won;
    }

    private bool GetPlayerWon()
    {
        return playerWon;
    }

    private void IncreasePlayerScoreForObjectives (int amount)
    {
        playerScoreForObjectives += amount;
    }

    private void DecreaseAIScoreForObjectives(int amount)
    {
        AIScoreForObjectives -= amount;
    }

    private int GetPlayerScoreForObjectives()
    {
        return playerScoreForObjectives;
    }

    private int GetAIScoreForObjectives()
    {
        return AIScoreForObjectives;
    }
}

