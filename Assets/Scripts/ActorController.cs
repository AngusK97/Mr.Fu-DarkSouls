using System;
using UnityEngine;

public class ActorController : MonoBehaviour
{
    public GameObject model;
    public PlayerInput pi;
    public float walkSpeed = 2.0f;

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
        anim.SetFloat("forward", pi.Dmag);
        if (pi.Dmag > 0.1f) model.transform.forward = pi.Dvec;
        movingVec = model.transform.forward * (pi.Dmag * walkSpeed);
    }

    private void FixedUpdate()
    {
        // rigid.position += movingVec * Time.fixedDeltaTime;  // 修改 rigid.position 移动角色
        rigid.velocity = new Vector3(movingVec.x, rigid.velocity.y, movingVec.z);   // 修改 rigid.velocity 移动角色
    }
}
