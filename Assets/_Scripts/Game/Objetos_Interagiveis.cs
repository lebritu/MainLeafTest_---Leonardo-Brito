using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objetos_Interagiveis : MonoBehaviour
{
    public float maxSpeed = 0.2f;//Replace with your max speed
    public Transform pai;
    public Rigidbody rb;
    public Transform[] targets;

    private Vector3 posicao_inicial;
    private Vector3 rotacao_inicial;

    [Header("Audio")]
    private SoundPool SP;

    private void Awake()
    {
        SP = FindObjectOfType<SoundPool>();
        posicao_inicial = transform.position;
        rotacao_inicial = transform.rotation.eulerAngles;
    }

    public void ResetGame()
    {
        transform.position = posicao_inicial;
        transform.rotation = Quaternion.Euler(rotacao_inicial);
    }

   
    void FixedUpdate()
    {
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Untagged"))
        {
            int ale = Random.Range(0, SP.hit_madeira.Length - 1); // gera um numero aleatorio para não repetir o mesmo audio
            SP.PlayAudio(SP.hit_madeira[ale]);
        }
    }
}
