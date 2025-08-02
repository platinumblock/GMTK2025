using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class Player : MonoBehaviour
{
    float speed = 3;
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

    public static Vector3 startingPosition;
    public static List<Vector2> velocities = new List<Vector2>();
    public static List<float> angles = new List<float>();

    public PostProcessVolume volume;
    private Vignette vignette;
    private ChromaticAberration chrome;

    private void Start()
    {
        startingPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();

        volume.profile.TryGetSettings(out vignette);
        volume.profile.TryGetSettings(out chrome);
    }

    public void RecordStart()
    {
        startingPosition = transform.position;
    }
    void Update()
    {
        Movement();
        
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

    Vector2 oldPosition;

    void Movement2()
    {

        velocities.Add(transform.position);

        rb.MovePosition(rb.position + new Vector2(xVelocity, yVelocity) * Time.fixedDeltaTime);
        
        
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
            GameObject bulletObj = Instantiate(bullet, transform.position, transform.GetChild(0).transform.rotation);
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

        StartCoroutine(DamageAnimation());
        StartCoroutine(GameObject.FindObjectOfType<PlayerHealthBar>().Animate(health));
    }

    IEnumerator DamageAnimation()
    {
        float time = 0f;
        float duration = 0.2f;
        while(time < duration)
        {
            vignette.intensity.value = Mathf.Lerp(0, 0.4f, time / duration);
            chrome.intensity.value = Mathf.Lerp(0, 0.6f, time / duration);
            time += Time.deltaTime;
            yield return null;
            
        }
        yield return new WaitForSeconds(0.1f);
        time = 0f;
        while (time < duration)
        {
            vignette.intensity.value = Mathf.Lerp(0.4f, 0, time / duration);
            chrome.intensity.value = Mathf.Lerp(0.6f, 0, time / duration);
            time += Time.deltaTime;
            yield return null;

        }

    }

    public float GetHealth() {
        return (float) this.health;
    }
}
