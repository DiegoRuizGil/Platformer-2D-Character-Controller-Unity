using Character_Controller.Runtime.CustomAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace Character_Controller.Runtime.Controller
{
    [CreateAssetMenu(fileName = "New Player Movement Data", menuName = "Player Movement Data")]
    public class PlayerMovementData : ScriptableObject
    {
        [Header("GRAVITY")]
        [Tooltip("Force needed for the desired Jump Height and Jump Time To Apex")]
        [ReadOnly] public float gravityStrength;
        [Tooltip("Strength of the player's gravity as a multiplier of gravity (value of rigidbody2d.gravityScale)")]
        [ReadOnly] public float gravityScale;
        [Space(5)]
        [Tooltip("Multiplier to the player's gravityScale when falling")]
        public float fallGravityMult;
        [Tooltip("Maximum fall speed of the player when falling")]
        public float maxFallSpeed;
        [Space(5)]
        [Tooltip("Larger multiplier to the player's gravityScale when falling anf downwards input is pressed")]
        public float fastFallGravityMult;
        [Tooltip("Maximum fall speed of the player when falling when performing a faster fall")]
        public float maxFastFallSpeed;

        [Space(20)]
        
        [Header("RUN")]
        [Tooltip("Target speed we want the player to reach")]
        public float runMaxSpeed;
        public float acceleration;
        [Range(0f, 1f)] public float groundDecay;
        [Range(0f, 1f)] public float airDecay;

        [Space(20)]
        [Header("CROUCH")]
        public float crouchSpeed;
        
        [Space(20)]
        
        [Header("JUMP")]
        [Tooltip("Height of the player's jump")]
        public float jumpHeight;
        [Tooltip("Time between applying the jump force and reaching the desired jump height")]
        public float jumpTimeToApex;
        [Tooltip("The actual force applied to the player when jumping")]
        [ReadOnly] public float jumpForce;
        [Space(5)]
        public int additionalJumps;

        [Header("BOTH JUMPS")]
        [Tooltip("Multiplier to increase gravity if the player releases the jump button while jumping")]
        public float jumpCutGravity;
        [Tooltip("Reduces gravity while close to the apex of the jump")]
        [Range(0, 1)] public float jumpHangGravityMult;
        [Tooltip("Speeds (close to 0) where the player will experience extra 'jump hang'. The player's velocity.y is closest to 0 at the jump's apex")]
        public float jumpHangTimeThreshold;

        [Header("WALL JUMP")]
        [Tooltip("Force applied to the player when wall jumping")]
        public Vector2 wallJumpForce;
        [Space(5)]
        [Tooltip("Reduces the effect of player's movement while wall jumping")]
        [Range(0, 1)] public float wallJumpRunLerp;
        public float wallJumpAccelerationMult;
        public float wallJumpMinDistance;

        [Space(20)]
        
        [Header("SLIDE")]
        [Tooltip("Target speed we want the player to reach while sliding")]
        public float slideSpeed;
        [Tooltip("The speed at which the player accelerates to max speed, can be set to runMaxSpeed to instant acceleration down to 0 for none at all")]
        public float slideAccel;
        [Tooltip("Time the input must be active to detach from the wall")]
        [Range(0f, 0.1f)] public float wallSlideReleaseTime;

        [Header("DASH")]
        [Tooltip("Speed of the dash")]
        public float dashSpeed;
        [Tooltip("Duration of the dash")]
        public float dashDuration;
        [Tooltip("Time to recharge the dash after dashing")]
        public float dashRefillTime;
        [Tooltip("Time that the timeScale will be set to 0 when a dash is performed")]
        public float dashSleepTime;

        [Header("ASSISTS")]
        [Tooltip("Margin time to perform a jump while the player is falling")]
        [Range(0.01f, 0.5f)] public float coyoteTime;
        [Tooltip("Margin time in which the jump input can be processed")]
        [Range(0.01f, 0.5f)] public float jumpInputBufferTime;
        [Tooltip("Margin time in which the dash input can be processed")]
        [Range(0.01f, 0.5f)] public float dashInputBufferTime;

        private void OnValidate()
        {
            gravityStrength = -(2 * jumpHeight) / (jumpTimeToApex * jumpTimeToApex);
            gravityScale = gravityStrength / Physics2D.gravity.y;
            
            jumpForce = Mathf.Abs(gravityStrength) * jumpTimeToApex;
        }
    }
}
