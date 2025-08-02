using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealthBar : MonoBehaviour
{
    public Image healthBarFill;
    public Image healthBarFill2;
    public TMP_Text healthText;
    private GameObject player;

    public int textStartPosition;
    public int barWidth;
    public int maxValue;

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

    public IEnumerator SlowAnimate(float end)
    {
        float elapsedTime = 0f;
        float duration = 3f;
        float start = healthBarFill.fillAmount;
        float startText = healthText.gameObject.GetComponent<RectTransform>().anchoredPosition.x;

        while (elapsedTime < duration)
        {
            healthBarFill.fillAmount = Mathf.Lerp(start, end / maxValue, elapsedTime / duration);
            healthBarFill2.fillAmount = Mathf.Lerp(start, end / maxValue, elapsedTime / duration);
            healthText.text = Mathf.Round(100 * Mathf.Lerp(start * maxValue, end, elapsedTime / duration) / maxValue) + "%";
            //healthText.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(Mathf.Lerp(startText, textStartPosition - barWidth * (maxValue - end) / maxValue, elapsedTime / duration), healthText.gameObject.GetComponent<RectTransform>().anchoredPosition.y);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
    public IEnumerator Animate(float end)
    {
        //healthText.text = Mathf.Round(end) + "%"; // ASSUMING TOTAL HEALTH 100
        float elapsedTime = 0f;
        float duration = 0.3f;
        float start = healthBarFill.fillAmount;
        float startText = healthText.gameObject.GetComponent<RectTransform>().anchoredPosition.x;
       
        while (elapsedTime < duration)
        {
            healthBarFill.fillAmount = Mathf.Lerp(start, end / maxValue, elapsedTime / duration);
            healthBarFill2.fillAmount = Mathf.Lerp(start, end / maxValue, elapsedTime / duration);
            healthText.text = Mathf.Round(100 * Mathf.Lerp(start * maxValue, end, elapsedTime / duration) / maxValue) + "%";
            //healthText.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(Mathf.Lerp(startText, textStartPosition - barWidth * (maxValue - end) / maxValue, elapsedTime / duration), healthText.gameObject.GetComponent<RectTransform>().anchoredPosition.y);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
