using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;


public class RoundManager : MonoBehaviour
{
    int round = 1;
    float roundLength = 30;
    float timeBetweenSpawns = 3;
    float enemiesSpawned = 8;
    float timeBetweenRounds = 5;
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
    public TMP_Text beginText;

    public static bool transitioning = false;
    public static bool endGame = false;
    public GameObject endScreen;
    public TMP_Text finalTimeText;

    public GameObject restartButton;
    public GameObject exitButton;

    private int totalTimer = 0;

    public AudioClip beginRound;
    public AudioClip rewind;

    public AudioSource song;

    void Start()
    {
        blocks = GameObject.FindObjectsOfType<Block>();
        StartCoroutine(Begin());
        

        volume.profile.TryGetSettings(out depth);
        volume.profile.TryGetSettings(out vignette);
        volume.profile.TryGetSettings(out chrome);
    }

    IEnumerator Begin()
    {
        AudioListener.volume = 15f;
        AudioSource.PlayClipAtPoint(beginRound, new Vector3(0, 0, 0));
        
        transitioning = true;
        float elapsedTime = 0f;
        float duration = 0.5f;

        while (elapsedTime < duration)
        {
            beginText.fontSize = Mathf.Lerp(0, 50, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(2.5f);
        AudioListener.volume = 1f;
        elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            beginText.fontSize = Mathf.Lerp(50, 0, elapsedTime / duration);
            depth.focusDistance.value = Mathf.Lerp(2, 4.24f, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        beginText.fontSize = 0;
        transitioning = false;


        StartCoroutine(RunRound());
        StartCoroutine(RoundTimer());
        StartCoroutine(TotalTimer());

    }

    IEnumerator TotalTimer()
    {
        yield return new WaitForSeconds(1f);
        if (endGame)
        {
            yield break;
        }
        if (!transitioning)
        {
            totalTimer += 1;
        }
        StartCoroutine(TotalTimer());
    }

    IEnumerator RoundTimer()
    {
        StartCoroutine(timeBar.GetComponent<PlayerHealthBar>().Animate(0f));
        for (int i = 0; i < roundLength * 2; i++)
        {
            
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(timeBar.GetComponent<PlayerHealthBar>().Animate((i + 1) / 2f));
            if (endGame || transitioning)
            {
                yield break;
            }
            
        }
        /*
        yield return new WaitForSeconds(timeBetweenRounds);
        
        if (endGame)
        {
            yield break;
        }
        */

        //StartCoroutine(RoundTimer());
    }

    IEnumerator RunRound()
    {
        song.Play();
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
        foreach (Block block in blocks)
        {
            block.Bro();
        }
        song.Pause();
        yield return new WaitForSeconds(1f);
        
        AudioSource.PlayClipAtPoint(rewind, new Vector3(0, 0, 0));
        StartCoroutine(RoundTransition());
        StartCoroutine(NoEffects2());

        

        yield return new WaitForSeconds(timeBetweenRounds - 1);

        if (endGame)
        {
            yield break;
        }

        
        SpawnPastSelf();
        



        transitioning = false;

        StartCoroutine(RoundTimer());
        StartCoroutine(RunRound());
    }

    public void Lose()
    {
        song.Pause();
        transitioning = true;
        endGame = true;
        StartCoroutine(healthBar.GetComponent<PlayerHealthBar>().Animate(0));
        ClearEnemies();
        ClearBullets();
        
        StartCoroutine(NoEffects());
        
    }

    public void Restart()
    {
        transitioning = false;
        endGame = false;
        Player.velocities = new List<Vector2>();
        Player.angles = new List<float>();
        SceneManager.LoadScene("SampleScene");
    }

    public void Exit()
    {
        transitioning = false;
        endGame = false;
        Player.velocities = new List<Vector2>();
        Player.angles = new List<float>();
        SceneManager.LoadScene("MainMenu");
    }
    IEnumerator DropEnd()
    {
        string rank = "";
        Color color;
        switch (totalTimer)
        {
            case < 30:
                rank = "D";
                color = new Color(196, 44, 173);
                break;
            case < 60:
                rank = "C";
                color = new Color(167, 110, 51);
                break;
            case < 150:
                rank = "B";
                color = new Color(42, 88, 121);
                break;
            case < 270:
                rank = "A";
                color = new Color(197, 61, 56);
                break;
            case >= 270:
                rank = "S";
                color = new Color(39, 255, 0);
                break;
        }
        color = new Color(color.r / 255f, color.g / 255f, color.b / 255f);
        finalTimeText.text = rank + " Rank";
        if(rank == "D")
        {
            finalTimeText.text += " :(";
        }
        if(rank == "A")
        {
            finalTimeText.text += "!";
        }
        if (rank == "S") {
            finalTimeText.text += "!!!";
        }
        finalTimeText.color = color;
        restartButton.SetActive(true);
        exitButton.SetActive(true);
        float elapsedTime = 0f;
        float duration = 0.5f;
        float start = 509;
        while (elapsedTime < duration)
        {
            endScreen.GetComponent<RectTransform>().anchoredPosition = new Vector2(endScreen.GetComponent<RectTransform>().anchoredPosition.x, Mathf.Lerp(start, 60, Mathf.SmoothStep(0f, 1f, elapsedTime / duration)));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator NoEffects2()
    {
        float elapsedTime = 0f;
        float duration = 4f;
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
    IEnumerator NoEffects()
    {
        float elapsedTime = 0f;
        float duration = 0.5f;
        float startV = vignette.intensity.value;
        float startC = chrome.intensity.value;
        float startD = depth.focusDistance.value;
        while (elapsedTime < duration)
        {
            vignette.intensity.value = Mathf.Lerp(startV, 0, elapsedTime / duration);
            chrome.intensity.value = Mathf.Lerp(startC, 0, elapsedTime / duration);
            depth.focusDistance.value = Mathf.Lerp(startD, 2f, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        StartCoroutine(DropEnd());
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
            roundTransText.fontSize = Mathf.Lerp(0, 50, elapsedTime / duration);
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
        ResetPastSelves();


        yield return new WaitForSeconds(1.5f);
        elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            depth.focusDistance.value = Mathf.Lerp(end, start, elapsedTime / duration);
            roundTransText.fontSize = Mathf.Lerp(50, 0, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        roundTransText.fontSize = 0;

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
