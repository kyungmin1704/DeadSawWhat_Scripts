using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISoundManager : Singleton<UISoundManager>
{
    public AudioSource audioSource;

    public AudioClip hoverSound;
    public AudioClip clickSound;

    public void HoverSoundPlay()
    {
        audioSource.PlayOneShot(hoverSound);
    }
    public void ClickSoundPlay()
    {
        audioSource.PlayOneShot(clickSound);
    }

}

