using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

public class Player : MonoBehaviour
{
    float speed = 2;
    float xVelocity = 0;
    float yVelocity = 0;
    string[] moveKeys = { "a", "w", "s", "d" };
    bool[] moving = {false, false, false, false};

    int health = 100;

    void Update()
    {
        Movement();
        Rotation();
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

        transform.position += new Vector3(xVelocity, yVelocity, 0) * Time.deltaTime;
    }

    void Rotation()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        Vector3 direction = mousePos - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    public void Damage(int damage)
    {
        health -= damage;
    }
}
