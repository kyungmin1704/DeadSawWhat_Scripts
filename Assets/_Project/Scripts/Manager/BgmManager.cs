using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;

public class BgmManager : Singleton<BgmManager>
{
    public AudioMixer audioMixer;
    public AudioClip[] bgm;

    private AudioSource[] bgmPlayer;

    protected override void Awake()
    {
        base.Awake();

        bgmPlayer = new AudioSource[bgm.Length];
        for (int i = 0; i < bgm.Length; i++)
        {
            bgmPlayer[i] = new GameObject().AddComponent<AudioSource>();
            bgmPlayer[i].transform.SetParent(transform);
            bgmPlayer[i].clip = bgm[i];
            bgmPlayer[i].loop = true;
            bgmPlayer[i].outputAudioMixerGroup = audioMixer.FindMatchingGroups("Master")[1];
            bgmPlayer[i].volume = 0.0f;
            bgmPlayer[i].Play();
        }
    }

    public void FadeInBgm(BgmType bgm, bool isRestart)
    {
        bgmPlayer[(int)bgm].Play();
        bgmPlayer[(int)bgm].DOFade(1f, 2f);
        if (isRestart) bgmPlayer[(int)bgm].PlayScheduled(0.0);
    }

    public void FadeOutBgm(BgmType bgm, bool isPause)
    {
        Tween t = bgmPlayer[(int)bgm].DOFade(0f, 2f);
        if (isPause)
        {
            t.OnComplete(
                () => bgmPlayer[(int)bgm].Pause()
                );
        }
        
    }
}

public enum BgmType
{
    Menu = 0,
    Game = 1,
}
