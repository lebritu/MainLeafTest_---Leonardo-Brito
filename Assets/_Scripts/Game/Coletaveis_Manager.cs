using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coletaveis_Manager : MonoBehaviour
{
    [Header("STATUS")]
    public int valor;
    public Player_Move PM;

    [Header("VISUAL")]
    public GameObject visual;
    public SphereCollider colisor;

    [Header("AUDIO")]
    private SoundPool SP;

    void Start()
    {
        SP = FindObjectOfType<SoundPool>();
    }

    public void Coletado()
    {
        PM.RecebeCristais(valor);
        visual.SetActive(false);
        colisor.enabled = false;
        if(valor > 1)
        {
            SP.PlayAudio(SP.pega_cristal_azul);
        }
        else
        {
            SP.PlayAudio(SP.pega_cristal_verde);
        }
    }
    public void ResetGame()
    {
        visual.SetActive(true);
        colisor.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player_Move>())
        {
            Coletado();
        }
    }
}
