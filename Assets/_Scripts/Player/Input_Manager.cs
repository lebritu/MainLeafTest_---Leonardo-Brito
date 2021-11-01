using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Input_Manager : MonoBehaviour
{
    public string Hori;
    public string Verti;
    public string MouseX;
    public string MouseY;
    public string Mouse0;
    public string Pause;
    public string Run;
    public string Jump;
    public string Crouch;
    public string MouseScroll;

    private void Start()
    {
        AtualizaInput();
    }
    public void AtualizaInput()
    {
        Hori = "Horizontal";
        Verti = "Vertical";
        MouseX = "Mouse X";
        MouseY = "Mouse Y";
        Run = "Run";
        Pause = "Pause";
        MouseScroll = "Mouse_S";
        Mouse0 = "Mouse 0";
        Jump = "Jump";
        Crouch = "Crouch";
    }
}
