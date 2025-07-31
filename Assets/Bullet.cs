using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public const float DEFAULT_DAMAGE = 1.0f;
    public const float DEFAULT_SPEED = 0.25f;

    private float damage;

    // Start is called before the first frame update
    void Start()
    {
        this.damage = Bullet.DEFAULT_DAMAGE;
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
        if (c.tag == "Enemy") {
            c.gameObject.GetComponent<Enemy>().TakeDamage(this.damage);
            Destroy(this.gameObject);
        }
        else if (c.tag == "PastPlayer") {
            // TODO: call lose here
        }
    }

    public void SetDamage(float damage) {
        this.damage = damage;
    }
}
