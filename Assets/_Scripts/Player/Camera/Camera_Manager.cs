using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Manager : MonoBehaviour
{
    public enum Select { TerceiraPessoa,Fixa,Caixote}
    public Select Tipo;

   [Header("STATUS")]
    public float velocidade_camera;
    public float sensibilidade_camera;
    public float limiteVertical_camera;
    private float rotX, rotY;
    public Player_Move PM;
    public LayerMask Layer;

    [Header("POSIÇÔES")]
    public Transform Target;
    public Transform posicao_camera_terceira_pessoa, posicao_camera_caixote;
    private Transform posicao_look_interagivel;
    public Vector3 TargetOffSet;   
    private Quaternion default_rotation;   

    void Start()
    {
        default_rotation = transform.rotation;
    }

    void Update()
    {
        switch (Tipo)
        {
            case Select.TerceiraPessoa:
                ModoTerceiraPessoa();
                break;
            case Select.Fixa:
                ModoCameraFixa();
                break;
            case Select.Caixote:
                ModoCameraCaixote();
                break;
        }
    }
   
    private void LateUpdate()
    {
        switch (Tipo)
        {
            case Select.TerceiraPessoa:
                transform.LookAt(Target);
                break;
            case Select.Fixa:
                Look();
                break;
            case Select.Caixote:
                LookIntergivel();
                break;
        }
    }

    public void AtivaTerceiraPessoa()
    {
        Tipo = Select.TerceiraPessoa;
    }
    public void DesativarTerceiraPessoa()
    {
        Tipo = Select.Fixa;
        ResetCam();
    }
    void TerceiraPessoaCollisionCheck() // checa a colisão entre o personagem e a parede
    {
        
        RaycastHit hit;
        if (!Physics.Linecast(Target.transform.position, posicao_camera_terceira_pessoa.transform.position,Layer))
        {
            transform.position = Vector3.Lerp(transform.position, posicao_camera_terceira_pessoa.transform.position, velocidade_camera * Time.deltaTime);
        }
        else if (Physics.Linecast(Target.transform.position, posicao_camera_terceira_pessoa.transform.position, out hit, Layer))
        {
            transform.position = Vector3.Lerp(transform.position, hit.point, velocidade_camera * Time.deltaTime);
        }
    }

    void ModoTerceiraPessoa() // responsavel pelo movimento da camera
    {
        float mouseX = Input.GetAxis(PM.input_M.MouseX);
        float mouseY = Input.GetAxis(PM.input_M.MouseY);

        rotY += mouseX * sensibilidade_camera * Time.deltaTime;
        rotX += -mouseY * sensibilidade_camera * Time.deltaTime;

        rotX = Mathf.Clamp(rotX, -limiteVertical_camera, limiteVertical_camera);

        Quaternion localRot = Quaternion.Euler(rotX, rotY, 0);
        Target.rotation = localRot;

        TerceiraPessoaCollisionCheck();

    }
    void ModoCameraFixa() // Essa função é responsavel pela camera fixa
    {
        transform.position = Vector3.Lerp(transform.position, Target.position + TargetOffSet, velocidade_camera * Time.deltaTime);
    }

    public void CameraInterativa(Objetos_Interagiveis obj)
    {
        foreach (Transform t in obj.targets)
        {
            if(Vector3.Distance(PM.transform.position, t.position) <= 1.7f)
            {
                posicao_camera_caixote = t;
                posicao_look_interagivel = obj.transform;
                Tipo = Camera_Manager.Select.Caixote;
            }
        }
    }
    void ModoCameraCaixote() //responsavel pela camera do caixote
    {
        transform.position = Vector3.Lerp(transform.position, posicao_camera_caixote.position, velocidade_camera * Time.deltaTime);
    }
    void Look() // função responsavel por olhar um alvo
    {
        var targetRotation = Quaternion.LookRotation(Target.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, velocidade_camera * Time.deltaTime);
    }
    void LookIntergivel() // função responsavel por olhar um alvo interagivel
    {
        var targetRotation = Quaternion.LookRotation(posicao_look_interagivel.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, velocidade_camera * Time.deltaTime);
    }

    public void ResetCam() // reseta a rotação da camera para o padrão
    {
        transform.rotation = default_rotation;
    }
}
