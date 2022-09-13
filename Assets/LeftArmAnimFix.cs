using UnityEngine;

public class LeftArmAnimFix : MonoBehaviour
{
    public Vector3 a;
    
    private Animator anim;
    private Transform leftLowerArm;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        leftLowerArm = anim.GetBoneTransform(HumanBodyBones.LeftLowerArm);
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (!anim.GetBool("defense"))
        {
            leftLowerArm.localEulerAngles += a;
            anim.SetBoneLocalRotation(HumanBodyBones.LeftLowerArm, Quaternion.Euler(leftLowerArm.localEulerAngles));
        }
    }
}
