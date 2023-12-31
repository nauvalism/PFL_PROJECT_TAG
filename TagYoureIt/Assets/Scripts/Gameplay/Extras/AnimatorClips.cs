using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorClips : MonoBehaviour
{
    AudioSource source;
    [SerializeField] List<AudioClip> clips;

    private void OnValidate() {
        source = GetComponent<AudioSource>();
    }

    public void PlayClip(int index)
    {
        source.clip = clips[index];
        source.Play();
    }
}
