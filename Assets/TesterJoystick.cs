using System;
using UnityEngine;

public class TesterJoystick : MonoBehaviour
{
    private string currentButton;
    void Update()
    {
        // print(Input.GetAxis("Dright"));
        // print(Input.GetAxis("Dup"));
        
        // print(Input.GetAxis("Jright"));
        // print(Input.GetAxis("Jup"));
        
        // print(Input.GetAxis("padH"));
        // print(Input.GetAxis("padV"));
        
        // print(Input.GetButtonDown("btn0"));
        // print(Input.GetButtonDown("btn1"));
        // print(Input.GetButtonDown("btn2"));
        // print(Input.GetButtonDown("btn3"));
        
        // print(Input.GetButtonDown("LB"));
        // print(Input.GetButtonDown("LT"));
        // print(Input.GetButtonDown("RB"));
        // print(Input.GetButtonDown("RT"));
        
        var values = Enum.GetValues(typeof(KeyCode));  // 存储所有的按键
        for (int x = 0; x < values.Length; x++)
        {
            if (Input.GetKeyDown((KeyCode)values.GetValue(x)))
            {
                currentButton = values.GetValue(x).ToString();  // 遍历并获取当前按下的按键
                print(currentButton);
            }
        }
    }
}
