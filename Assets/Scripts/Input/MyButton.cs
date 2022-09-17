public class MyButton
{
    private readonly float extendingDuration = 0.15f;
    private readonly float delayingDuration = 0.2f;
    
    public bool IsPressing;
    public bool OnPressed;
    public bool OnReleased;
    public bool IsExtending;
    public bool IsDelaying;

    private bool curState;
    private bool lastState;
    
    private MyTimer extTimer = new MyTimer();
    private MyTimer delayTimer = new MyTimer();

    public void Tick(bool input)
    {
        extTimer.Tick();
        delayTimer.Tick();
        
        curState = input;
        
        IsPressing = curState;
        OnPressed = false;
        OnReleased = false;
        IsExtending = false;
        IsDelaying = false;

        if (curState != lastState)
        {
            if (curState)
            {
                OnPressed = true;
                StartTimer(delayTimer, delayingDuration);
            }
            else
            {
                OnReleased = true;
                StartTimer(extTimer, extendingDuration);
            }
        }
        
        lastState = curState;
        
        if (extTimer.state == MyTimer.State.RUN)
            IsExtending = true;

        if (delayTimer.state == MyTimer.State.RUN)
            IsDelaying = true;
    }

    private void StartTimer(MyTimer timer, float duration)
    {
        timer.duration = duration;
        timer.Go();
    }
}
