using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] AudioSource _audio;


    public void Explode()
    {
        anim.Play("Explode",-1,0);
        _audio.Play();
    }

}
