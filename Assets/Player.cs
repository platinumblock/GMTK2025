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

    public GameObject healthBar;

    public static Vector3 realStartingPosition;

    public RoundManager rM;

    private void Start()
    {
        startingPosition = transform.position;
        realStartingPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();

        volume.profile.TryGetSettings(out vignette);
        volume.profile.TryGetSettings(out chrome);
    }

    public void RecordStart()
    {
        transform.position = realStartingPosition;
        startingPosition = transform.position;
    }

    public void Reset()
    {
        xVelocity = 0;
        yVelocity = 0;
        moving = new bool[]{false, false, false, false};
        shooting = false;
        shotCooldown = false;
        health = 100;
        

    }
    void Update()
    {
        if (!RoundManager.transitioning)
        {
            Movement();

            Shoot();
        }
        
    }

    private void FixedUpdate()
    {
        if (!RoundManager.transitioning)
        {
            Movement2();

            Shoot2();
        }
        
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
        if (RoundManager.transitioning)
        {
            return;
        }
        health -= damage;
        if (health <= 0) {
            rM.Lose(); // Lost all health
            return;
        }

        StartCoroutine(DamageAnimation());
        StartCoroutine(healthBar.GetComponent<PlayerHealthBar>().Animate(health));
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
            vignette.intensity.value = Mathf.Lerp(0.4f, 0.2f + 0.3f * (100 - health) / 100, time / duration);
            chrome.intensity.value = Mathf.Lerp(0.6f, 0.3f * (100 - health) / 100, time / duration);
            time += Time.deltaTime;
            yield return null;

        }

    }

    public float GetHealth() {
        return (float) this.health;
    }
}
