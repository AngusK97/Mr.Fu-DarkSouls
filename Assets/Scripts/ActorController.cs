using UnityEngine;

public class ActorController : MonoBehaviour
{
    public GameObject model;
    public PlayerInput pi;
    public float walkSpeed = 2.0f;
    public float runMultiplier = 2.0f;

    [SerializeField] 
    private Animator anim;
    private Rigidbody rigid;
    private Vector3 movingVec;

    private void Awake()
    {
        pi = GetComponent<PlayerInput>();
        anim = model.GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // 动画
        float targetRunMulti = Mathf.Lerp(anim.GetFloat("forward"), pi.Dmag * (pi.run ? 2.0f : 1.0f), 0.05f);
        anim.SetFloat("forward", targetRunMulti);

        // 角色旋转
        if (pi.Dmag > 0.1f)
            model.transform.forward = Vector3.Slerp(model.transform.forward, pi.Dvec, 0.3f);
        
        // 角色移动
        movingVec = model.transform.forward * (pi.Dmag * walkSpeed * (pi.run ? runMultiplier : 1.0f)); 
    }

    private void FixedUpdate()
    {
        // rigid.position += movingVec * Time.fixedDeltaTime;  // 修改 rigid.position 移动角色
        rigid.velocity = new Vector3(movingVec.x, rigid.velocity.y, movingVec.z);   // 修改 rigid.velocity 移动角色
    }
}
