using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 피해를 받을 수 있는 오브젝트의 클래스에 구현해야할 인터페이스입니다.
/// TakeDamage 메서드를 구현해야 합니다.
/// </summary>
public interface IDamageable
{
    public void TakeDamage(Vector3 rayOrigin, Vector3 hitPoint, float damage);
    public void TakeDamage(float damage);
    public void PlayDamageEffect(string effectName, float cameraShakeWeight);
}

/// <summary>
/// 상호작용 가능한 오브젝트의 클래스에 구현해야할 인터페이스입니다.
/// </summary>
public interface IInteractable
{
    public void Interact();
}

public interface IAmmoStatus
{
    /// <summary>
    /// 모든 총알의 정보를 반환하는 메서드
    /// </summary>
    public Dictionary<AmmoData, int> GetAllAmmo();

    /// <summary>
    /// 현재 총의 사용 가능한 총알의 갯수를 반환하는 메서드
    /// </summary>
    public int GetAmmo();

    /// <summary>
    /// 현재 총의 탄창에 남은 총알의 갯수를 반환하는 메서드
    /// </summary>
    /// <returns></returns>
    public int GetMagAmmo();
}

public interface IInitializable
{
    public void Init();
}
