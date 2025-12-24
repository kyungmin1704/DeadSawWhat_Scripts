using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInventory : MonoBehaviour
{
    private TestPlayerController pc;
    protected List<GunData> gunDatas;
    private int activeGunIndex;

    public int ActiveGunIndex
    {
        get => activeGunIndex;
        set
        {
            activeGunIndex = value;
        }
    }
    private Dictionary<GunData, GunController> gunObjectDict;
    public Dictionary<GunData, GunController> GunObjectDict { get => gunObjectDict; }
    private Dictionary<AmmoData, int> ammoDict;
    public Dictionary<AmmoData, int> AmmoDict { get => ammoDict; }

    

    public void Init(List<AmmoData> ammoDatas, TestPlayerController pc)
    {
        this.pc = pc;
        gunDatas = new List<GunData>();
        gunObjectDict = new Dictionary<GunData, GunController>();
        ammoDict = new Dictionary<AmmoData, int>();
        foreach (AmmoData i in ammoDatas)
        {
            ammoDict.Add(i, i.AmmoAmount);
        }
    }
    
    public void AddGunDatas(GunData gunData)
    {
        gunDatas.Add(gunData);
        gunObjectDict.Add(gunData, Instantiate(gunData.FPSGunPrefab, transform).GetComponent<GunController>());
        gunObjectDict[gunData].Init(ammoDict, pc);
    }

    public void AddAmmo(AmmoData ammoData, int ammoAmount)
    {
        ammoDict[ammoData] += ammoAmount;
    }

    public void AddActiveAmmo()
    {
        ammoDict[GetActiveGun().gunData.ammoData] += GetActiveGun().gunData.ammoData.addAmmoAmount;
    }

    public GunController GetActiveGun()
    {
        return gunObjectDict[gunDatas[activeGunIndex]];
    }

    public int GetAmmo()
    {
        return ammoDict[gunDatas[activeGunIndex].ammoData];
    }

    public int GetMagAmmo()
    {
        return gunObjectDict[gunDatas[activeGunIndex]].currentMag;
    }
}
