using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
	bool showCursor = false;

	public void ShowCursor (bool val) { showCursor = val;}

    // Update is called once per frame
    void Update()
    {
        Cursor.visible = showCursor;
    }
}
