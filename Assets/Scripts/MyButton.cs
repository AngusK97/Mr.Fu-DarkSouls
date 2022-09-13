public class MyButton
{
    public bool IsPressing = false;
    public bool OnPressed = false;
    public bool OnReleased = false;

    private bool curState = false;
    private bool lastState = false;

    public void Tick(bool input)
    {
        curState = input;
        IsPressing = curState;
        OnPressed = !lastState && curState;
        OnReleased = lastState && !curState;
        lastState = curState;
    }
}
