using UnityEngine;

namespace Hitrush.Player
{
    public class PlayerAnimationController : MonoBehaviour
    {
        [Header("Animation Parameters")]
        private static readonly int SpeedHash = Animator.StringToHash("Speed");
        private static readonly int IsJumpingHash = Animator.StringToHash("IsJumping");
        private static readonly int IsSlidingHash = Animator.StringToHash("IsSliding");
        private static readonly int IsDeadHash = Animator.StringToHash("IsDead");
        private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");
        
        [Header("Procedural Animation (fallback)")]
        [SerializeField] private Transform bodyTransform;
        [SerializeField] private Transform leftArm, rightArm;
        [SerializeField] private Transform leftLeg, rightLeg;
        [SerializeField] private float runBobSpeed = 10f;
        [SerializeField] private float runBobAmount = 0.05f;
        [SerializeField] private float armSwingAmount = 20f;
        [SerializeField] private float legSwingAmount = 30f;
        
        private Animator animator;
        private bool useProcedural = false;
        private float runTimer;
        
        private void Awake()
        {
            animator = GetComponent<Animator>();
            useProcedural = animator == null || animator.runtimeAnimatorController == null;
        }
        
        private void Update()
        {
            if (useProcedural)
            {
                UpdateProceduralAnimation();
            }
        }
        
        public void SetSpeed(float speed)
        {
            if (!useProcedural && animator != null)
            {
                animator.SetFloat(SpeedHash, speed);
            }
        }
        
        public void SetJumping(bool jumping)
        {
            if (!useProcedural && animator != null)
            {
                animator.SetBool(IsJumpingHash, jumping);
            }
        }
        
        public void SetSliding(bool sliding)
        {
            if (!useProcedural && animator != null)
            {
                animator.SetBool(IsSlidingHash, sliding);
            }
        }
        
        public void SetDead(bool dead)
        {
            if (!useProcedural && animator != null)
            {
                animator.SetBool(IsDeadHash, dead);
            }
        }
        
        public void SetGrounded(bool grounded)
        {
            if (!useProcedural && animator != null)
            {
                animator.SetBool(IsGroundedHash, grounded);
            }
        }
        
        private void UpdateProceduralAnimation()
        {
            if (bodyTransform == null) return;
            
            runTimer += Time.deltaTime * runBobSpeed;
            
            // Body bob
            float bobY = Mathf.Sin(runTimer) * runBobAmount;
            bodyTransform.localPosition = new Vector3(0, bobY, 0);
            
            // Arm swing (opposite to legs)
            float armSwing = Mathf.Sin(runTimer) * armSwingAmount;
            if (leftArm != null) leftArm.localRotation = Quaternion.Euler(armSwing, 0, 0);
            if (rightArm != null) rightArm.localRotation = Quaternion.Euler(-armSwing, 0, 0);
            
            // Leg swing
            float legSwing = Mathf.Sin(runTimer + Mathf.PI) * legSwingAmount;
            if (leftLeg != null) leftLeg.localRotation = Quaternion.Euler(legSwing, 0, 0);
            if (rightLeg != null) rightLeg.localRotation = Quaternion.Euler(-legSwing, 0, 0);
        }
        
        public void SetProceduralJumping(bool jumping)
        {
            // Could add jump-specific procedural animation here
        }
        
        public void SetProceduralSliding(bool sliding)
        {
            // Could add slide-specific procedural animation here
        }
    }
}
