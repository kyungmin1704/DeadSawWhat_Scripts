using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TestPlayerAim : StateMachine<AimState>
{
    TestPlayerController pc;
    Camera mCam;
    Camera oCam;

    public TestPlayerAim Init()
    {
        pc = GetComponent<TestPlayerController>();
        mCam = GameObject.Find("Main Camera").GetComponent<Camera>();
        oCam = GameObject.Find("Overlay Camera").GetComponent<Camera>();
        return this;
    }

    protected override void OnBeginState(AimState state)
    {
        switch (state)
        {
            case AimState.NotReady:
                oCam.DOFieldOfView(60f, 0.25f).SetEase(Ease.OutCubic);
                mCam.DOFieldOfView(60f, 0.25f).SetEase(Ease.OutCubic);
                break;
            case AimState.Ready:
                oCam.DOFieldOfView(60f, 0.25f).SetEase(Ease.OutCubic);
                mCam.DOFieldOfView(60f, 0.25f).SetEase(Ease.OutCubic);
                break;
            case AimState.AimDownSight:
                oCam.DOFieldOfView(40f, 0.25f).SetEase(Ease.OutCubic);
                mCam.DOFieldOfView(40f, 0.25f).SetEase(Ease.OutCubic);
                pc.aimAnimator.AddAnimationTransition("Aiming", 1, .5f, EaseType.OutCubic);
                break;
        }
    }

    protected override void OnEndState(AimState state)
    {
        switch (state)
        {
            case AimState.NotReady:
                break;
            case AimState.Ready:
                break;
            case AimState.AimDownSight:
                pc.aimAnimator.AddAnimationTransition("Aiming", 0, .5f, EaseType.OutCubic);
                break;
        }
    }
}

public enum AimState
{
    NotReady,
    Ready,
    AimDownSight,
}
