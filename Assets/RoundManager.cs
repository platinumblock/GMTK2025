using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;


public class RoundManager : MonoBehaviour
{
    int round = 1;
    float roundLength = 30;
    float timeBetweenSpawns = 3;
    float enemiesSpawned = 8;
    float timeBetweenRounds = 4;
    public GameObject enemy;
    public GameObject pastSelf;
    public Player player;
    Block[] blocks;

    public GameObject enemySpawnLocations;

    public GameObject timeBar;
    public GameObject healthBar;

    public PostProcessVolume volume;
    DepthOfField depth;
    private Vignette vignette;
    private ChromaticAberration chrome;
    public TMP_Text roundTransText;

    public static bool transitioning = false;
    public static bool endGame = false;

    void Start()
    {
        blocks = GameObject.FindObjectsOfType<Block>();
        StartCoroutine(RunRound());
        StartCoroutine(RoundTimer());

        volume.profile.TryGetSettings(out depth);
        volume.profile.TryGetSettings(out vignette);
        volume.profile.TryGetSettings(out chrome);
    }

    IEnumerator RoundTimer()
    {
        StartCoroutine(timeBar.GetComponent<PlayerHealthBar>().Animate(0f));
        for (int i = 0; i < roundLength * 2; i++)
        {
            StartCoroutine(timeBar.GetComponent<PlayerHealthBar>().Animate(i / 2f));
            yield return new WaitForSeconds(0.5f);
            
            if (endGame)
            {
                yield break;
            }
            
        }
        yield return new WaitForSeconds(timeBetweenRounds);
        
        if (endGame)
        {
            yield break;
        }

        StartCoroutine(RoundTimer());
    }

    IEnumerator RunRound()
    {
        for(int i = 0; i < enemiesSpawned; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(timeBetweenSpawns);

            if (endGame)
            {
                yield break;
            }
        }

        yield return new WaitForSeconds(roundLength - enemiesSpawned * timeBetweenSpawns);

        if (endGame)
        {
            yield break;
        }

        transitioning = true;
        ClearEnemies();
        ClearBullets();
        
        StartCoroutine(RoundTransition());

        

        yield return new WaitForSeconds(timeBetweenRounds);

        if (endGame)
        {
            yield break;
        }

        ResetPastSelves();
        SpawnPastSelf();
        



        transitioning = false;

        StartCoroutine(RunRound());
    }

    public void Lose()
    {
        transitioning = true;
        endGame = true;
        StartCoroutine(healthBar.GetComponent<PlayerHealthBar>().Animate(0));
        ClearEnemies();
        ClearBullets();
        
        StartCoroutine(NoEffects());
    }

    IEnumerator NoEffects()
    {
        float elapsedTime = 0f;
        float duration = 0.5f;
        float startV = vignette.intensity.value;
        float startC = chrome.intensity.value;
        while (elapsedTime < duration)
        {
            vignette.intensity.value = Mathf.Lerp(startV, 0, elapsedTime / duration);
            chrome.intensity.value = Mathf.Lerp(startC, 0, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator RoundTransition()
    {
        float elapsedTime = 0f;
        float duration = 0.5f;
        float start = 4.24f;
        float end = 2f;
        while (elapsedTime < duration)
        {
            depth.focusDistance.value = Mathf.Lerp(start, end, elapsedTime / duration);
            roundTransText.fontSize = Mathf.Lerp(0, 96, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        StartCoroutine(timeBar.GetComponent<PlayerHealthBar>().SlowAnimate(0));
        StartCoroutine(healthBar.GetComponent<PlayerHealthBar>().SlowAnimate(100));

        yield return new WaitForSeconds(1.5f);

        player.Reset();
        foreach (Block block in blocks)
        {
            block.gameObject.SetActive(true);
            block.Reset();
        }
        player.RecordStart();


        yield return new WaitForSeconds(1.5f);
        elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            depth.focusDistance.value = Mathf.Lerp(end, start, elapsedTime / duration);
            roundTransText.fontSize = Mathf.Lerp(96, 0, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

    }
    void ClearBullets()
    {
        Bullet[] bullets = GameObject.FindObjectsOfType<Bullet>();
        foreach(Bullet bullet in bullets)
        {
            bullet.SelfDestruct();
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
        Instantiate(pastSelf, new Vector3(Player.startingPosition.x, Player.startingPosition.y, 0f), Quaternion.identity);
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
