using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "OptionButtonVisualData", menuName = "Scriptable Object/Option")]
public class OptionButtonVisualData : ScriptableObject
{
    //public Sprite normalSprite;
    //public Sprite selectedSprite;
    public Color originalColor;
    public Color highlightColor = new Color32(255, 144, 0, 255);
}
