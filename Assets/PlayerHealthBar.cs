using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    public Image healthBarFill;
    private GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectsWithTag("Player")[0];
    }

    void Update()
    {
        if (player != null && healthBarFill != null) {
            float fillAmount = player.GetComponent<Player>().GetHealth() / 100.0f;
            healthBarFill.fillAmount = fillAmount;
        }
    }
}
