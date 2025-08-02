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
        healthText.text = Mathf.Round(end) + "%"; // ASSUMING TOTAL HEALTH 100
        float elapsedTime = 0f;
        float duration = 0.3f;
        float start = healthBarFill.fillAmount;
        float startText = healthText.gameObject.GetComponent<RectTransform>().anchoredPosition.x;
        while (elapsedTime < duration)
        {
            healthBarFill.fillAmount = Mathf.Lerp(start, end / 100f, elapsedTime / duration);
            healthBarFill2.fillAmount = Mathf.Lerp(start, end / 100f, elapsedTime / duration);
            healthText.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(Mathf.Lerp(startText, 232 - 250 * (100f - end) / 100f, elapsedTime / duration), -11.7f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
