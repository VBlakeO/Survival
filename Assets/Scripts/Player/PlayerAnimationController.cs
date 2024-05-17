using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{   
    [SerializeField] private PlayerMovementSystem playerMovement = null;
    [SerializeField] private Animator anim = null;
    [SerializeField] private float distanceToGround = 1;
    [SerializeField] private float range = 1f;
    [SerializeField] private LayerMask layerMask = 1;


    private void Start() 
    {
        playerMovement.OnLanding += Land;
    }

    void Update()
    {
        anim.SetFloat("SpeedY", playerMovement.currentDir.y);
        anim.SetFloat("SpeedX", playerMovement.currentDir.x);

        anim.SetBool("isSprinting", playerMovement.isSprinting);
        anim.SetBool("isCrouched", playerMovement.isCrouched);
        anim.SetBool("isWalking", playerMovement.isWalking);
        anim.SetBool("isJumping", playerMovement.isJumping);
        anim.SetBool("isFalling", !playerMovement.isGrounded && playerMovement.velocityY < 0);
    }

    void Land()
    {
        if(!playerMovement.isCrouched)
        {
            anim.SetTrigger("Land");
            //anim.Play("Land",0, 0);
            print("Land");
        }
    }

    private void OnAnimatorIK(int layerIndex) 
    {
        if (anim)
        {
            anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1f);
            anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1f);

            anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1f);
            anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1f);

            //LeftFoot
            RaycastHit hit;
            Ray ray = new Ray(anim.GetIKPosition(AvatarIKGoal.LeftFoot) + Vector3.up, Vector3.down);
            if (Physics.Raycast(ray, out hit, distanceToGround + range, layerMask))
            {
                if (hit.transform.tag == "Walkable")
                {
                    Vector3 footPosition = hit.point;
                    footPosition.y += distanceToGround;
                    anim.SetIKPosition(AvatarIKGoal.LeftFoot, footPosition);
                    anim.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.LookRotation(playerMovement.transform.forward, hit.normal));
                }
            }

            //RightFoot
            ray = new Ray(anim.GetIKPosition(AvatarIKGoal.RightFoot) + Vector3.up, Vector3.down);
            if (Physics.Raycast(ray, out hit, distanceToGround + range, layerMask))
            {
                if (hit.transform.tag == "Walkable")
                {
                    Vector3 footPosition = hit.point;
                    footPosition.y += distanceToGround;
                    anim.SetIKPosition(AvatarIKGoal.RightFoot, footPosition);
                    anim.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.LookRotation(playerMovement.transform.forward, hit.normal));
                }
            }
        }    
    }
}
