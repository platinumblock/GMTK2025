using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    private int hp = 8;
    bool cooldown = false;

    public GameObject blockExplosion;

    public float scaleX;
    public float scaleY;

    public AudioClip blockExplodes;

    public void Start()
    {
        StartCoroutine(ScaleUp());
    }

    IEnumerator ScaleUp()
    {
        float elapsed = 0f;
        float duration = 0.5f;
        while (elapsed < duration)
        {
            float newScaleX = Mathf.Lerp(0f, scaleX, elapsed / duration);
            float newScaleY = Mathf.Lerp(0f, scaleY, elapsed / duration);
            transform.localScale = new Vector3(newScaleX, newScaleY, 1);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localScale = new Vector3(scaleX, scaleY, 1);
    }
    public void Bro()
    {
        if(this.isActiveAndEnabled)
        {
            StartCoroutine(ScaleDown());
        }
    }
    IEnumerator ScaleDown()
    {
        float elapsed = 0f;
        float duration = 0.5f;
        while (elapsed < duration)
        {
            float newScaleX = Mathf.Lerp(scaleX,0f, elapsed / duration);
            float newScaleY = Mathf.Lerp(scaleY,0f, elapsed / duration);
            transform.localScale = new Vector3(newScaleX, newScaleY, 1);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localScale = new Vector3(0, 0, 1);
    }


    public void Reset()
    {
        hp = 8;
        cooldown = false;
        StartCoroutine(ScaleUp());
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
                AudioSource.PlayClipAtPoint(blockExplodes, new Vector3(0, 0, 0));
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
