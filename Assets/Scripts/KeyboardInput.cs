using System;
using UnityEngine;

/// <summary>
/// Input -> Signal
/// </summary>
public class KeyboardInput : IUserInput
{
    [Header("===== Keys settings =====")]
    public string keyUp = "w";
    public string keyDown = "s";
    public string keyLeft = "a";
    public string keyRight = "d";

    public string keyA;
    public string keyB;
    public string keyC;
    public string keyD;
    
    public string keyJUp;
    public string keyJDown;
    public string keyJRight;
    public string keyJLeft;

    [Header("===== Mouse settings =====")]
    public bool mouseEnable;
    public float mouseSensitivityX = 1.0f;
    public float mouseSensitivityY = 1.0f;

    private void Update()
    {
        if (mouseEnable)
        {
            Jup = Input.GetAxis("Mouse Y") * 2.5f * mouseSensitivityY;
            Jright = Input.GetAxis("Mouse X") * 3f * mouseSensitivityX;
        }
        else
        {
            Jup = (Input.GetKey(keyJUp) ? 1.0f : 0f) - (Input.GetKey(keyJDown) ? 1.0f : 0f);
            Jright = (Input.GetKey(keyJRight) ? 1.0f : 0f) - (Input.GetKey(keyJLeft) ? 1.0f : 0f);       
        }
        
        targetDup = (Input.GetKey(keyUp) ? 1.0f : 0f) - (Input.GetKey(keyDown) ? 1.0f : 0f);
        targetDright = (Input.GetKey(keyRight) ?  1.0f : 0f) - (Input.GetKey(keyLeft) ? 1.0f : 0f);

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
        run = Input.GetKey(keyA);
        defense = Input.GetKey(keyD);

        // trigger once signal（等价于 Input.GetKeyDown()）
        // jump = Input.GetKeyDown(keyB);
        bool newJump = Input.GetKey(keyB);
        if (newJump != lastJump && newJump) jump = true;
        else jump = false;
        lastJump = newJump;
        
        bool newAttack = Input.GetKey(keyC);
        if (newAttack != lastAttack && newAttack) attack = true;
        else attack = false;
        lastAttack = newAttack;

        lockOn = Input.GetKeyDown(KeyCode.Q);
    }
}
