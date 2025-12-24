using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class TestPlayerTilt : StateMachine<TiltState>
{
    TestPlayerController pc;
    Camera mcam;
    Camera ocam;

    public TestPlayerTilt Init()
    {
        pc = GetComponent<TestPlayerController>();
        mcam = GameObject.Find("Main Camera").GetComponent<Camera>();
        ocam = GameObject.Find("Overlay Camera").GetComponent<Camera>();
        return this;
    }

    protected override void OnBeginState(TiltState state)
    {
        switch (state)
        {
            case TiltState.Left:
                pc.handAnimator.AddAnimationTransition("LeftTilt", 1f, .5f, EaseType.OutCubic);
                pc.cameraAnimator.AddAnimationTransition("LeftTilt", 1f, .5f, EaseType.OutCubic);
                break;
            case TiltState.Center:
                mcam.transform.DOLocalMoveX(0, 0.25f).SetEase(Ease.OutCubic);
                ocam.transform.DOLocalMoveX(0, 0.25f).SetEase(Ease.OutCubic);
                break;
            case TiltState.Right:
                pc.handAnimator.AddAnimationTransition("RightTilt", 1f, .5f, EaseType.OutCubic);
                pc.cameraAnimator.AddAnimationTransition("RightTilt", 1f, .5f, EaseType.OutCubic);
                break;
        }
    }

    protected override void OnEndState(TiltState state)
    {
        switch (state)
        {
            case TiltState.Left:
                pc.handAnimator.AddAnimationTransition("LeftTilt", 0f, .5f, EaseType.OutCubic);
                pc.cameraAnimator.AddAnimationTransition("LeftTilt", 0f, .5f, EaseType.OutCubic);
                break;
            case TiltState.Center:
                break;
            case TiltState.Right:
                pc.handAnimator.AddAnimationTransition("RightTilt", 0f, .5f, EaseType.OutCubic);
                pc.cameraAnimator.AddAnimationTransition("RightTilt", 0f, .5f, EaseType.OutCubic);
                break;
        }
    }
}

public enum TiltState
{
    Left,
    Center,
    Right,
}
