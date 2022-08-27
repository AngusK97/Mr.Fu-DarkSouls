using UnityEngine;

public class ActorController : MonoBehaviour
{
    public GameObject model;
    public PlayerInput pi;

    [SerializeField] 
    private Animator anim;

    private void Awake()
    {
        pi = GetComponent<PlayerInput>();
        anim = model.GetComponent<Animator>();
    }

    private void Update()
    {
        anim.SetFloat("forward", pi.Dmag);
        if (pi.Dmag > 0.1f)
        {
            model.transform.forward = pi.Dvec;
        }
    }
}
