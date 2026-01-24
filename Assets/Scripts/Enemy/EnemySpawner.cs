using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Was soll gespawnt werden?")]
    // Die eckigen Klammern [] machen daraus eine Liste für den Inspector!
    public GameObject[] enemyPrefabs; 
    public Transform[] spawnPoints; 

    [Header("Zeit-Einstellungen")]
    public float spawnInterval = 3.0f; 
    public float difficultyIncreaseInterval = 30.0f; 

    [Header("Stärke-Einstellungen")]
    public int baseHealth = 20; 
    public int healthIncrease = 20; 
    
    private int currentEnemyHealth;
    private float nextSpawnTime = 0f;
    private float nextDifficultyTime = 0f;

    void Start()
    {
        currentEnemyHealth = baseHealth;
        nextDifficultyTime = Time.time + difficultyIncreaseInterval;
    }

    void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            SpawnEnemy();
            nextSpawnTime = Time.time + spawnInterval;
        }

        if (Time.time >= nextDifficultyTime)
        {
            IncreaseDifficulty();
            nextDifficultyTime = Time.time + difficultyIncreaseInterval;
        }
    }

    void SpawnEnemy()
    {
        if(spawnPoints.Length == 0 || enemyPrefabs.Length == 0) return;

        // 1. Zufälligen Ort wählen
        int randomPointIndex = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[randomPointIndex];

        // 2. Zufälligen Gegner wählen (Normal oder Tank)
        int randomEnemyIndex = Random.Range(0, enemyPrefabs.Length);
        GameObject selectedPrefab = enemyPrefabs[randomEnemyIndex];

        // 3. Gegner erstellen
        GameObject newEnemy = Instantiate(selectedPrefab, spawnPoint.position, Quaternion.identity);

        // --- CHECK: Welches Script hat der Gegner? ---
        
        // Versuche das normale Script zu holen
        EnemyHealth normalHealth = newEnemy.GetComponent<EnemyHealth>();
        if (normalHealth != null)
        {
            normalHealth.SetHealth(currentEnemyHealth);
        }

        // Versuche das Tank Script zu holen
        TankHealth tankHealth = newEnemy.GetComponent<TankHealth>();
        if (tankHealth != null)
        {
            // Tanks kriegen doppelt so viel Leben als Bonus!
            tankHealth.SetHealth(currentEnemyHealth); 
        }
    }

    void IncreaseDifficulty()
    {
        currentEnemyHealth += healthIncrease;
        Debug.Log("Gegner stärker geworden! HP Basis: " + currentEnemyHealth);
        
        if(spawnInterval > 0.5f)
        {
            spawnInterval -= 0.1f; 
        }
    }
}