using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public const float DEFAULT_SPEED = 0.1f;
    public const float DEFAULT_HEALTH = 100.0f;
    public const float SHOOTING_COOLDOWN_SECONDS = 1.0f;

    private float speed;
    private float health;
    private float rotation;

    public GameObject bullet;

    // Start is called before the first frame update
    void Start()
    {
        this.speed = Enemy.DEFAULT_SPEED;
        this.health = Enemy.DEFAULT_HEALTH;
        this.rotation = this.transform.localRotation.eulerAngles.z;

        StartCoroutine(this.Shoot());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        this.FaceTowardPlayer();
        this.transform.position += this.transform.up * this.speed;
    }

    void FaceTowardPlayer() {
        // Get the player (nothing to face if the player is null)
        GameObject player = GameObject.FindGameObjectsWithTag("Player")[0];
        if (player == null) {
            Debug.LogWarning("Player not found! Cannot shoot bullet.", this);
            return;
        }

        // Calculate angle to player
        float myX = this.transform.position.x;
        float myY = this.transform.position.y;
        float targetX = player.transform.position.x;
        float targetY = player.transform.position.y;

        float dx = targetX - myX;
        float dy = targetY - myY;

        float theta = 0.0f;
        if (dy != 0.0f) {
            theta = dx < 0.0f ? Mathf.Atan(dy / dx) + Mathf.PI : Mathf.Atan(dy / dx);
        }
        float thetaDeg = theta * (180.0f / Mathf.PI) - 90.0f;

        // Rotate to face the player
        this.transform.Rotate(0.0f, 0.0f, (thetaDeg - this.rotation));
        this.rotation = thetaDeg;

    }

    public void Damage(float damage) {
        this.health -= damage;
        if (this.health <= 0) {
            Destroy(this.gameObject);
        }
    }

    public IEnumerator ShootAfterCooldown() {
        yield return new WaitForSeconds(Enemy.SHOOTING_COOLDOWN_SECONDS);

        // We are already facing toward the player because of FaceTowardPlayer in FixedUpdate,
        // so we can just shoot with the current position/rotation to shoot toward the player
        GameObject bulletObj = Instantiate(this.bullet,
                                           this.transform.position,
                                           this.transform.rotation);
        bulletObj.GetComponent<Bullet>().SetShooterTag(this.gameObject.tag);

        StartCoroutine(this.Shoot());
    }

    public IEnumerator Shoot() {
        return this.ShootAfterCooldown();
    }
}
