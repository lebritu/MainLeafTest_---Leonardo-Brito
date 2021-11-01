using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player_Move : MonoBehaviour
{

    [Header("STATUS")]    
    public float padrão_velocidade;
    public float agachado_velocidade;
    public float padrão_rotacao_velocidade, agachado_rotacao_velocidade;
    public float pulo_forca = 1.0f;
    public float gravidade = -9.81f;
    private float movimento_velocidade, rotação_velocidade = 8;
    public int cristais_quantidade;


    [Header("CONDIÇÔES")]
    private bool agarrou;
    private bool escala_B, escala_cooldown, agachado, pode_levantar = true, aguardar_para_se_levantar, Pausado;
    public float pendurado_velocidade_para_subir; // serve para marcar a posição correta quando estiver pendurado
    public float tempo_para_subir_obstaculo = 0.5f; // serve para marcar a posição correta quando estiver pendurado

    public Game_Manager GM;
    public Input_Manager input_M;
    public Camera_Manager camera_M;
    public Animator anim, actions_anim;
    public TMP_Text actions_txt, cristatis_txt;
    public Objetos_Interagiveis obj_temp;

    [Header("CHARACTER CONTROLLER")]
    public bool onGround;
    private bool groundedPlayer;  
    public CharacterController controller;
    private Vector3 playerVelocity;   
    private Vector3 move_vector3;

    [Header("AUDIO")]
    private SoundPool SP;
    private AudioSource saida_de_som;

    void Start()
    {
        movimento_velocidade = padrão_velocidade;
        rotação_velocidade = padrão_rotacao_velocidade;
        SP = FindObjectOfType<SoundPool>();
        saida_de_som = GetComponent<AudioSource>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void Update()
    {
        Move();
        PuxaEmpurra();
        MenuControle();
    }

    private void LateUpdate()
    {
        PuxaObjeto();
    }

    void MenuControle()
    {
        if (Input.GetButtonDown(input_M.Pause) && !Pausado)
        {
            Pausado = true;
            GM.PauseGame();
        }
        else if (Input.GetButtonDown(input_M.Pause) && Pausado)
        {
            Pausado = false;
            GM.ResumeGame();
        }
    }

    void Move() // responsavel pelo movimento do personagem
    {
        if (escala_B)
        {
            Escalando();
        }
        else
        {
            AgachadoControle();

            groundedPlayer = controller.isGrounded;
            if (groundedPlayer && playerVelocity.y < 0)
            {
                playerVelocity.y = 0f;
            }

            // verifica se o personagem esta puxando ou empurrando um objeto
            if (agarrou)
            {
                move_vector3 = new Vector3(0, 0, Input.GetAxisRaw(input_M.Verti));
            }
            else
            {
                move_vector3 = new Vector3(Input.GetAxisRaw(input_M.Hori), 0, Input.GetAxisRaw(input_M.Verti));
            }
            move_vector3 = camera_M.transform.TransformDirection(move_vector3);
            move_vector3.y = 0.0f;
            move_vector3.Normalize();
            controller.Move(move_vector3 * Time.deltaTime * movimento_velocidade);

            // troca a posição do personagem no eixo y para simular um pulo
            if (Input.GetButtonDown(input_M.Jump) && onGround && !agarrou)
            {
                playerVelocity.y += Mathf.Sqrt(pulo_forca * -3.0f * gravidade);
                onGround = false;
                anim.SetTrigger("Jump");
            }
            playerVelocity.y += gravidade * Time.deltaTime;
            controller.Move(playerVelocity * Time.deltaTime);

            RotateTowardMovementVector(move_vector3); // rotaciona o personagem no seu proprio eixo para sempre olhar para frente

            // verifica se o personagem esta puxando ou empurrando um objeto
            if (agarrou)
            {
                anim.SetFloat("Z", Input.GetAxis(input_M.Verti), 0.1f, Time.deltaTime);
            }
            else
            {
                anim.SetFloat("Z", move_vector3.magnitude, 0.1f, Time.deltaTime);
            }

            VerifSolo();
        }

    }
    void RotateTowardMovementVector(Vector3 movementDirection)
    {
        if (movementDirection.magnitude == 0) { return; }
        var rotation = Quaternion.LookRotation(movementDirection);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, rotação_velocidade);
    }

    void VerifSolo() // responsavel por verificar se o personagem tocou o chão
    {
        if (controller.isGrounded && !onGround)
        {
            onGround = true;
            anim.SetTrigger("Land");
            Som("Land");
        }
    }
    void AgachadoControle() // controle da função agachar
    {
        if (Input.GetButton(input_M.Crouch) && onGround && !agarrou)
        {
            agachado = true;
            anim.SetBool("Agachado", true);
            movimento_velocidade = agachado_velocidade;
            rotação_velocidade = agachado_rotacao_velocidade;
            controller.center = new Vector3(0, 0.1f, 0);
            controller.height = 0.2f;
        }
        else if (Input.GetButtonUp(input_M.Crouch) && !agarrou && pode_levantar)
        {
            agachado = false;
            anim.SetBool("Agachado", false);
            movimento_velocidade = padrão_velocidade;
            rotação_velocidade = padrão_rotacao_velocidade;
            controller.center = new Vector3(0, 0.26f, 0);
            controller.height = 0.5f;
        }
        else if(agachado)
        {
            aguardar_para_se_levantar = true;
        }

        if(pode_levantar && aguardar_para_se_levantar && agachado)
        {
            agachado = false;
            aguardar_para_se_levantar = false;
            anim.SetBool("Agachado", false);
            movimento_velocidade = padrão_velocidade;
            rotação_velocidade = padrão_rotacao_velocidade;
            controller.center = new Vector3(0, 0.26f, 0);
            controller.height = 0.5f;
        }
    }

    void PuxaEmpurra() //controle da função puxar/empurrar 
    {
        if(obj_temp != null)
        {         
            if (Input.GetButtonDown(input_M.Mouse0) && onGround && !agarrou)
            {
                agarrou = true;
                anim.SetBool("PuxaEmpurra", true);
                movimento_velocidade = agachado_velocidade;
                rotação_velocidade = 0;              

                camera_M.CameraInterativa(obj_temp);
                transform.rotation = camera_M.posicao_camera_caixote.rotation;
            }
            else if (Input.GetButtonUp(input_M.Mouse0) && agarrou)
            {
                SoltaCaixote();

            }
        }
    }
    void SoltaCaixote() // responsavel por retornar o jogador a forma padrão
    {
        agarrou = false;
        anim.SetBool("PuxaEmpurra", false);
        movimento_velocidade = padrão_velocidade;
        rotação_velocidade = padrão_rotacao_velocidade;
        camera_M.Tipo = Camera_Manager.Select.TerceiraPessoa;
        obj_temp.maxSpeed = 200f;
        if (obj_temp != null)
        {
            obj_temp.transform.parent = obj_temp.pai;
        }
    } 
    void PuxaObjeto()
    {
        if (agarrou)
        {
            if (Input.GetAxisRaw(input_M.Verti) < 0)
            {
                float forca = 10;
                obj_temp.maxSpeed = 2f;
                obj_temp.rb.velocity += -camera_M.posicao_camera_caixote.forward * (forca * Time.deltaTime);
            }
            if (Input.GetAxisRaw(input_M.Verti) > 0)
            {
                float forca = 6f;
                obj_temp.maxSpeed = 0.25f;
                obj_temp.rb.velocity += camera_M.posicao_camera_caixote.forward * (forca * Time.deltaTime);
            }
        }
    }

    void ComeçaAEscalar()
    {
        if (!escala_B && !escala_cooldown)
        {
            anim.SetTrigger("Pendurado");
            escala_B = true;
            escala_cooldown = true;
            StartCoroutine("termina_de_subir");
            controller.enabled = false;
        }
    }
    void Escalando() // responsavel por fazer o personagem subir objetos
    {
        
        transform.position = new Vector3(transform.position.x, transform.position.y + pendurado_velocidade_para_subir * Time.deltaTime, transform.position.z);
        
    }
    IEnumerator termina_de_subir()
    {
        yield return new WaitForSeconds(tempo_para_subir_obstaculo);
        escala_B = false;
        controller.enabled = true;
        yield return new WaitForSeconds(1);
        escala_cooldown = false; // cooldown para poder subir denovo
    }

    void DisplayActions(string s)
    {
        actions_txt.text = s;
        actions_anim.SetTrigger("Play");
    }
    public void RecebeCristais(int i)
    {
        cristais_quantidade += i;
        cristatis_txt.text = cristais_quantidade.ToString();
    }

    public void ResetGame()
    {
        cristais_quantidade = 0;
        cristatis_txt.text = "0";
        DisplayActions("");
        aguardar_para_se_levantar = false;
        pode_levantar = true;
        anim.SetTrigger("Idle");

    }

    public void Som(string s) // esta função é um atalho para poder usar sons de uma maneira mais rapida
    {
        switch (s)
        {
            case "Land":
                saida_de_som.PlayOneShot(SP.land, SP.volume);
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Objetos_Interagiveis>())
        {
            obj_temp = other.GetComponent<Objetos_Interagiveis>();
            DisplayActions("Agarrar");
        }
        if (other.GetComponent<Trigger_Interativo>())
        {
            Trigger_Interativo ti = other.GetComponent<Trigger_Interativo>();
            switch (ti.Tipo)
            {
                case Trigger_Interativo.Select.BlockAgachar:
                    pode_levantar = false;
                    break;
                case Trigger_Interativo.Select.PassaNivel:
                    GM.TransiçãoDeNivel(ti.NextNivel);
                    break;
                case Trigger_Interativo.Select.Final:
                    GM.Final();
                    break;
            }
            if (!onGround)
            {
                ComeçaAEscalar();
            }
        }
        if (other.GetComponent<CameraPoint>())
        {
            camera_M.TargetOffSet = other.GetComponent<CameraPoint>().Point;
        }
    }
 
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Objetos_Interagiveis>())
        {
            if (agarrou)
            {
                SoltaCaixote();               
            }
            if (obj_temp != null)
            {
                obj_temp = null;
            }
            DisplayActions("");
        }
        if (other.GetComponent<Trigger_Interativo>())
        {
            if (other.GetComponent<Trigger_Interativo>().Tipo == Trigger_Interativo.Select.BlockAgachar)
            {
                pode_levantar = true;
            }
        }
    }
}
