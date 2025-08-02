using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public const float DEFAULT_SPEED = 1.5f;
    public const float DEFAULT_HEALTH = 50.0f;
    public const float SHOOTING_COOLDOWN_SECONDS = 3.0f;

    private float speed;
    private float health;
    private float rotation;

    public GameObject bullet;

    public GameObject explosionPrefab;

    private Vector2 targetPosition;

    // Start is called before the first frame update
    void Start()
    {
        this.speed = Enemy.DEFAULT_SPEED;
        this.health = Enemy.DEFAULT_HEALTH;
        this.rotation = this.transform.localRotation.eulerAngles.z;
        StartCoroutine(this.Shoot());
        StartCoroutine(ScaleUp());
        newTarget();
    }

    IEnumerator ScaleUp()
    {
        float elapsed = 0f;
        float duration = 0.5f;
        while (elapsed < duration)
        {
            float newScale = Mathf.Lerp(0f, 1.5f, elapsed / duration);
            transform.localScale = new Vector3(newScale, newScale, 1);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    void newTarget()
    {
        for (int i = 0; i < 1000; i++)
        {
            float x = Random.Range(-16f, 16f);
            float y = Random.Range(-7f, 7f);
            if (!WillLineOverlapBlock(transform.position, new Vector2(x, y)))
            {
                targetPosition = new Vector2(x, y);
                break;
            }
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        this.FaceTowardPlayer();
        //this.transform.position += this.transform.GetChild(0).transform.up * this.speed * Time.deltaTime;
        Vector2 v = new Vector2(targetPosition.x - transform.position.x, targetPosition.y - transform.position.y);
        float mag = Mathf.Sqrt(v.x * v.x + v.y * v.y);
        v = v / mag;
        this.transform.position = new Vector3(transform.position.x + v.x * speed * Time.deltaTime, transform.position.y + v.y * speed * Time.deltaTime, -1);
        if(Vector2.Distance(transform.position, targetPosition) < 0.5f)
        {
            newTarget();
        }
        
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
        transform.GetChild(0).transform.Rotate(0.0f, 0.0f, (thetaDeg - this.rotation));
        this.rotation = thetaDeg;

    }

    public void Damage(float damage) {
        this.health -= damage;
        if (this.health <= 0) {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }

    public IEnumerator ShootAfterCooldown() {
        yield return new WaitForSeconds(Enemy.SHOOTING_COOLDOWN_SECONDS);

        // We are already facing toward the player because of FaceTowardPlayer in FixedUpdate,
        // so we can just shoot with the current position/rotation to shoot toward the player
        GameObject bulletObj = Instantiate(this.bullet,
                                           this.transform.position,
                                           this.transform.GetChild(0).transform.rotation);
        bulletObj.GetComponent<Bullet>().SetShooterTag(this.gameObject.tag);

        StartCoroutine(this.Shoot());
    }

    public IEnumerator Shoot() {
        return this.ShootAfterCooldown();
    }

















    public static bool WillLineOverlapBlock(Vector2 start, Vector2 end)
    {
        foreach (var block in Object.FindObjectsOfType<GameObject>())
        {
            if (block.name != "Block") continue;

            BoxCollider2D[] colliders = block.GetComponents<BoxCollider2D>();
            foreach (var col in colliders)
            {
                Bounds bounds = col.bounds;

                if (bounds.size.x <= 0 || bounds.size.y <= 0) continue;

                if (LineIntersectsAABB(start, end, bounds))
                    return true;
            }
        }

        return false;
    }

    private static bool LineIntersectsAABB(Vector2 p1, Vector2 p2, Bounds bounds)
    {
        bounds.Expand(0.75f * 2f);

        Vector2 min = bounds.min;
        Vector2 max = bounds.max;

        if (bounds.Contains(new Vector3(p1.x, p1.y, bounds.center.z)) || bounds.Contains(new Vector3(p2.x, p2.y, bounds.center.z)))
            return true;

        Vector2[] corners = new Vector2[4]
        {
            new Vector2(min.x, min.y),
            new Vector2(min.x, max.y),
            new Vector2(max.x, max.y),
            new Vector2(max.x, min.y)
        };

        for (int i = 0; i < 4; i++)
        {
            Vector2 edgeStart = corners[i];
            Vector2 edgeEnd = corners[(i + 1) % 4];

            if (LinesIntersect(p1, p2, edgeStart, edgeEnd))
                return true;
        }

        return false;
    }

    private static bool LinesIntersect(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
    {
        float d = (a2.x - a1.x) * (b2.y - b1.y) - (a2.y - a1.y) * (b2.x - b1.x);
        if (Mathf.Approximately(d, 0f)) return false;

        float u = ((b1.x - a1.x) * (b2.y - b1.y) - (b1.y - a1.y) * (b2.x - b1.x)) / d;
        float v = ((b1.x - a1.x) * (a2.y - a1.y) - (b1.y - a1.y) * (a2.x - a1.x)) / d;

        return u >= 0f && u <= 1f && v >= 0f && v <= 1f;
    }

    public float GetHealth() {
        return this.health;
    }
}
