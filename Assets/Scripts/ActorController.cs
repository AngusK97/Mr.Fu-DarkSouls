using UnityEditorInternal;
using UnityEngine;

public class ActorController : MonoBehaviour
{
    public GameObject model;
    public PlayerInput pi;
    public float walkSpeed = 2.4f;
    public float runMultiplier = 2.0f;
    public float jumpVelocity = 7f;
    public float rollVelocity = 1f;

    [SerializeField] 
    private Animator anim;
    private Rigidbody rigid;
    private Vector3 planarVec;
    private Vector3 thrustVec;
    private bool lockPlanar;
    private bool canAttack;

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
        
        // 下落后翻滚
        if (rigid.velocity.magnitude > 0f)
            anim.SetTrigger("roll");

        // 跳跃
        if (pi.jump)
        {
            anim.SetTrigger("jump");
            canAttack = false;
        }

        // 跳跃
        if (pi.attack && CheckState("ground") && canAttack)
            anim.SetTrigger("attack");
        
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

    private bool CheckState(string stateName, string layerName = "Base Layer")
    {
        int layerIndex = anim.GetLayerIndex(layerName);
        bool result = anim.GetCurrentAnimatorStateInfo(layerIndex).IsName(stateName);
        return result;
    }
    

    ///
    /// Message processing block
    ///
    public void OnJumpEnter()
    {
        thrustVec = new Vector3(0f, jumpVelocity, 0f);
        pi.inputEnable = false;
        lockPlanar = true;
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
        canAttack = true;
    }

    public void OnFallEnter()
    {
        pi.inputEnable = false;
        lockPlanar = true;
    }

    public void OnRollEnter()
    {
        thrustVec = new Vector3(0f, rollVelocity, 0f);
        pi.inputEnable = false;
        lockPlanar = true;
    }

    public void OnJabEnter()
    {
        pi.inputEnable = false;
        lockPlanar = true;
    }

    public void OnJabUpdate()
    {
        thrustVec = model.transform.forward * anim.GetFloat("jabVelocity");
    }

    public void OnAttackIdleEnter()
    {
        pi.inputEnable = true;
        anim.SetLayerWeight(anim.GetLayerIndex("attack"), 0f);
    }

    public void OnAttack1hAEnter()
    {
        pi.inputEnable = false;
        anim.SetLayerWeight(anim.GetLayerIndex("attack"), 1f);
    }

    public void OnAttack1hAUpdate()
    {
        thrustVec = model.transform.forward * anim.GetFloat("attack1hAVelocity");
    }
}
