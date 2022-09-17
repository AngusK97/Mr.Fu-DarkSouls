using UnityEngine;

public class MyTimer
{
    public enum State
    {
        IDLE,
        RUN,
        FINISHED
    }

    public State state;
    public float duration;
    
    private float elapsedTime;

    public void Tick()
    {
        if (state == State.IDLE)
        {
            
        }
        else if (state == State.RUN)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= duration)
            {
                state = State.FINISHED;
            }
        }
        else if (state == State.FINISHED)
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
        state = State.RUN;
    }
}
