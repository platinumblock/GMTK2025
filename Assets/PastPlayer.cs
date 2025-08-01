using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PastPlayer : MonoBehaviour
{
    List<Vector2> velocities;
    List<float> angles;
    Rigidbody2D rb;

    Vector3 startingPosition;

    int frameCounter = 0;
    void Start()
    {
        velocities = new List<Vector2>(Player.velocities);
        Player.velocities.Clear();
        angles = new List<float>(Player.angles);
        Player.angles.Clear();

        rb = GetComponent<Rigidbody2D>();
        startingPosition = transform.position;
    }

    private void FixedUpdate()
    {
        if(frameCounter < velocities.Count)
        {
            transform.position = velocities[frameCounter];
            //rb.MovePosition(rb.position + velocities[frameCounter] * Time.fixedDeltaTime);
            transform.GetChild(0).rotation = Quaternion.Euler(new Vector3(0, 0, angles[frameCounter]));
        }
        frameCounter += 1;
    }

    public void Reset()
    {
        frameCounter = 0;
        transform.position = startingPosition;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Wall")
        {
            GetComponent<SpriteRenderer>().enabled = false;
            transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Wall")
        {
            GetComponent<SpriteRenderer>().enabled = true;
            transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
        }
    }
}
