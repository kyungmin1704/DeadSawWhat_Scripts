using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PerkData", menuName = "Scriptables/PerkData")]
public class PerkData : ScriptableObject
{
    [Tooltip("반드시 고유한 번호여야 합니다.")] public int perkID;
    public string perkName;
    public string perkDescription;
    public Sprite perkIcon;
    public float moveSpeedMultiplier;
    public float damageMultiplier;
    public float reloadSpeedMultiplier;

    public static float GetMoveSpeedMultiplier(List<PerkData> perks)
    {
        float result = 0;
        foreach (PerkData i in perks)
        {
            result += i.moveSpeedMultiplier;
        }
        result++;
        return result;
    }
    
    public static float GetDamageMultiplier(List<PerkData> perks)
    {
        float result = 0;
        foreach (PerkData i in perks)
        {
            result += i.damageMultiplier;
        }
        result++;
        return result;
    }
    
    public static float GetReloadSpeedMultiplier(List<PerkData> perks)
    {
        float result = 0;
        foreach (PerkData i in perks)
        {
            result += i.reloadSpeedMultiplier;
        }
        result++;
        return result;
    }
}
