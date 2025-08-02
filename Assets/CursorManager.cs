using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public Texture2D cursorTexture;
    public Vector2 hotspot = Vector2.zero;

    void Start()
    {
        // Check if the cursor texture is assigned
        if (cursorTexture != null)
        {
            // Set the cursor using the texture and hotspot
            Cursor.SetCursor(cursorTexture, hotspot, CursorMode.Auto);
        }
    }
}
