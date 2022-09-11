using UnityEngine;

public class TriggerControl : MonoBehaviour
{
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void ResetTrigger(string triggerName)
    {
        print("reset!!!");
        anim.ResetTrigger(triggerName);
    }
}
