using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Was soll gespawnt werden?")]
    public GameObject enemyPrefab; // Zieh hier dein Enemy-Prefab rein
    public Transform[] spawnPoints; // Orte, wo Gegner erscheinen

    [Header("Zeit-Einstellungen")]
    public float spawnInterval = 3.0f; // Alle 3 Sekunden ein neuer Gegner
    public float difficultyIncreaseInterval = 30.0f; // Alle 30 Sekunden werden sie stärker

    [Header("Stärke-Einstellungen")]
    public int baseHealth = 20; // Start-Leben (1 Schlag bei 20 Schaden)
    public int healthIncrease = 20; // Wie viel Leben kommt dazu? (20 = +1 Schlag)
    
    private int currentEnemyHealth;
    private float nextSpawnTime = 0f;
    private float nextDifficultyTime = 0f;

    void Start()
    {
        // Am Anfang haben Gegner das Basis-Leben
        currentEnemyHealth = baseHealth;
        
        // Die erste Schwierigkeits-Erhöhung planen
        nextDifficultyTime = Time.time + difficultyIncreaseInterval;
    }

    void Update()
    {
        // 1. Gegner Spawnen
        if (Time.time >= nextSpawnTime)
        {
            SpawnEnemy();
            nextSpawnTime = Time.time + spawnInterval;
        }

        // 2. Schwierigkeit erhöhen (Mit der Zeit stärker werden)
        if (Time.time >= nextDifficultyTime)
        {
            IncreaseDifficulty();
            nextDifficultyTime = Time.time + difficultyIncreaseInterval;
        }
    }

    void SpawnEnemy()
    {
        // Zufälligen Spawn-Punkt auswählen
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[randomIndex];

        // Gegner erstellen
        GameObject newEnemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);

        // --- HIER PASSIERT DIE MAGIE ---
        // Wir greifen auf das Health-Script des NEUEN Gegners zu
        EnemyHealth healthScript = newEnemy.GetComponent<EnemyHealth>();

        if (healthScript != null)
        {
            // Wir überschreiben seine Werte mit den neuen, stärkeren Werten
            healthScript.maxHealth = currentEnemyHealth;
            // Wir müssen auch das aktuelle Leben auffüllen, sonst startet er halb tot
            healthScript.SetHealth(currentEnemyHealth); 
        }
    }

    void IncreaseDifficulty()
    {
        currentEnemyHealth += healthIncrease;
        Debug.Log("ACHTUNG: Gegner sind stärker geworden! Neues Leben: " + currentEnemyHealth);
        
        // Optional: Spawn-Rate erhöhen (damit es auch MEHR Gegner werden)
        if(spawnInterval > 0.5f)
        {
            spawnInterval -= 0.1f; // Gegner kommen etwas schneller
        }
    }
}