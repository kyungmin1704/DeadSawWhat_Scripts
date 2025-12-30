using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AmmoData", menuName = "Scriptables/AmmoData")]
public class AmmoData : ScriptableObject
{
    public string ammoName;

    private int ammoAmount;
    public int AmmoAmount
    {
        get => ammoAmount;
        set
        {
            ammoAmount = value;
            ammoAmount = ammoAmount > maxAmmoStack ? maxAmmoStack : ammoAmount;
        }
    }

    public int maxAmmoStack;
    public int addAmmoAmount;
}
