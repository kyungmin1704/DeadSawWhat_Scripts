using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TestPlayerCrouch : StateMachine<StandState>
{
    TestPlayerController pc;

    public TestPlayerCrouch Init()
    {
        pc = GetComponent<TestPlayerController>();
        return this;
    }

    protected override void OnBeginState(StandState state)
    {
        switch (state)
        {
            case StandState.Stand:
                break;
            case StandState.Crouch:
                pc.handAnimator.AddAnimationTransition("Crouch", 1, .5f, EaseType.OutCubic);
                pc.lookAnimator.AddAnimationTransition("Crouch", 1, .5f, EaseType.OutCubic);
                pc.aimAnimator.AddEffectTransition("Aiming", "Crouch", 1, .5f, EaseType.OutCubic);
                break;
        }
    }

    protected override void OnEndState(StandState state)
    {
        switch (state)
        {
            case StandState.Stand:
                break;
            case StandState.Crouch:
                pc.handAnimator.AddAnimationTransition("Crouch", 0, .5f, EaseType.OutCubic);
                pc.lookAnimator.AddAnimationTransition("Crouch", 0, .5f, EaseType.OutCubic);
                pc.aimAnimator.AddEffectTransition("Aiming", "Crouch", 0, .5f, EaseType.OutCubic);
                break;
        }
    }
}

public enum StandState
{
    Stand,
    Crouch,
}
