using UnityEditorInternal;
using UnityEngine;

public class ActorController : MonoBehaviour
{
    public GameObject model;
    public PlayerInput pi;
    public float walkSpeed = 2.0f;
    public float runMultiplier = 2.0f;
    public float jumpVelocity = 7f;

    [SerializeField] 
    private Animator anim;
    private Rigidbody rigid;
    private Vector3 planarVec;
    private Vector3 thrustVec;
    private bool lockPlanar;

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

        // 跳跃
        if (pi.jump) anim.SetTrigger("jump");
        
        // 角色旋转
        if (pi.Dmag > 0.1f)
            model.transform.forward = Vector3.Slerp(model.transform.forward, pi.Dvec, 0.3f);
        
        // 角色移动
        if (!lockPlanar)
            planarVec = model.transform.forward * (pi.Dmag * walkSpeed * (pi.run ? runMultiplier : 1.0f)); 
    }

    private void FixedUpdate()
    {
        // rigid.position += movingVec * Time.fixedDeltaTime;  // 修改 rigid.position 移动角色
        rigid.velocity = new Vector3(planarVec.x, rigid.velocity.y, planarVec.z) + thrustVec; // 修改 rigid.velocity 移动角色
        thrustVec = Vector3.zero;
    }

    ///
    /// Message processing block
    ///
    public void OnJumpEnter()
    {
        pi.inputEnable = false;
        lockPlanar = true;
        thrustVec = new Vector3(0f, jumpVelocity, 0f);
    }

    public void IsGround()
    {
        anim.SetBool("isGround", true);
    }
    
    public void IsNotGround()
    {
        anim.SetBool("isGround", false);
    }

    public void OnGroundEnter()
    {
        pi.inputEnable = true;
        lockPlanar = false;
    }

    public void OnFallEnter()
    {
        pi.inputEnable = false;
        lockPlanar = true;
    }
}
