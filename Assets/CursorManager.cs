using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public Texture2D cursorTexture;

    void Start()
    {
        if (cursorTexture != null)
        {
            Vector2 hotspot = new Vector2(cursorTexture.width / 2.0f, cursorTexture.height / 2.0f);
            Cursor.SetCursor(cursorTexture, hotspot, CursorMode.Auto);
        }
    }
}
