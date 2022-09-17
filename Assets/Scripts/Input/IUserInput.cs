using UnityEngine;

public abstract class IUserInput : MonoBehaviour
{
    [Header("===== Output signals =====")]
    // player ground move
    public float Dup;  // [-1, 1] forward backward
    public float Dright;  // [-1, 1] leftward rightward
    public float Dmag;  // normalized move amount
    public Vector3 Dvec;  // normalized move vector
    
    // camera move
    public float Jup;
    public float Jright;

    // player behaviors
    public bool run;
    public bool jump;
    public bool roll;
    public bool attack;
    public bool lockOn;
    public bool defense;

    [Header("===== Others =====")]
    public bool inputEnable = true;
    
    // for smooth
    protected float targetDup;
    protected float targetDright;
    protected float velocityDup;
    protected float velocityDright;
    
    protected Vector2 SquareToCircle(Vector2 input)
    {
        Vector2 output = Vector2.zero;
        output.x = input.x * Mathf.Sqrt(1 - (input.y * input.y) / 2.0f);
        output.y = input.y * Mathf.Sqrt(1 - (input.x * input.x) / 2.0f);
        return output;
    }
}
