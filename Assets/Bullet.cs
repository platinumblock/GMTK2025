using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public const float DEFAULT_DAMAGE = 25.0f;
    public const float DEFAULT_SPEED = 6f;

    private float damage;
    private string shooterTag;

    public GameObject explosionPrefab;

    private Rigidbody2D rb;
    private int bounces = 0;

    // Start is called before the first frame update
    void Start()
    {
        this.damage = Bullet.DEFAULT_DAMAGE;
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = this.transform.up * Bullet.DEFAULT_SPEED;
    }

    // Update is called once per frame
    void Update()
    {
        
       // this.transform.position += this.transform.up * Bullet.DEFAULT_SPEED * Time.deltaTime;

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
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
        else if (c.tag == "PastPlayer" && this.shooterTag == "Player") {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            GameManager.Lose();  // Killed a past player
            Destroy(this.gameObject);
        }
        else if (c.tag == "Player" && this.shooterTag == "Enemy") {
            c.gameObject.GetComponent<Player>().Damage((int) this.damage);
            GameObject explode = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            var main = explode.GetComponent<ParticleSystem>().main;
            main.startColor = new Color(191 / 255f, 64 / 255f, 64 / 255f);
            Destroy(this.gameObject);
        }
        else if (c.tag == "Wall")
        {
            if(c.name == "Block")
            {
                StartCoroutine(c.gameObject.GetComponent<Block>().Shake(0.15f, 0.1f));
            }
            
            if(bounces == 1)
            {
                if (this.shooterTag == "Enemy")
                {
                    GameObject explode = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
                    var main = explode.GetComponent<ParticleSystem>().main;
                    main.startColor = new Color(191 / 255f, 64 / 255f, 64 / 255f);
                }
                else
                {
                    Instantiate(explosionPrefab, transform.position, Quaternion.identity);
                }

                Destroy(this.gameObject);
            }

            Vector3 center = c.bounds.center;
            Vector3 extents = c.bounds.extents;
            float yTop = center.y + extents.y;
            float yBot = center.y - extents.y;
            float xLeft = center.x - extents.x;
            float xRight = center.x + extents.x;

            if(transform.position.y > yBot && transform.position.y < yTop) //horizontal collision
            {
                rb.velocity = new Vector2(-rb.velocity.x, rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x, -rb.velocity.y);
            }
            bounces += 1;
               
            
           
            //Destroy(this.gameObject);
        }
    }

    public void SetDamage(float damage) {
        this.damage = damage;
    }

    public void SetShooterTag(string shooterTag) {
        this.shooterTag = shooterTag;

        if(this.shooterTag == "Enemy")
        {
            GetComponent<SpriteRenderer>().color = new Color(191 / 255f, 64 / 255f, 64 / 255f);
            var main = GetComponent<ParticleSystem>().main;
            main.startColor = new Color(191 / 255f, 64 / 255f, 64 / 255f);
        }
    }
}
