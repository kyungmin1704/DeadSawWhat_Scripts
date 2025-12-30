using UnityEngine;

[CreateAssetMenu(fileName = "OptionButtonData", menuName = "Scriptable Object/OptionButton")]
public class OptionButtonData : ScriptableObject
{
    public string optionName;
    [TextArea(3, 3)]
    public string description;

    public float ftValue = -999f;
    public int itgValue = -999;
    public bool btnOn;
}
