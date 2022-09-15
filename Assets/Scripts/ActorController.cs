using UnityEditorInternal;
using UnityEngine;

public class ActorController : MonoBehaviour
{
    public GameObject model;
    public CameraController camCon;
    public IUserInput pi;
    public float walkSpeed = 2.4f;
    public float runMultiplier = 2.0f;
    public float jumpVelocity = 7f;
    public float rollVelocity = 1f;

    [Space(10)]
    [Header("===== Friction Settings =====")]
    public PhysicMaterial frictionOne;
    public PhysicMaterial frictionZero;

    private Animator anim;
    private Rigidbody rigid;
    private Vector3 planarVec;
    private Vector3 thrustVec;
    private bool canAttack;
    private bool lockPlanar = false;
    private bool trackDirection = false;
    private CapsuleCollider col;
    private float lerpTarget;
    private Vector3 deltaPos;

    private void Awake()
    {
        // pi = GetComponent<PlayerInput>();
        IUserInput[] inputs = GetComponents<IUserInput>();
        foreach (var input in inputs)
        {
            if (input.enabled)
            {
                pi = input;
                break;
            }
        }
        anim = model.GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
    }

    private void Update()
    {
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

        // 跳跃
        if (pi.attack && CheckState("ground") && canAttack)
            anim.SetTrigger("attack");

        if (!camCon.lockState)
        {
            // 角色旋转
            if (pi.Dmag > 0.1f)
                model.transform.forward = Vector3.Slerp(model.transform.forward, pi.Dvec, 0.3f);
        
            // 角色移动
            if (!lockPlanar)
                planarVec = model.transform.forward * (pi.Dmag * walkSpeed * (pi.run ? runMultiplier : 1.0f));   
        }
        else
        {
            if (!trackDirection)
                model.transform.forward = transform.forward;    
            else
                model.transform.forward = planarVec.normalized;
            
            if (!lockPlanar)
                planarVec = pi.Dvec * (pi.Dmag * walkSpeed * (pi.run ? runMultiplier : 1.0f));
        }
    }

    private void FixedUpdate()
    {
        rigid.position += deltaPos;
        // rigid.position += movingVec * Time.fixedDeltaTime;  // 修改 rigid.position 移动角色
        rigid.velocity = new Vector3(planarVec.x, rigid.velocity.y, planarVec.z) + thrustVec; // 修改 rigid.velocity 移动角色
        thrustVec = Vector3.zero;
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
        thrustVec = new Vector3(0f, jumpVelocity, 0f);
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
        thrustVec = new Vector3(0f, rollVelocity, 0f);
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
        thrustVec = model.transform.forward * anim.GetFloat("jabVelocity");
    }

    public void OnAttackIdleEnter()
    {
        pi.inputEnable = true;
        lerpTarget = 0f;
    }

    public void OnAttackIdleUpdate()
    {
        float currentWeight = anim.GetLayerWeight(anim.GetLayerIndex("attack"));
        currentWeight = Mathf.Lerp(currentWeight, lerpTarget, 0.1f);
        anim.SetLayerWeight(anim.GetLayerIndex("attack"), currentWeight);
    }

    public void OnAttack1hAEnter()
    {
        pi.inputEnable = false;
        lerpTarget = 1.0f;
    }

    public void OnAttack1hAUpdate()
    {
        thrustVec = model.transform.forward * anim.GetFloat("attack1hAVelocity");
        float currentWeight = anim.GetLayerWeight(anim.GetLayerIndex("attack"));
        currentWeight = Mathf.Lerp(currentWeight, lerpTarget, 0.1f);
        anim.SetLayerWeight(anim.GetLayerIndex("attack"), currentWeight);
    }

    public void OnUpdateRM(object _deltaPos)
    {
        if (CheckState("attack1hC", "attack"))
            deltaPos += (Vector3) _deltaPos;
    }
}
