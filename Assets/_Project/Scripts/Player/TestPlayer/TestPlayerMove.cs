using DG.Tweening;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestPlayerMove : StateMachine<MoveState>
{
    TestPlayerController pc;
    Camera mCam;
    Camera oCam;

    public TestPlayerMove Init()
    {
        pc = GetComponent<TestPlayerController>();
        mCam = GameObject.Find("Main Camera").GetComponent<Camera>();
        oCam = GameObject.Find("Overlay Camera").GetComponent<Camera>();
        return this;
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void OnBeginState(MoveState state)
    {
        switch (state)
        {
            case MoveState.Idle:
                pc.handAnimator.AddAnimationTransition("Idle", 1, .5f, EaseType.OutCubic);
                break;
            case MoveState.Walk:
                pc.handAnimator.AddAnimationTransition("Walk", 1, .5f, EaseType.OutCubic);
                break;
            case MoveState.Run:
                oCam.DOFieldOfView(60f, 0.25f).SetEase(Ease.OutCubic);
                mCam.DOFieldOfView(60f, 0.25f).SetEase(Ease.OutCubic);
                pc.handAnimator.AddAnimationTransition("Run", 1, .5f, EaseType.OutCubic);
                break;
        }
    }

    protected override void OnState(MoveState state)
    {
        base.OnState(state);
    }

    protected override void OnEndState(MoveState state)
    {
        switch (state)
        {
            case MoveState.Idle:
                pc.handAnimator.AddAnimationTransition("Idle", 0, .5f, EaseType.OutCubic);
                break;
            case MoveState.Walk:
                pc.handAnimator.AddAnimationTransition("Walk", 0, .5f, EaseType.OutCubic);
                break;
            case MoveState.Run:
                pc.handAnimator.AddAnimationTransition("Run", 0, .5f, EaseType.OutCubic);
                break;
        }
    }
}

public enum MoveState
{
    Idle,
    Walk,
    Run,
}
