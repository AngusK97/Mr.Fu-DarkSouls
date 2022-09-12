 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickInput : IUserInput
{
    [Header("===== Joystick Settings =====")]
    public string axisX = "axisX";
    public string axisY = "axisY";
    public string axisJright = "axis4";
    public string axisJup = "axis5";
    public string btnA = "btn0";
    public string btnB = "btn1";
    public string btnC = "btn2";
    public string btnD = "btn3";
    
    void Update()
    {
        Jup = Input.GetAxis(axisJup);
        Jright = Input.GetAxis(axisJright);
        
        targetDup = Input.GetAxis(axisY);
        targetDright = Input.GetAxis(axisX);

        if (inputEnable == false)
        {
            targetDup = 0f;
            targetDright = 0f;
        }

        Dup = Mathf.SmoothDamp(Dup, targetDup, ref velocityDup, 0.1f);
        Dright = Mathf.SmoothDamp(Dright, targetDright, ref velocityDright, 0.1f);
        
        Vector2 tempDAxis = SquareToCircle(new Vector2(Dright, Dup));
        float Dright2 = tempDAxis.x;
        float Dup2 = tempDAxis.y;
        
        Dmag = Mathf.Sqrt((Dup2 * Dup2) + (Dright2 * Dright2));
        Dvec = Dright2 * transform.right + Dup2 * transform.forward;
        
        // pressing signal
        run = Input.GetButton(btnA);

        // trigger once signal（等价于 Input.GetKeyDown()）
        // jump = Input.GetKeyDown(keyB);
        bool newJump = Input.GetButton(btnB);
        if (newJump != lastJump && newJump) jump = true;
        else jump = false;
        lastJump = newJump;
        
        bool newAttack = Input.GetButton(btnC);
        if (newAttack != lastAttack && newAttack) attack = true;
        else attack = false;
        lastAttack = newAttack;
    }
}
