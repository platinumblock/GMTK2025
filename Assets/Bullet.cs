using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public const float DEFAULT_DAMAGE = 25.0f;
    public const float DEFAULT_SPEED = 0.25f;

    private float damage;
    private string shooterTag;

    // Start is called before the first frame update
    void Start()
    {
        this.damage = Bullet.DEFAULT_DAMAGE;
        this.shooterTag = "";  // Set this when Player/Enemy creates a bullet to shoot
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position += this.transform.up * Bullet.DEFAULT_SPEED;

        float aspect = (float) Screen.width / Screen.height;
        float worldHeight = 2.0f * Camera.main.orthographicSize;
        float worldWidth = worldHeight * aspect;

        if (this.transform.position.x < -worldWidth / 2.0f ||
            this.transform.position.x > worldWidth / 2.0f ||
            this.transform.position.y < -worldHeight ||
            this.transform.position.y > worldHeight)
        {
            Destroy(this.gameObject);
        }
    }

    public void OnTriggerEnter2D(Collider2D c) {
        if (this.shooterTag == "") {
            Debug.Log("WARN: Bullet shooter tag not set");
        }

        if (c.tag == "Enemy" && this.shooterTag == "Player") {
            c.gameObject.GetComponent<Enemy>().Damage(this.damage);
            Destroy(this.gameObject);
        }
        else if (c.tag == "PastPlayer" && this.shooterTag == "Player") {
            GameManager.Lose();  // Killed a past player
        }
        else if (c.tag == "Player" && this.shooterTag == "Enemy") {
            c.gameObject.GetComponent<Player>().Damage((int) this.damage);
            Destroy(this.gameObject);
        }
    }

    public void SetDamage(float damage) {
        this.damage = damage;
    }

    public void SetShooterTag(string shooterTag) {
        this.shooterTag = shooterTag;
    }
}
