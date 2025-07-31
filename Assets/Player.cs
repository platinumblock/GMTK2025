using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class Player : MonoBehaviour
{
    float speed = 4;
    float xVelocity = 0;
    float yVelocity = 0;
    string[] moveKeys = { "a", "w", "s", "d" };
    bool[] moving = {false, false, false, false};
    Rigidbody2D rb;

    float shootingDelay = 0.5f;
    bool shooting = false;
    bool shotCooldown = false;
    public GameObject bullet;

    int health = 100;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        Movement();
        Rotation();
        Shoot();
    }

    private void FixedUpdate()
    {
        Movement2();
        Shoot2();
    }

    void Movement()
    {
        for (int i = 0; i < moveKeys.Length; i++)
        {
            if (Input.GetKeyDown(moveKeys[i]))
            {
                moving[i] = true;
            }
            if (Input.GetKeyUp(moveKeys[i]))
            {
                moving[i] = false;
            }
        }

        if (moving[0])
        {
            xVelocity = -speed;
        }
        if (moving[3])
        {
            xVelocity = speed;
        }
        if ((moving[0] && moving[3]) || (!moving[0] && !moving[3]))
        {
            xVelocity = 0;
        }
        if (moving[1])
        {
            yVelocity = speed;
        }
        if (moving[2])
        {
            yVelocity = -speed;
        }
        if ((moving[1] && moving[2]) || (!moving[1] && !moving[2]))
        {
            yVelocity = 0;
        }
    }

    void Movement2()
    {
        rb.MovePosition(rb.position + new Vector2(xVelocity, yVelocity) * Time.fixedDeltaTime);
    }

    void Rotation()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        Vector3 direction = mousePos - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;

        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
    void Shoot()
    {
        if (Input.GetMouseButtonDown(0))
        {
            shooting = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            shooting = false;
        }
    }

    void Shoot2()
    {
        if (shooting && !shotCooldown)
        {
            GameObject bulletObj = Instantiate(bullet, transform.position, transform.rotation);
            bulletObj.GetComponent<Bullet>().SetShooterTag(gameObject.tag);
            StartCoroutine(ShotCooldown());
        }
    }

    IEnumerator ShotCooldown()
    {
        shotCooldown = true;
        yield return new WaitForSeconds(shootingDelay);
        shotCooldown = false;
    }

    public void Damage(int damage)
    {
        health -= damage;
        if (health <= 0) {
            GameManager.Lose();  // Lost all health
        }
    }
}
