using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorClips : MonoBehaviour
{
    [SerializeField]AudioSource source;
    [SerializeField] List<AudioClip> clips;

    private void OnValidate() {
        source = GetComponent<AudioSource>();
    }

    public void PlayClip(int index)
    {
        source.clip = clips[index];
        source.Play();
    }

    public void PlayClipWithManner(int index)
    {
        if(source.isPlaying)
        {
            if(source.clip == clips[index])
            {
                return;
            }
            else
            {
                source.Stop();
                source.clip = clips[index];
                source.Play();
            }
        }
        else
        {
            source.Stop();
            source.clip = clips[index];
            source.Play();
        }
    }

    public void StopClip()
    {
        source.Stop();
    }
}
