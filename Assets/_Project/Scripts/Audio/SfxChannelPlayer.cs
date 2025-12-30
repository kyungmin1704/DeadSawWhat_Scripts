using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SfxChannelPlayer : MonoBehaviour
{
    public string sfxName;
    public AudioClip[] clips;
    public int channels;
    [Range(0, 1)] public float baseVolume;
    [Range(0, 1)] public float sfx3dBlend;
    private AudioSource[] sources;
    private int sourceIndex;
    
    public SfxChannelPlayer Init()
    {
        if (sources != null) return this;
        sourceIndex = 0;
        sources = new AudioSource[channels];
        for (int i = 0; i < channels; i++)
        {
            sources[i] = new GameObject("AudioSource").AddComponent<AudioSource>();
            sources[i].playOnAwake = false;
            sources[i].loop = false;
            sources[i].volume = baseVolume;
            sources[i].spatialBlend = sfx3dBlend;
            sources[i].rolloffMode = AudioRolloffMode.Linear;
            sources[i].maxDistance = 50f;
            sources[i].outputAudioMixerGroup = GameManager.Instance.audioMixer.FindMatchingGroups("Master")[0];
            sources[i].transform.parent = transform;
            sources[i].transform.localPosition = Vector3.zero;
        }

        return this;
    }

    public void PlaySfx()
    {
        sources[sourceIndex].clip = clips[Random.Range(0, clips.Length)];
        sources[sourceIndex].Play();
        sourceIndex = (sourceIndex + 1) % channels;
    }
}
