using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animacao_eventos : MonoBehaviour
{
    public AudioClip clip;

    private SoundPool SP;

    void Start()
    {
        SP = FindObjectOfType<SoundPool>();
    }

    public void Audio()
    {
        SP.PlayAudio(clip);
    }
}
