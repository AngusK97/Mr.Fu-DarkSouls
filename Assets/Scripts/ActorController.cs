using UnityEngine;

public class ActorController : MonoBehaviour
{
    public IUserInput pi;
    public GameObject model;
    public CameraController camCon;
    public float walkSpeed = 2.4f;
    public float runMultiplier = 2.0f;
    public float jumpVelocityY = 7f;
    public float rollVelocity = 1f;

    [Space(10)]
    [Header("===== Friction Settings =====")]
    public PhysicMaterial frictionOne;
    public PhysicMaterial frictionZero;

    private Animator anim;
    private Rigidbody rigid;
    private CapsuleCollider col;
    
    private Vector3 planarVelocity;  // 角色基础移动速度
    private Vector3 thrustVelocity;  // 附加速度
    
    private bool canAttack;  // 是否允许攻击
    private bool lockPlanar;  // 是否锁定地面移动量 planarVelocity
    private bool trackDirection;  // 记录是否在翻滚或跳跃
    
    private float layerLerpTarget;  // 动画层权重的目标值 
    private Vector3 deltaPos;  // Root Motion 的位置偏移量

    private void Awake()
    {
        IUserInput[] inputs = GetComponents<IUserInput>();
        foreach (var input in inputs)
        {
            if (input.enabled)
            {
                pi = input;
                break;
            }
        }
        col = GetComponent<CapsuleCollider>();
        anim = model.GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // 锁定敌人
        if (pi.lockOn)
            camCon.LockUnlock();

        if (!camCon.lockState)
        {
            // 动画
            float targetRunMulti = Mathf.Lerp(anim.GetFloat("forward"), pi.Dmag * (pi.run ? 2.0f : 1.0f), 0.05f);
            anim.SetFloat("forward", targetRunMulti);
            anim.SetFloat("right", 0);
        }
        else
        {
            var localDVec = transform.InverseTransformVector(pi.Dvec);
            anim.SetFloat("forward", localDVec.z * (pi.run ? 2.0f : 1.0f));
            anim.SetFloat("right", localDVec.x * (pi.run ? 2.0f : 1.0f));
        }
        
        // 举盾
        anim.SetBool("defense", pi.defense);
        
        // 下落后翻滚
        if (pi.roll || rigid.velocity.magnitude > 7f)
        {
            anim.SetTrigger("roll");
            canAttack = false;
        }

        // 跳跃
        if (pi.jump)
        {
            anim.SetTrigger("jump");
            canAttack = false;
        }

        // 攻击
        if (pi.attack && CheckState("ground") && canAttack)
            anim.SetTrigger("attack");

        // 锁定模式下角色的朝向与基础速度
        if (!camCon.lockState)
        {
            // 角色旋转：模型朝向输入方向
            if (pi.Dmag > 0.1f)
                model.transform.forward = Vector3.Slerp(model.transform.forward, pi.Dvec, 0.3f);
        
            // 角色地面基础速度
            if (!lockPlanar)
                planarVelocity = model.transform.forward * (pi.Dmag * walkSpeed * (pi.run ? runMultiplier : 1.0f));
        }
        else
        {
            // 角色旋转
            if (!trackDirection)
                model.transform.forward = transform.forward;    
            else
                model.transform.forward = planarVelocity.normalized;
            
            // 角色地面基础速度
            if (!lockPlanar)
                planarVelocity = pi.Dvec * (pi.Dmag * walkSpeed * (pi.run ? runMultiplier : 1.0f));
        }
    }

    private void FixedUpdate()
    {
        // 角色移动
        rigid.position += deltaPos;  // 考虑 Root motion 移动量
        // rigid.position += movingVec * Time.fixedDeltaTime;  // 移动 Rigidbody 方式一：修改 position
        rigid.velocity = new Vector3(planarVelocity.x, rigid.velocity.y, planarVelocity.z) + thrustVelocity; // 移动 Rigidbody 方式二：修改 velocity
        
        thrustVelocity = Vector3.zero;
        deltaPos = Vector3.zero;
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
        thrustVelocity = new Vector3(0f, jumpVelocityY, 0f);
        pi.inputEnable = false;
        lockPlanar = true;
        trackDirection = true;
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
        col.material = frictionOne;
        pi.inputEnable = true;
        lockPlanar = false;
        trackDirection = false;
        canAttack = true;
    }

    public void OnGroundExit()
    {
        col.material = frictionZero;
    }

    public void OnFallEnter()
    {
        pi.inputEnable = false;
        lockPlanar = true;
    }

    public void OnRollEnter()
    {
        thrustVelocity = new Vector3(0f, rollVelocity, 0f);
        pi.inputEnable = false;
        lockPlanar = true;
        trackDirection = true;
    }

    public void OnJabEnter()
    {
        pi.inputEnable = false;
        lockPlanar = true;
    }

    public void OnJabUpdate()
    {
        thrustVelocity = model.transform.forward * anim.GetFloat("jabVelocity");
    }

    public void OnAttackIdleEnter()
    {
        pi.inputEnable = true;
        layerLerpTarget = 0f;
    }

    public void OnAttackIdleUpdate()
    {
        float currentWeight = anim.GetLayerWeight(anim.GetLayerIndex("attack"));
        currentWeight = Mathf.Lerp(currentWeight, layerLerpTarget, 0.1f);
        anim.SetLayerWeight(anim.GetLayerIndex("attack"), currentWeight);
    }

    public void OnAttack1hAEnter()
    {
        pi.inputEnable = false;
        layerLerpTarget = 1.0f;
    }

    public void OnAttack1hAUpdate()
    {
        thrustVelocity = model.transform.forward * anim.GetFloat("attack1hAVelocity");
        float currentWeight = anim.GetLayerWeight(anim.GetLayerIndex("attack"));
        currentWeight = Mathf.Lerp(currentWeight, layerLerpTarget, 0.1f);
        anim.SetLayerWeight(anim.GetLayerIndex("attack"), currentWeight);
    }

    public void OnUpdateRM(object _deltaPos)
    {
        if (CheckState("attack1hC", "attack"))
            deltaPos += (Vector3) _deltaPos;
    }
}
