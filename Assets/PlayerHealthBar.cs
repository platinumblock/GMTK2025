using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    public Image healthBarFill;
    public Image healthBarFill2;
    private GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectsWithTag("Player")[0];
    }

    void Update()
    {
        /*
        if (player != null && healthBarFill != null) {
            float fillAmount = player.GetComponent<Player>().GetHealth() / 100.0f;
            healthBarFill.fillAmount = fillAmount;
        }
        */
    }

    public IEnumerator Animate(float end)
    {
        float elapsedTime = 0f;
        float duration = 0.3f;
        float start = healthBarFill.fillAmount;
        while(elapsedTime < duration)
        {
            healthBarFill.fillAmount = Mathf.Lerp(start, end / 100f, elapsedTime / duration);
            healthBarFill2.fillAmount = Mathf.Lerp(start, end / 100f, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
