using UnityEngine;

public class MyTimer
{
    public enum STATE
    {
        IDLE,
        RUN,
        FINISHED
    }

    public STATE state;
    public float duration;
    
    private float elapsedTime;

    public void Tick()
    {
        if (state == STATE.IDLE)
        {
            
        }
        else if (state == STATE.RUN)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= duration)
            {
                state = STATE.FINISHED;
            }
        }
        else if (state == STATE.FINISHED)
        {
            
        }
        else
        {
            Debug.LogError("MyTimer Error");
        }
    }

    public void Go()
    {
        elapsedTime = 0f;
        state = STATE.RUN;
    }
}
