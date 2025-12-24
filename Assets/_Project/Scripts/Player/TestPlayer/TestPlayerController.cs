using DG.Tweening;
using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TestPlayerController : MonoBehaviour, IDamageable, IAmmoStatus, IPunInstantiateMagicCallback, IPunObservable
{
    public float moveSpeed;
    public float runSpeed;
    public float jumpPower;
    public float lookSensitivity;
    public float verticalSensitivityRatio;
    [SerializeField] private float maxHealth;

    public float MaxHealth
    {
        get => maxHealth;
        set
        {
            maxHealth = value < 300 ? value : 300;
            if (pv) pv.RPC("ChangeHealth", RpcTarget.All, CurrentHealth, MaxHealth, PhotonNetwork.LocalPlayer);
        }
    }
    private float currentHealth;
    public float CurrentHealth
    {
        get => currentHealth;
        set
        {
            HpCheck(currentHealth, value);
            currentHealth = value < maxHealth ? value : maxHealth;
            InGameUIManager.Instance.CurrentPlayerHp.text = currentHealth.ToString("0");
            if (pv) pv.RPC("ChangeHealth", RpcTarget.All, CurrentHealth, MaxHealth, PhotonNetwork.LocalPlayer);
        }
    }
    private float maxStamina;
    public float MaxStamina
    {
        get { return maxStamina; }
        set
        {
            maxStamina = value > 20 ? 20 : value;
            InGameUIManager.Instance.PlayerStaminaRefresh();
        }
    }
    private float currentStamina;
    public float CurrentStamina
    {
        get => currentStamina;
        set
        {
            if (value < 0)
                currentStamina = 0;
            else if (maxStamina < value)
                currentStamina = maxStamina;
            else
                currentStamina = value;

            InGameUIManager.Instance.PlayerStaminaRefresh();
        }
    }

    Ray groundRay;
    RaycastHit groundHit;
    Vector2 inputMoveVector;
    Vector3 desireVector3XZ;
    Vector3 moveVector;
    Vector3 lookVector;
    public Vector3 LookVector => lookVector;
    Vector3 gravity;
    float audioHighPassFrequency;
    public bool isRun;
    bool isGround;
    bool isFire;
    bool isAim;
    public bool isReload;
    public bool isHeal;
    Vignette vignette;
    FilmGrain grain;

    public PlayerOutfitController[] playerLookPrefabs;

    TestPlayerMove testPlayerMove;
    TestPlayerTilt testPlayerTilt;
    TestPlayerAim testPlayerAim;
    TestPlayerCrouch testPlayerCrouch;
    public PlayerInput PlayerInput { get; private set; }
    CharacterController cc;
    public CharacterController CC => cc;
    PhotonView pv;
    public PhotonView Pv => pv;
    Transform lookPivot;
    public Transform LookPivot => lookPivot;
    SfxChannelPlayer[] footStepSfx;
    SfxChannelPlayer hitSfx;
    private PlayerOutfitController playerOutfit;

    [HideInInspector] public ProceduralAnimator lookAnimator;
    [HideInInspector] public ProceduralAnimator aimAnimator;
    [HideInInspector] public ProceduralAnimator handAnimator;
    public ProceduralAnimationEffect lookTrackEffect;
    [HideInInspector] public ProceduralAnimator cameraAnimator;
    [HideInInspector] public CameraAnimator cameraShaker;
    [HideInInspector] public AudioHighPassFilter audioHighPassFilter;

    [HideInInspector] public TestInventory inventory;

    public void Init(List<AmmoData> ammoList)
    {
        testPlayerMove = GetComponent<TestPlayerMove>().Init();
        testPlayerTilt = GetComponent<TestPlayerTilt>().Init();
        testPlayerAim = GetComponent<TestPlayerAim>().Init();
        testPlayerCrouch = GetComponent<TestPlayerCrouch>().Init();
        inventory = GetComponentInChildren<TestInventory>();
        footStepSfx = GetComponents<SfxChannelPlayer>().ToList().FindAll(obj => obj.sfxName.Contains("foot")).ToArray();
        foreach (var i in footStepSfx) i.Init();
        hitSfx = GetComponents<SfxChannelPlayer>().ToList().Find(obj => obj.sfxName.Contains("hit")).Init();
        inventory.Init(ammoList, this);
        cc = GetComponent<CharacterController>();
        lookPivot = transform.GetChild(0);
        lookAnimator = GetComponentsInChildren<ProceduralAnimator>().ToList().Find(obj => obj.animationName == "LookPivot");
        aimAnimator = GetComponentsInChildren<ProceduralAnimator>().ToList().Find(obj => obj.animationName == "AimHolder");
        handAnimator = GetComponentsInChildren<ProceduralAnimator>().ToList().Find(obj => obj.animationName == "HandHolder");
        cameraAnimator = GetComponentsInChildren<ProceduralAnimator>().ToList().Find(obj => obj.animationName == "CameraHolder");
        audioHighPassFilter = GetComponentInChildren<AudioHighPassFilter>();
        cameraShaker = GetComponentInChildren<CameraAnimator>();
        PlayerInput = GetComponent<PlayerInput>();
        pv = GetComponent<PhotonView>();
        InputActionMap mainActionMap = PlayerInput.actions.FindActionMap("Player");
        InputAction move = mainActionMap.FindAction("Move");
        InputAction look = mainActionMap.FindAction("Look");
        InputAction tilt = mainActionMap.FindAction("Tilt");
        InputAction run = mainActionMap.FindAction("Run");
        InputAction aim = mainActionMap.FindAction("Aim");
        InputAction crouch = mainActionMap.FindAction("Crouch");
        InputAction jump = mainActionMap.FindAction("Jump");
        InputAction reload = mainActionMap.FindAction("Reload");
        InputAction fire = mainActionMap.FindAction("Fire");
        InputAction escape = mainActionMap.FindAction("Escape");
        InputAction func02 = mainActionMap.FindAction("Func02");

        move.performed += Move;
        move.canceled += Move;

        look.performed += Look;

        tilt.performed += Tilt;
        tilt.canceled += Tilt;

        run.performed += Run;
        run.canceled += Run;

        aim.performed += Aim;
        aim.canceled += Aim;

        crouch.performed += Crouch;
        crouch.canceled += Crouch;

        jump.started += Jump;

        reload.started += Reload;

        fire.started += Fire;
        fire.canceled += Fire;

        escape.started += Escape;

        func02.started += InGameUIManager.Instance.ToggleKeybinding;

        groundRay = new Ray();

        Cursor.lockState = CursorLockMode.Locked;

        maxHealth = 100;
        currentHealth = maxHealth;
        maxStamina = 10;
        currentStamina = maxStamina;

        var profile = FindObjectOfType<Volume>().profile;
        profile.TryGet(out vignette);
        profile.TryGet(out grain);
        vignette.intensity.Override(0f);
        grain.intensity.Override(0f);
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        pv = info.photonView;
        if (pv.IsMine) return;

        cc = GetComponent<CharacterController>();
        GetComponentsInChildren<ProceduralAnimator>().ToList().Find(obj => obj.animationName == "CameraHolder").gameObject.SetActive(false);
        playerOutfit = Instantiate(playerLookPrefabs[PhotonNetwork.LocalPlayer.ActorNumber%4], transform).Init(this);
        playerOutfit.transform.localPosition = Vector3.zero;
        if (PhotonNetwork.IsMasterClient)
            if (!GameManager.Instance.players.Contains(this)) GameManager.Instance.players.Add(this);
        Destroy(GetComponent<PlayerInput>());
    }

    void Escape(InputAction.CallbackContext obj)
    {
        Pause();
        InGameUIManager.Instance.ShowInGameMenu(obj);
    }

    public void Resume()
    {
        PlayerInput.SwitchCurrentActionMap(PlayerInput.defaultActionMap);
    }

    public void Pause()
    {
        PlayerInput.SwitchCurrentActionMap("UI");
    }

    public void SetPerkMod(float moveSpeedMod, float damageMod, float reloadSpeedMod)
    {
        moveSpeed *= moveSpeedMod;
        runSpeed *= moveSpeedMod;
        inventory.GetActiveGun().gunDamage *= damageMod;
        foreach (var i in inventory.GunObjectDict)
        {
            i.Value.SetReloadSpeedMod(reloadSpeedMod);
        }
    }

    private void Update()
    {
        if (pv?.IsMine == false) return;
        isGround = CheckGround();
        if (isGround && gravity.y <= 0f) gravity = Vector3.zero;
        else gravity += Physics.gravity * Time.deltaTime;
        desireVector3XZ = lookPivot.forward;
        desireVector3XZ.y = 0;
        desireVector3XZ = desireVector3XZ.normalized;
        desireVector3XZ *= inputMoveVector.y;
        desireVector3XZ += lookPivot.right * inputMoveVector.x;
        moveVector = Vector3.Lerp(moveVector, desireVector3XZ, Time.deltaTime * 10f);
        
        if (cc.enabled)
        {
            if (testPlayerMove.State == MoveState.Run) cc.Move((moveVector * runSpeed + gravity) * Time.deltaTime);
            else cc.Move((moveVector * moveSpeed + gravity) * Time.deltaTime);
        }

        lookTrackEffect.targetVector3 = Vector3.Lerp(lookTrackEffect.targetVector3, Vector3.zero, Time.deltaTime * 10f);
        lookPivot.rotation = Quaternion.Euler(lookVector);

        if (!isAim) CurrentStamina += 1 * Time.deltaTime;

        audioHighPassFilter.cutoffFrequency = Mathf.Lerp(audioHighPassFilter.cutoffFrequency, 10, Time.deltaTime / 2f);
    }


    bool CheckGround()
    {
        groundRay.origin = transform.position + transform.up * .01f;
        groundRay.direction = Physics.gravity;
        if (Physics.Raycast(groundRay, out groundHit, .02f, LayerMask.GetMask("Ground"))) return true;
        else return false;
    }

    void Move(InputAction.CallbackContext obj)
    {
        Vector2 inputVector2 = obj.ReadValue<Vector2>();
        inputMoveVector.x = inputVector2.x;
        inputMoveVector.y = inputVector2.y;

        if (inputVector2.magnitude < .1f)
        {
            testPlayerMove.State = MoveState.Idle;
        }
        else
        {
            if (isRun && !isHeal)
            {
                testPlayerMove.State = MoveState.Run;
            }
            else testPlayerMove.State = MoveState.Walk;
        }
    }

    void Look(InputAction.CallbackContext obj)
    {
        Vector2 inputVector2 = obj.ReadValue<Vector2>();
        lookVector.y += inputVector2.x * lookSensitivity;
        lookVector.x -= inputVector2.y * lookSensitivity * verticalSensitivityRatio;
        lookTrackEffect.targetVector3.y += inputVector2.x * lookSensitivity;
        lookTrackEffect.targetVector3.x -= inputVector2.y * lookSensitivity * verticalSensitivityRatio;
        lookVector.x = Mathf.Clamp(lookVector.x, -75, 75);
    }

    void Tilt(InputAction.CallbackContext obj)
    {
        float inputFloat = obj.ReadValue<float>();
        if (inputFloat < -0.1f)
        {
            testPlayerTilt.State = TiltState.Left;
            testPlayerAim.State = AimState.Ready;
        }
        else if (inputFloat > 0.1f)
        {
            testPlayerTilt.State = TiltState.Right;
            testPlayerAim.State = AimState.Ready;
        }
        else
        {
            testPlayerTilt.State = TiltState.Center;
            if (isAim && !isHeal)
            {
                testPlayerAim.State = AimState.AimDownSight;
                if (testPlayerMove.State == MoveState.Run)
                    testPlayerMove.State = MoveState.Walk;
            }
            else
            {
                testPlayerAim.State = AimState.Ready;
            }
        }
    }

    void Run(InputAction.CallbackContext obj)
    {
        GunController gunController = inventory.GetActiveGun();
        bool inputBool = !obj.canceled;
        isRun = inputBool && !isHeal;
        if (isRun)
        {
            testPlayerTilt.State = TiltState.Center;
            testPlayerAim.State = AimState.NotReady;
            if (testPlayerMove.State == MoveState.Walk) testPlayerMove.State = MoveState.Run;
            if (testPlayerCrouch.State == StandState.Crouch) testPlayerCrouch.State = StandState.Stand;
            if (isFire)
            {
                isFire = false;
                inventory.GetActiveGun().Fire(false);
            }
            if (gunController.currentMag == 1) gunController.LastShot();
        }
        else
        {
            if (isAim && !isHeal)
            {
                testPlayerAim.State = AimState.AimDownSight;
                if (testPlayerTilt.State != TiltState.Center) testPlayerTilt.State = TiltState.Center;
            }
            else if (testPlayerAim.State == AimState.NotReady) testPlayerAim.State = AimState.Ready;
            if (testPlayerMove.State == MoveState.Run) testPlayerMove.State = MoveState.Walk;
            if (gunController.currentMag == 1) gunController.LastShot();
        }
    }

    void Aim(InputAction.CallbackContext obj)
    {
        bool inputBool = !obj.canceled;
        isAim = inputBool && !isHeal;
        if (isAim)
        {
            testPlayerAim.State = AimState.AimDownSight;
            if (testPlayerMove.State == MoveState.Run) testPlayerMove.State = MoveState.Walk;
            if (testPlayerTilt.State != TiltState.Center) testPlayerTilt.State = TiltState.Center;
        }
        else if (!isAim && isRun)
        {
            isRun = true;
            testPlayerAim.State = AimState.Ready;
            if (testPlayerMove.State == MoveState.Walk) testPlayerMove.State = MoveState.Run;
        }
        else
        {
            testPlayerAim.State = AimState.Ready;
        }
    }

    void Crouch(InputAction.CallbackContext obj)
    {
        bool inputBool = !obj.canceled;
        if (inputBool)
        {
            if (isRun && isGround && CurrentStamina >= 3)
            {
                if (testPlayerMove.State == MoveState.Run) testPlayerMove.State = MoveState.Walk;
                Vector3 back = lookPivot.forward;
                back.y = 0f;
                back = back.normalized;
                gravity = Vector3.up * (jumpPower * 1f)
                        + (-back * (runSpeed * 1.5f));
                CurrentStamina -= 3;
            }
            else if (!isRun)
            {
                testPlayerCrouch.State = StandState.Crouch;
                isRun = false;
                if (testPlayerMove.State == MoveState.Run) testPlayerMove.State = MoveState.Walk;
                cc.height = 0.9f;
                cc.center = new Vector3(cc.center.x, 0.45f, cc.center.z);
            }
        }
        else
        {
            testPlayerCrouch.State = StandState.Stand;
            cc.height = 1.8f;
            cc.center = new Vector3(cc.center.x, 0.9f, cc.center.z);
        }
    }

    void Jump(InputAction.CallbackContext obj)
    {
        bool inputBool = !obj.canceled;
        if (inputBool)
        {
            if (isGround) gravity = Vector3.up * jumpPower;
        }
    }

    void Reload(InputAction.CallbackContext obj)
    {
        bool inputBool = !obj.canceled;
        if (inputBool && !isReload && !isRun && currentStamina >= 3)
        {
            inventory.GetActiveGun().StartReload();
            isReload = true;
            Invoke("ReloadDelay", 1.5f);
        }
        else if (inputBool && isRun && currentStamina >= 10)
        {
            inventory.GetActiveGun().Skill1();
            isHeal = true;
            CurrentStamina -= 10;
            if (testPlayerMove.State == MoveState.Run) testPlayerMove.State = MoveState.Walk;
        }
    }

    void Fire(InputAction.CallbackContext obj)
    {
        bool inputBool = !obj.canceled;
        if (inputBool && isRun)
        {
            isRun = false;
            if (testPlayerMove.State == MoveState.Run)
                testPlayerMove.State = MoveState.Walk;
        }
        isFire = inputBool;
        if (testPlayerMove.State == MoveState.Run) testPlayerMove.State = MoveState.Walk;
        inventory.GetActiveGun().Fire(isFire);
        if (!isFire && isRun) testPlayerMove.State = MoveState.Run;
    }

    public void TakeDamage(Vector3 rayOrigin, Vector3 hitPoint, float damage)
    {
        if (!pv || pv.IsMine)
            TakeDamage(damage);
    }

    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
        hitSfx.PlaySfx();
        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            cc.enabled = false;
            GameManager.Instance.players.Remove(this);
            InGameUIManager.Instance.GameClear();
            if (pv)
            {
                if (pv.IsMine) pv.RPC("OnDead", RpcTarget.Others);
            }
        }
    }

    [PunRPC]
    public void ChangeHealth(float current, float max, Photon.Realtime.Player player)
    {
        InGameUIManager.Instance.RefreshPlayerHealth(current, max, player.ActorNumber);
    }


    [PunRPC]
    public void OnDead()
    {
        GameManager.Instance.players.Remove(this);
        Destroy(playerOutfit.gameObject);
        cc.enabled = false;
    }

    public void PlayDamageEffect(string effectName, float cameraShakeWeight)
    {
        if (!pv || pv.IsMine)
        {
            cameraShakeWeight = cameraShakeWeight > .5f ? cameraShakeWeight : .5f;
            cameraShaker.AddAnimation(effectName, cameraShakeWeight);
            if (effectName == "MeleeHit") InGameUIManager.Instance.HitFX();
            if (effectName == "Interrupt") audioHighPassFilter.cutoffFrequency = 5000;
        }
    }

    public Dictionary<AmmoData, int> GetAllAmmo()
    {
        return inventory.AmmoDict;
    }

    public int GetAmmo()
    {
        return inventory.GetAmmo();
    }

    public int GetMagAmmo()
    {
        return inventory.GetMagAmmo();
    }

    void OnTriggerEnter(Collider other)
    {
        if (pv && !pv.IsMine) return;

        GunController gunController = inventory.GetActiveGun();
        if (other.CompareTag("ItemA")) gunController.ItemAA();
        if (other.CompareTag("ItemB"))
        {
            for (int i = 0; i < 2; i++) inventory.AddActiveAmmo();
            gunController.ItemBB();
        }

        if (other.CompareTag("ItemC")) gunController.ItemCC();
        if (other.CompareTag("ItemY")) gunController.ItemYY();
        if (other.CompareTag("ItemZ")) gunController.ItemZZ();
    }

    public void ReloadDelay()
    {
        isReload = false;
    }

    public void BanDong(float ups, float rights = 0f)
    {
        lookVector.x = Mathf.Clamp(lookVector.x - ups, -75f, 75f);
        lookVector.y += rights;
    }

    void HpCheck(float currentHp, float estimatedHp)
    {
        if (currentHp < 50 && 50 <= estimatedHp)
        {
            DOTween.To(() => vignette.intensity.value, x => vignette.intensity.value = x, 0f, 1f)
                .SetUpdate(true)
                .SetEase(Ease.OutCubic);
            DOTween.To(() => grain.intensity.value, x => grain.intensity.value = x, 0f, 1f)
                .SetUpdate(true)
                .SetEase(Ease.OutCubic);
        }
        else if (estimatedHp < 50 && 50 <= currentHp)
        {
            DOTween.To(() => vignette.intensity.value, x => vignette.intensity.value = x, .444f, 1f)
                .SetUpdate(true)
                .SetEase(Ease.OutCubic);
            DOTween.To(() => grain.intensity.value, x => grain.intensity.value = x, .2f, 1f)
                .SetUpdate(true)
                .SetEase(Ease.OutCubic);
        }
        else if (estimatedHp < 20 && 20 <= currentHp)
        {
            DOTween.To(() => vignette.intensity.value, x => vignette.intensity.value = x, .666f, 1f)
                .SetUpdate(true)
                .SetEase(Ease.OutCubic);
            DOTween.To(() => grain.intensity.value, x => grain.intensity.value = x, 1f, 1f)
                .SetUpdate(true)
                .SetEase(Ease.OutCubic);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            if (pv.IsMine) stream.SendNext(lookVector);
        }
        else if (stream.IsReading)
        {
            if (!pv.IsMine) lookVector = (Vector3)stream.ReceiveNext();
        }
    }

    [PunRPC]
    private void EjectLineRenderer(Vector3 hitPoint)
    {
        playerOutfit.EjectLineRenderer(hitPoint);
    }
}
