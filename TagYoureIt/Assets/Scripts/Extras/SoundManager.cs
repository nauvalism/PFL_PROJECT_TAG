using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] AudioSource source;
    [SerializeField] List<AudioClip> clips;
    [SerializeField] float volumeMultiplier;
    [SerializeField] string ppKey;

    private void Awake() {
        
    }

    private void OnValidate() {
        if(PlayerPrefs.GetFloat(ppKey, -1) == -1)
        {
            source.volume = 1 * volumeMultiplier;
        }
        else
        {
            source.volume = PlayerPrefs.GetFloat(ppKey, -1);
        }
    }

    private void Start() {
        
    }

    public virtual void Play(int clipIndex)
    {
        source.Stop();
        source.clip = clips[clipIndex];
        source.Play();
    }

    public virtual void PlayRandomBetween(int a, int b)
    {
        source.Stop();
        source.clip = clips[Random.Range(a,(b+1))];
        source.Play();
    }

    public virtual void Stop()
    {
        source.Stop();
    }

    public virtual void FadeOut(System.Action after, float duration = 1.0f)
    {
        LeanTween.cancel(source.gameObject);
        LeanTween.value(source.gameObject, source.volume, .0f, duration).setOnUpdate((float f)=>{
            source.volume = f * volumeMultiplier;
        }).setOnComplete(()=>{
            if(after != null)
            {
                after();
            }
        });
    }

    public virtual void FadeIn(System.Action after, float duration)
    {
        LeanTween.cancel(source.gameObject);
        LeanTween.value(source.gameObject, source.volume, 1.0f, duration).setOnUpdate((float f)=>{
            source.volume = f * volumeMultiplier;
        }).setOnComplete(()=>{
            if(after != null)
            {
                after();
            } 
        });
    }
}
