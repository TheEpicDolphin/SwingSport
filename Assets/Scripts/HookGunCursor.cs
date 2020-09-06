using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HookGunCursor : MonoBehaviour
{
    public Image cursorImage;

    public void setCursorColor(Color c)
    {
        cursorImage.color = c;
    }
}
