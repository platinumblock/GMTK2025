using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    int round = 1;
    float roundLength = 10;
    float timeBetweenSpawns = 2;
    float enemiesSpawned = 3;
    float timeBetweenRounds = 1;
    public GameObject enemy;
    public GameObject pastSelf;
    public Player player;
    void Start()
    {
        StartCoroutine(RunRound());
    }

    IEnumerator RunRound()
    {
        for(int i = 0; i < enemiesSpawned; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(timeBetweenSpawns);
        }

        yield return new WaitForSeconds(roundLength - enemiesSpawned * timeBetweenSpawns);

        ClearEnemies();


        yield return new WaitForSeconds(timeBetweenRounds);

        ResetPastSelves();
        SpawnPastSelf();
        player.RecordStart();

        StartCoroutine(RunRound());
    }

    void ResetPastSelves()
    {
        PastPlayer[] pastSelves = GameObject.FindObjectsOfType<PastPlayer>();
        foreach(PastPlayer pastSelf in pastSelves)
        {
            pastSelf.Reset();
        }
    }

    void SpawnPastSelf()
    {
        Instantiate(pastSelf, Player.startingPosition, Quaternion.identity);
    }

    void ClearEnemies()
    {
        Enemy[] enemies = GameObject.FindObjectsOfType<Enemy>();
        foreach(Enemy enemy in enemies)
        {
            enemy.Damage(99999);
        }
    }

    void SpawnEnemy()
    {
        Instantiate(enemy, new Vector3(3, -1, 0), Quaternion.identity);
    }
}
