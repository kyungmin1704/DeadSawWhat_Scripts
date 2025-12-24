using DG.Tweening;
using Lean.Pool;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GunController : MonoBehaviour
{
    public GunData gunData;
    public int magSize;
    public int currentMag;
    public float gunDamage;
    public Transform muzzle;
    public SfxChannelPlayer muzzleSound;
    public GameObject gunShotFx;
    public GameObject gunImpactFx;

    public SfxChannelPlayer magLoadSfx;
    public SfxChannelPlayer magUnloadSfx;
    public SfxChannelPlayer dryFireSfx;
    
    private Animator gunAnimator;
    private TestPlayerController pc;
    private ItemManagerTest it;

    private Dictionary<AmmoData, int> ammoDict;
    private Ray muzzleRay;
    private RaycastHit muzzleHit;

    public GameObject cartridge;
    public GameObject druggy;
    private Transform cylinder;
    private Transform lefthand;
    public GameObject tracerPrefab;

    Vector3 gravity;
    ColorAdjustments skillColour;
    bool isSkill2;


    public void Init(Dictionary<AmmoData, int> ammoDict, TestPlayerController pc)
    {
        cylinder = GameObject.Find("01").GetComponent<Transform>();
        lefthand = GameObject.Find("Items").GetComponent<Transform>();
        it = GameObject.Find("ItemMaker").GetComponent<ItemManagerTest>();

        this.pc = pc;
        gunAnimator = GetComponent<Animator>();
        this.ammoDict = ammoDict;

        muzzleSound.Init();
        magLoadSfx.Init();
        magUnloadSfx.Init();
        dryFireSfx.Init();

        InGameUIManager.Instance.SetEquipmentName(gunData.gunName);
        InGameUIManager.Instance.AmmoRefresh(currentMag, ammoDict[gunData.ammoData]);

        var profile = FindObjectOfType<Volume>().profile;
        profile.TryGet(out skillColour);
        skillColour.saturation.Override(0f);
        skillColour.contrast.Override(0f);
    }

    public void SetReloadSpeedMod(float reloadSpeedMultiplier)
    {
        gunAnimator.SetFloat("ReloadSpeed", reloadSpeedMultiplier);
    }

    public void StartReload()
    {
        if (currentMag < magSize && ammoDict[gunData.ammoData] > 0)
        {
            gunAnimator.SetTrigger("Reload");
            pc.CurrentStamina -= 3;
        }
    }

    public void Fire(bool isFire)
    {
        if (currentMag > 0)
        {
            gunAnimator.SetBool("Fire", isFire);
        }
        else
        {
            if (isFire) dryFireSfx.PlaySfx();
        }
        
    }

    public void EjectBullet()
    {
        if (currentMag <= 0)
        {
            dryFireSfx.PlaySfx();
            gunAnimator.SetBool("Fire", false);
            return;
        }
        EjectCartridge();

        pc.cameraShaker.AddAnimation("GunFireShake", Time.timeScale);
        currentMag -= 1;
        GameObject obj = Instantiate(gunShotFx, muzzle.position, muzzle.rotation);
        Destroy(obj, 5f);
        muzzleRay.origin = muzzle.position;
        muzzleRay.direction = muzzle.forward;

        pc.BanDong(0.6f, Random.Range(-0.2f, 0.2f));

        if (Physics.Raycast(muzzleRay, out muzzleHit, 100f))
        {
            EjectLineRenderer();
            IDamageable damageable = muzzleHit.collider.GetComponent<IDamageable>();
            if (muzzleHit.collider.gameObject.CompareTag("Player")) damageable = null;
            damageable?.TakeDamage(muzzleRay.origin, muzzleHit.point, gunDamage);
            if (!muzzleHit.collider.CompareTag("Enemy"))
            {
                LeanPool.Despawn(LeanPool.Spawn(gunImpactFx, muzzleHit.point, Quaternion.LookRotation(muzzleHit.normal)), 3f);
            }

            if (isSkill2 && currentMag == 0)
            {
                DOTween.To(() => skillColour.contrast.value, v => skillColour.contrast.value = v, 100f, 0.2f).SetUpdate(true).SetEase(Ease.OutCubic);
                Invoke("LastShotTrigger", 0.3f);
                damageable?.TakeDamage(muzzleRay.origin, muzzleHit.point, gunDamage * 3f);
                pc.cameraShaker.AddAnimation("GunFireShake", 5f);
                pc.cameraShaker.AddAnimation("Vib", 2f);
                
                float origin = 0f;
                DOTween.To(() => 0f, v => {
                    float push = v - origin;
                    origin = v;
                    pc.CC.Move(-pc.transform.forward * push);
                }, 3f, 0.5f)
                .SetEase(Ease.OutCubic)
                .SetUpdate(true);

                pc.BanDong(2f, Random.Range(0f, 0f));

                isSkill2 = false;
                StartReload();
                pc.isReload = true;
                Invoke("ReloadDelay1", 1.5f);
            }
        }
        
        pc.BanDong(0.6f, Random.Range(-0.2f, 0.2f));
        
        muzzleSound.PlaySfx();
        
        InGameUIManager.Instance.AmmoRefresh(currentMag, ammoDict[gunData.ammoData]);
    }

    public void Reload()
    {
        int reqAmmo = magSize - currentMag;
        if (ammoDict[gunData.ammoData] < reqAmmo)
        {
            currentMag = ammoDict[gunData.ammoData];
            ammoDict[gunData.ammoData] = 0;
        }
        else
        {
            currentMag = magSize;
            ammoDict[gunData.ammoData] -= reqAmmo;
        }

        magLoadSfx.PlaySfx();

        InGameUIManager.Instance.AmmoRefresh(currentMag, ammoDict[gunData.ammoData]);
    }

    public void Unload()
    {
        magUnloadSfx.PlaySfx();
    }

    public void Vib()
    {
        pc.cameraShaker.AddAnimation("Vib", 1f);
    }

    public void EjectLineRenderer()
    {
        var tr = LeanPool.Spawn(tracerPrefab);
        var lr = tr.GetComponent<LineRenderer>();
        var start = muzzle.position;
        var end = (muzzleHit.collider ? muzzleHit.point : start + muzzle.forward * 100f);
        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        if (isSkill2 && currentMag == 0)
        {
            lr.widthMultiplier = 5f;
            DOTween.To(() => lr.widthMultiplier, v => lr.widthMultiplier = v, 0f, 1.5f).SetUpdate(true).OnComplete(() => LeanPool.Despawn(tr));
        }
        else
        {
            lr.widthMultiplier = 0.5f;
            DOTween.To(() => lr.widthMultiplier, v => lr.widthMultiplier = v, 0f, 0.07f).SetUpdate(true).OnComplete(() => LeanPool.Despawn(tr));
        }

        if (PhotonNetwork.InRoom)
        {
            pc.Pv.RPC("EjectLineRenderer", RpcTarget.Others, muzzleHit.point);
        }
    }
    
    public void EjectCartridge()
    {
        GameObject bulletP = LeanPool.Spawn(cartridge, cylinder.position, Quaternion.Euler(cylinder.rotation.eulerAngles));
        Rigidbody br = bulletP.GetComponent<Rigidbody>();
        br.velocity = Vector3.zero;
        br.angularVelocity = Vector3.zero;
        Vector3 direction = cylinder.right * 1.5f + cylinder.up * 0.5f + Random.onUnitSphere * .2f;
        br.AddForce(direction, ForceMode.Impulse);
        Vector3 spin = bulletP.transform.forward * 15f + Random.onUnitSphere * 5f;
        br.AddTorque(spin, ForceMode.Impulse);
        LeanPool.Despawn(bulletP, 0.3f);
    }

    public void Skill1()
    {
        gunAnimator.SetTrigger("Heal");
        StartCoroutine(MakeSkillItem(druggy));
    }
    IEnumerator MakeSkillItem(GameObject whatitem)
    {
        yield return new WaitForSeconds(0.5f);
        GameObject healSkill = LeanPool.Spawn(whatitem, lefthand);
        StartCoroutine(UsingSkill(healSkill));
    }
    IEnumerator UsingSkill(GameObject thisitem)
    {
        yield return new WaitForSeconds(1f);
        LeanPool.Despawn(thisitem);
        pc.isHeal = false;

        float slowed = 0.1f;
        Time.timeScale = slowed;
        Time.fixedDeltaTime = 0.02f * slowed;
        gravity = Physics.gravity;
        Physics.gravity = gravity / (slowed * slowed);
        pc.jumpPower *= 1f / slowed;

        pc.handAnimator.isUnscaledTime = true;
        pc.lookAnimator.isUnscaledTime = true;
        pc.aimAnimator.isUnscaledTime = true;
        pc.cameraAnimator.isUnscaledTime = true;
        gunAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
        pc.moveSpeed *= 1f / slowed;
        pc.runSpeed *= 1f / slowed;

        DOTween.To(() => skillColour.saturation.value, v => skillColour.saturation.value = v, -50f, 0.2f).SetUpdate(true).SetEase(Ease.OutCubic);

        yield return new WaitForSecondsRealtime(5f);
        Skill1End();
    }
    void Skill1End()
    {
        float slowed = Mathf.Max(0.0001f, Time.timeScale);
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        pc.moveSpeed *= slowed;
        pc.runSpeed *= slowed;
        Physics.gravity = gravity;
        pc.jumpPower *= slowed;

        pc.handAnimator.isUnscaledTime = false;
        pc.lookAnimator.isUnscaledTime = false;
        pc.aimAnimator.isUnscaledTime = false;
        pc.cameraAnimator.isUnscaledTime = false;
        gunAnimator.updateMode = AnimatorUpdateMode.Normal;

        DOTween.To(() => skillColour.saturation.value, v => skillColour.saturation.value = v, 0f, 0.2f).SetUpdate(true).SetEase(Ease.OutCubic);

        pc.GetType().GetField("gravity", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(pc, Vector3.zero);
        pc.GetType().GetField("moveVector", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(pc, Vector3.zero);
        pc.GetType().GetField("desireVector3XZ", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(pc, Vector3.zero);
    }

    public void LastShot()
    {
        if (pc.isRun) isSkill2 = true;
        else if (!pc.isRun) isSkill2 = false;
    }
    void LastShotTrigger()
    {
        DOTween.To(() => skillColour.contrast.value, v => skillColour.contrast.value = v, 0f, 0.2f).SetUpdate(true).SetEase(Ease.OutCubic);
    }


    public void ItemAA()
    {
        gunAnimator.SetTrigger("ItemGet");
        pc.CurrentHealth += 20;
        pc.CurrentStamina = pc.MaxStamina;
    }
    public void ItemBB()
    {
        gunAnimator.SetTrigger("ItemGet");
        
        InGameUIManager.Instance.AmmoRefresh(currentMag, ammoDict[gunData.ammoData]);
    }
    public void ItemCC()
    {
        gunAnimator.SetTrigger("ItemGet");
        if (pc.MaxHealth >= 200f || pc.MaxStamina >= 20f)
        {
            it.ItemSpawn();
            it.ItemSpawn();
        }
        else
        {
            pc.MaxHealth += 20;
            pc.MaxStamina += 2;
        }
    }
    public void ItemYY()
    {
        gunAnimator.SetTrigger("ItemGet");
        currentMag += 30;
        if (currentMag > 30) currentMag = 30;
        InGameUIManager.Instance.AmmoRefresh(currentMag, ammoDict[gunData.ammoData]);
    }
    public void ItemZZ()
    {
        gunAnimator.SetTrigger("ItemGet");
        pc.CurrentHealth += 2;
    }

    void ReloadDelay1()
    {
        pc.isReload = false;
    }
}
