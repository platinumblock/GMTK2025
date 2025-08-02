using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTurret : MonoBehaviour
{
    // Start is called before the first frame update
    public Camera lowResCamera; // The camera rendering to the low-res RenderTexture
    public RenderTexture renderTexture; // The low-res target texture
    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        if (!RoundManager.transitioning)
        {
            Rotation();
        }
        
    }

    void Rotation()
    {
        Vector3 mousePos = Input.mousePosition;

        float scaleX = (float)renderTexture.width / Screen.width;
        float scaleY = (float)renderTexture.height / Screen.height;
        mousePos = new Vector3(mousePos.x * scaleX, mousePos.y * scaleY, 0f);
        mousePos = lowResCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, lowResCamera.nearClipPlane));
        mousePos.z = 0f;
        
        Vector3 direction = mousePos - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;

        Player.angles.Add(angle);
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
}
