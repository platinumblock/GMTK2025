using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    private int hp = 4;
    bool cooldown = false;

    public GameObject blockExplosion;

    public void Reset()
    {
        hp = 4;
        cooldown = false;
    }

    IEnumerator Cooldown()
    {
        cooldown = true;
        yield return new WaitForSeconds(0.1f);
        cooldown = false;
    }

    public IEnumerator Shake(float duration, float magnitude)
    {
        
        if (!cooldown)
        {
            hp -= 1;
            if (hp <= 0)
            {
                Instantiate(blockExplosion, new Vector3(transform.position.x, transform.position.y, -3), transform.rotation);
                this.gameObject.SetActive(false);
                yield break;
            }
            StartCoroutine(Cooldown());
        }
        
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0f;
        while(elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            if(this == null)
            {
                yield break;
            }
            transform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }
        if(this == null)
        {
            yield break;
        }
        transform.localPosition = originalPos;

        
    }
}
