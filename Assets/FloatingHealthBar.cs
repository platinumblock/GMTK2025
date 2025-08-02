using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingHealthBar : MonoBehaviour
{
    public Image healthBarFill;
    private Enemy enemy;
    private Transform mainCameraTransform;

    void Start()
    {
        enemy = GetComponentInParent<Enemy>();
    }

    void Update()
    {
        if (enemy != null && healthBarFill != null) {
            float fillAmount = enemy.GetHealth() / Enemy.DEFAULT_HEALTH;
            healthBarFill.fillAmount = fillAmount;
        }
    }
}
