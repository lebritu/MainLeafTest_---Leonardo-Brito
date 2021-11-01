using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NivelManager : MonoBehaviour
{
    [Header("STATUS")]
    public int nivel_ID;
    public string nome_do_nivel;
    public int checkpoint_ID;
    private bool ativo;
    public Transform[] checkpoint_posicao;
    public Player_Move PM;

    [Header("VISUAL")]
    public Animator nome_display_anim;
    public TMP_Text nome_display_txt;   

    public Objetos_Interagiveis[] objetos;
    public IA_Manager[] guardas;
    public Coletaveis_Manager[] coletaveis;  

    private void OnDisable()
    {
        ativo = false;
    }

    void Awake()
    {
        objetos = GetComponentsInChildren<Objetos_Interagiveis>();
        guardas = GetComponentsInChildren<IA_Manager>();
        coletaveis = GetComponentsInChildren<Coletaveis_Manager>();
    }

    void Update()
    {
        if (!ativo)
        {
            ativo = true;
            AtivaNivel();
        }
    }

    public void AtivaNivel()
    {
        PM.transform.position = checkpoint_posicao[0].position;
        PM.transform.rotation = checkpoint_posicao[0].rotation;
        CondiçõesUnicasDeNivel();
        DisplayName();
    }
    public void ResetaNivel()
    {
        if (objetos.Length > 0)
        {
            for (int i = 0; i < objetos.Length; i++)
            {
                objetos[i].ResetGame();
            }
        }
        if (guardas.Length > 0)
        {
            for (int i = 0; i < guardas.Length; i++)
            {
                guardas[i].ResetAgent();
            }
        }
        if(coletaveis.Length > 0)
        {
            for (int i = 0; i < coletaveis.Length; i++)
            {
                coletaveis[i].ResetGame();
            }
        }
        PM.transform.position = checkpoint_posicao[0].position;
        PM.transform.rotation = checkpoint_posicao[0].rotation;
    }
    public void CheckPoint()
    {
        PM.transform.position = checkpoint_posicao[checkpoint_ID].position;
        PM.transform.rotation = checkpoint_posicao[checkpoint_ID].rotation;
    }

    void DisplayName()
    {
        StartCoroutine("displayName_temp");
    }
    IEnumerator displayName_temp()
    {
        yield return new WaitForSeconds(2f);
        nome_display_txt.text = nome_do_nivel;
        nome_display_anim.SetTrigger("Play");
    }
    void CondiçõesUnicasDeNivel()
    {
        switch (nivel_ID)
        {
            case 1:
                PM.camera_M.AtivaTerceiraPessoa();
                PM.enabled = true;
                break;
            case 2:
                PM.camera_M.DesativarTerceiraPessoa();
                PM.enabled = true;
                break;
        }
    }

    public void AtualizaCheckpoint()
    {
        checkpoint_ID = check_int();
    }
    int check_int()
    {
        checkpoint_ID++;
        if(checkpoint_ID > checkpoint_posicao.Length-1)
        {
            checkpoint_ID -= 1;
        }
        return checkpoint_ID;
    }
}
