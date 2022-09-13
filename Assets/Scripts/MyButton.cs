using UnityEngine;

public class MyButton
{
    public bool IsPressing = false;
    public bool OnPressed = false;
    public bool OnReleased = false;
    public bool IsExtending = false;

    public float extendingDuration = 0.15f;

    private bool curState = false;
    private bool lastState = false;
    
    private MyTimer extTimer = new MyTimer();

    public void Tick(bool input)
    {
        extTimer.Tick();    
        
        curState = input;
        IsPressing = curState;
        OnPressed = !lastState && curState;
        OnReleased = lastState && !curState;
        lastState = curState;

        if (OnReleased)
        {
            StartTimer(extTimer, extendingDuration);
        }

        IsExtending = extTimer.state == MyTimer.STATE.RUN;
    }

    private void StartTimer(MyTimer timer, float duration)
    {
        timer.duration = duration;
        timer.Go();
    }
}
