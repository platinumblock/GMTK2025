using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    int round = 1;
    float roundLength = 30;
    float timeBetweenSpawns = 3;
    float enemiesSpawned = 8;
    float timeBetweenRounds = 1;
    public GameObject enemy;
    public GameObject pastSelf;
    public Player player;
    Block[] blocks;

    public GameObject enemySpawnLocations;
    void Start()
    {
        blocks = GameObject.FindObjectsOfType<Block>();
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
        foreach(Block block in blocks)
        {
            block.gameObject.SetActive(true);
            block.Reset();
        }
        ClearBullets();

        StartCoroutine(RunRound());
    }

    void ClearBullets()
    {
        Bullet[] bullets = GameObject.FindObjectsOfType<Bullet>();
        foreach(Bullet bullet in bullets)
        {
            Destroy(bullet.gameObject);
        }
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
        Transform location = enemySpawnLocations.transform.GetChild(Random.Range(0, enemySpawnLocations.transform.childCount));
        Instantiate(enemy, new Vector3(location.position.x, location.position.y, -1), Quaternion.identity);
    }
}
