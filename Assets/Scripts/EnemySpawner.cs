using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Was soll gespawnt werden?")]
    public GameObject[] enemyPrefabs; 
    public Transform[] spawnPoints; 

    [Header("Zeit-Einstellungen")]
    public float spawnInterval = 4.0f;
    private float timeFirstPhase = 20.0f;
    private float timeSecondPhase = 40.0f;
    private float fasterSpawnInterval = 2.0f;
    
    private float nextSpawnTime = 0f;

    void Update()
    {
        float intervalToUse = (Time.time >= timeSecondPhase) ? fasterSpawnInterval : spawnInterval;
        if (Time.time >= nextSpawnTime)
        {
            SpawnEnemy();
            nextSpawnTime = Time.time + intervalToUse;
        }
    }

    void SpawnEnemy()
    {
        if(spawnPoints.Length == 0 || enemyPrefabs.Length == 0) return;

        int randomPointIndex = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[randomPointIndex];

        int selectedEnemyIndex = 0;
        if (Time.time > timeFirstPhase)
        {
            selectedEnemyIndex = Random.Range(0, enemyPrefabs.Length);
        }
        else
        {
            selectedEnemyIndex = 0;
        }

        GameObject selectedPrefab = enemyPrefabs[selectedEnemyIndex];
        Instantiate(selectedPrefab, spawnPoint.position, Quaternion.identity);
    }
}