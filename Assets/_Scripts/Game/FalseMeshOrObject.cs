using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FalseMeshOrObject : MonoBehaviour
{
    public enum select { Mesh, Object}
    public select Tipo;

    void Start()
    {
        switch (Tipo)
        {
            case select.Mesh:
                GetComponent<MeshRenderer>().enabled = false;
                break;
            case select.Object:
                gameObject.SetActive(false);
                break;
        }
    }
}
