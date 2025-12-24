using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GunData", menuName = "Scriptables/GunData")]
public class GunData : ScriptableObject
{
    public string gunName;
    public GameObject FPSGunPrefab;
    public AmmoData ammoData;
}
