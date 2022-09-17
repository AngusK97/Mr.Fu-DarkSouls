using UnityEngine;

/// <summary>
/// Input -> Signal
/// </summary>
public class KeyboardInput : IUserInput
{
    [Header("===== Keys settings =====")]
    public string keyUp;
    public string keyDown;
    public string keyLeft;
    public string keyRight;
    
    public string keyJUp;
    public string keyJDown;
    public string keyJRight;
    public string keyJLeft;
    
    public string keyA;  // run, jump, jab
    public string keyB;  // defense
    public string keyC;  // left mouse btn
    public string keyD;  // right mouse btn
    public string keyLock;  // lock on

    [Header("===== Mouse settings =====")]
    public bool mouseEnable;
    public float mouseSensitivityX = 1.0f;
    public float mouseSensitivityY = 1.0f;

    private MyButton buttonA = new MyButton();
    private MyButton buttonB = new MyButton();
    private MyButton buttonC = new MyButton();
    private MyButton buttonD = new MyButton();
    private MyButton buttonKLock = new MyButton();
    
    private void Update()
    {
        buttonA.Tick(Input.GetKey(keyA));
        buttonB.Tick(Input.GetKey(keyB));
        buttonC.Tick(Input.GetKey(keyC));
        buttonD.Tick(Input.GetKey(keyD));
        buttonKLock.Tick(Input.GetKey(keyLock));
        
        // camera move amount
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
        
        // smoothed player move amount
        targetDup = (Input.GetKey(keyUp) ? 1.0f : 0f) - (Input.GetKey(keyDown) ? 1.0f : 0f);
        targetDright = (Input.GetKey(keyRight) ?  1.0f : 0f) - (Input.GetKey(keyLeft) ? 1.0f : 0f);
        if (inputEnable == false)
        {
            targetDup = 0f;
            targetDright = 0f;
        }
        Dup = Mathf.SmoothDamp(Dup, targetDup, ref velocityDup, 0.1f);
        Dright = Mathf.SmoothDamp(Dright, targetDright, ref velocityDright, 0.1f);

        // normalize player move amount
        Vector2 tempDAxis = SquareToCircle(new Vector2(Dright, Dup));
        float Dright2 = tempDAxis.x;
        float Dup2 = tempDAxis.y;
        Dmag = Mathf.Sqrt((Dup2 * Dup2) + (Dright2 * Dright2));
        Dvec = Dright2 * transform.right + Dup2 * transform.forward;

        // player behaviors
        run = (buttonA.IsPressing && !buttonA.IsDelaying) || buttonA.IsExtending;
        jump = buttonA.OnPressed && buttonA.IsExtending;
        roll = buttonA.OnReleased && buttonA.IsDelaying;
        attack = buttonC.OnPressed;
        lockOn = buttonKLock.OnPressed;
        defense = buttonB.IsPressing;
    }
}
