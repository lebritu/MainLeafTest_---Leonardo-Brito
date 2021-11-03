using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPool : MonoBehaviour
{
    // Aqui fica armazenado todos os audios do jogo

    public float volume, volume_musica;
    public AudioSource musica_play, efeito_sonoro_play;

    public AudioClip land, pega_cristal_verde, pega_cristal_azul, hey_listen, guarda_hey;
    public AudioClip[] hit_madeira;

    public void PlayAudio(AudioClip ac)
    {
        efeito_sonoro_play.PlayOneShot(ac, volume);
    }
}
