using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game_Manager : MonoBehaviour
{
    private int proximo_nivel_ID;
    public bool avistado;

    public GameObject menu_in_game;
    public NivelManager[] Niveis;
    public GameObject Nivel1, Nivel2;
    public Animator fade_anim;
    public Player_Move PM;

    public GameObject aviso_parado, fim_de_jogo;

    public void Limbo()
    {
        if (!avistado)
        {
            avistado = true;
            PM.enabled = false;
            StartCoroutine("retorna_limbo");
        }
    }
    IEnumerator retorna_limbo()
    {
        yield return new WaitForSeconds(2f);
        fade_anim.SetTrigger("FadeIn");
        aviso_parado.SetActive(false);
        yield return new WaitForSeconds(2f);
        for (int i = 0; i < Niveis.Length; i++)
        {
            Niveis[i].gameObject.SetActive(false);
        }
        yield return new WaitForSeconds(0.2f);
        proximo_nivel_ID = 0;
        PM.ResetGame();
        Niveis[0].gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        fade_anim.SetTrigger("FadeOut");
        avistado = false;
    }
    public void Avistado()
    {
        if (!avistado)
        {
            avistado = true;
            PM.enabled = false;
            PM.anim.SetTrigger("Idle");
            aviso_parado.SetActive(true);
            StartCoroutine("retorna_para_o_comeco");
        }
    }
    IEnumerator retorna_para_o_comeco()
    {
        yield return new WaitForSeconds(6f);
        fade_anim.SetTrigger("FadeIn");
        aviso_parado.SetActive(false);
        yield return new WaitForSeconds(2f);
        for (int i = 0; i < Niveis.Length; i++)
        {
            Niveis[i].gameObject.SetActive(false);
        }
        yield return new WaitForSeconds(0.2f);
        proximo_nivel_ID = 0;       
        Niveis[0].gameObject.SetActive(true);
        PM.ResetGame();
        yield return new WaitForSeconds(0.5f);
        fade_anim.SetTrigger("FadeOut");
        avistado = false;
    }

    public void Final()
    {
        PM.enabled = false;
        fade_anim.SetTrigger("FadeIn");
        StartCoroutine("final_temp");
    }
    IEnumerator final_temp()
    {
        yield return new WaitForSeconds(2f);
        fim_de_jogo.SetActive(true);
        yield return new WaitForSeconds(5f);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        fim_de_jogo.SetActive(false);
        menu_in_game.SetActive(true);
    }

    public void TransiçãoDeNivel(int id)
    {
        PM.enabled = false;
        proximo_nivel_ID = id;
        fade_anim.SetTrigger("FadeIn");      
        StartCoroutine("proximo_nivel_temp");     
    }
    IEnumerator proximo_nivel_temp()
    {
        yield return new WaitForSeconds(2f);
        for (int i = 0; i < Niveis.Length; i++)
        {
            Niveis[i].gameObject.SetActive(false);
        }
        yield return new WaitForSeconds(0.2f);
        Niveis[proximo_nivel_ID-1].gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        fade_anim.SetTrigger("FadeOut");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void ResumeGame()
    {
        Time.timeScale = 1;
        menu_in_game.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public void PauseGame()
    {
        Time.timeScale = 0;
        menu_in_game.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void RestartGame()
    {
        Time.timeScale = 1;
        menu_in_game.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        PM.enabled = false;       
        fade_anim.SetTrigger("FadeIn");
        StartCoroutine("restart_game_temp");
    }
    IEnumerator restart_game_temp()
    {
        yield return new WaitForSeconds(2f);
        for (int i = 0; i < Niveis.Length; i++)
        {
            Niveis[i].ResetaNivel();
            Niveis[i].gameObject.SetActive(false);         
        }
        yield return new WaitForSeconds(0.2f);
        proximo_nivel_ID = 0;
        PM.ResetGame();
        Niveis[0].gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        fade_anim.SetTrigger("FadeOut");
        avistado = false;
    }
}
