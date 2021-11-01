using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IA_Manager : MonoBehaviour
{
    public enum Select { Patrulheiro, Aleatorio}
    public Select Tipo;

    [Header("STATUS")]
    public float speed;
    public int point_ID;

    private bool check, ativo;
    private float time_proximo_comando;
    public Game_Manager GM;
    public Player_Move PM;
    public Animator anim;
    public Transform[] points;
    private Transform next_point;
    private Vector3 posicao_inicial;
    public NavMeshAgent agent;

    [Header("CAMPO DE VISÃO")]
    public float angulo_maximo;
    public float distancia_maxima;
    private bool avistou_B;
    public Transform raycast_saida;
    public LayerMask Layer;

    private void OnDisable()
    {
        ativo = false;
        avistou_B = false;
    }
    private void Awake()
    {
        posicao_inicial = transform.position;
    }
    private void Update()
    {
        if (!ativo)
        {
            ativo = true;
            ResetAgent();
        }
        AnimUpdate();
        verificadorDeChegada();
        DetectorDeVisao();
    }
    void AnimUpdate()
    {
        anim.SetFloat("Z", agent.velocity.magnitude, 0.1f, Time.deltaTime);
    }

    public void ResetAgent()
    {
        agent.speed = speed;
        point_ID = 0;
        next_point = points[0];
        check = false;
        transform.position = posicao_inicial;
        SetPoint(points[point_ID]);
    }

    public void SetPoint(Transform t)
    {
        agent.SetDestination(t.position);
        next_point = t;
        check = false;
    }
    void verificadorDeChegada()
    {
        if (Vector3.Distance(transform.position, next_point.position) <= 0.1f && !check)
        {
            check = true;
            float t = Random.Range(2, 6);
            time_proximo_comando = t;
            StartCoroutine("proximo_comando");
        }
    }
    IEnumerator proximo_comando()
    {
        yield return new WaitForSeconds(time_proximo_comando);

        switch (Tipo)
        {
            case Select.Patrulheiro:
                SetPoint(points[next()]);
                break;
            case Select.Aleatorio:
                SetPoint(points[random()]);
                break;
        }
    }

    void DetectorDeVisao()
    {
        if (!avistou_B)
        {
            Vector3 targetDir = PM.transform.position - transform.position;
            float angle = Vector3.Angle(targetDir, transform.forward);
            raycast_saida.LookAt(PM.camera_M.Target);

            if (Vector3.Distance(transform.position, PM.transform.position) <= distancia_maxima)
            {
                if (angle < angulo_maximo)
                {
                    // verifica se tem alguma parede na frente
                    RaycastHit hit;
                    if (!Physics.Linecast(raycast_saida.transform.position, PM.camera_M.Target.transform.position, out hit, Layer))
                    {
                        print("Avistei");
                        Debug.DrawLine(raycast_saida.position, hit.point);
                        Avistou();
                    }
                    else if (Physics.Linecast(raycast_saida.transform.position, PM.camera_M.Target.transform.position, out hit, Layer))
                    {
                        print("não estou vendo");
                        Debug.DrawLine(raycast_saida.position, hit.point);
                    }
                }

            }
        }

    }
    void Avistou()
    {
        avistou_B = true;
        anim.SetTrigger("Avistou");
        agent.speed = 0;
        GM.Avistado();
    }

    int next()
    {
        point_ID++;
        if (point_ID > points.Length - 1)
        {
            point_ID = 0;
        }
        return point_ID;
    }
    int random()
    {
        int i = Random.Range(0, points.Length - 1);
        return i;
    }
}
