using PlayerController.States;
using StateMachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerController
{
    [RequireComponent(typeof(Rigidbody2D), typeof(RaycastInfo))]
    public class PlayerController : BaseStateMachine<PlayerStates>
    {
        #region Serialized Fields
        [field: SerializeField] public PlayerMovementData Data { get; private set; }
        [Header("Dependencies")]
        [SerializeField] private SpriteRenderer _spriteRenderer;

        [Space(10)]
        public bool updateInPlayMode;
        #endregion

        #region Private variables
        private Rigidbody2D _rb2d;
        private RaycastInfo _raycastInfo;
        
        private PlayerInputActions _playerInputActions;
        private InputAction _movementAction;

        private Animator _animator;
        private int _idleHash;
        private int _groundedHash;
        private int _fallingHash;
        #endregion
        
        #region Movement Parameters
        public Vector2 MovementDirection => _movementAction.ReadValue<Vector2>();
        public bool LeftWallHit => _raycastInfo.HitInfo.Left;
        public bool RightWallHit => _raycastInfo.HitInfo.Right;
        public Vector2 Velocity
        {
            get => _rb2d.velocity;
            set => _rb2d.velocity = value;
        }
        #endregion
        
        #region Jump Parameters
        private float _lastPressedJumpTime;
        [SerializeField, Space(10)] private int _additionalJumps;
        public bool IsGrounded => _raycastInfo.HitInfo.Below;
        public bool IsTouchingCeiling => _raycastInfo.HitInfo.Above;
        public bool JumpRequest { get; private set; }
        public bool HandleLongJumps { get; private set; }
        public bool IsActiveCoyoteTime { get; set; }
        public int AdditionalJumpsAvailable
        {
            get => _additionalJumps;
            set => _additionalJumps = Mathf.Clamp(value, 0, Data.additionalJumps);
        }
        #endregion

        // #region Corners Detection Properties
        // public float CornerDistanceCorrection => _cornerDistanceCorrection;
        // public bool IsTouchingLeftCorner => _raycastInfo.HitInfo.CornerLeft;
        // public bool IsTouchingRightCorner => _raycastInfo.HitInfo.CornerRight;
        // #endregion

        #region Unity Functions
        private void Awake()
        {
            _rb2d = GetComponent<Rigidbody2D>();
            _rb2d.gravityScale = 0f;
            _raycastInfo = GetComponent<RaycastInfo>();

            _playerInputActions = new PlayerInputActions();

            _animator = GetComponent<Animator>();
            SetAnimationsHash();
        }

        protected override void Start()
        {
            base.Start();
            SetGravityScale(Data.gravityScale);
        }

        protected override void Update()
        {
            base.Update();
            
            ManageJumpBuffer();
        }
        
        private void OnEnable()
        {
            EnableInput();
        }

        private void OnDisable()
        {
            DisableInput();
        }
        #endregion

        #region State Machine Functions
        protected override void SetStates()
        {
            States.Add(PlayerStates.Grounded, new PlayerGroundedState(PlayerStates.Grounded, this));
            States.Add(PlayerStates.Jumping, new PlayerJumpingState(PlayerStates.Jumping, this));
            States.Add(PlayerStates.Falling, new PlayerFallingState(PlayerStates.Falling, this));
            States.Add(PlayerStates.WallSliding, new PlayerWallSlidingState(PlayerStates.WallSliding, this));
            States.Add(PlayerStates.WallJumping, new PlayerWallJumpingState(PlayerStates.WallJumping, this));
            
            _currentState = States[PlayerStates.Grounded];
        }
        #endregion
        
        #region Input
        private void EnableInput()
        {
            _movementAction = _playerInputActions.Player.Movement;
            _movementAction.Enable();

            _playerInputActions.Player.Jump.started += OnJumpAction;
            _playerInputActions.Player.Jump.canceled += OnJumpAction;
            _playerInputActions.Player.Jump.Enable();
        }

        private void DisableInput()
        {
            _movementAction.Disable();
            _playerInputActions.Player.Jump.Disable();
        }
        #endregion

        public void SetGravityScale(float scale)
        {
            _rb2d.gravityScale = scale;
        }
        
        #region Movement Functions
        public void Run(float lerpAmount, bool canAddBonusJumpApex)
        {
            float targetSpeed = MovementDirection.x * Data.runMaxSpeed;
            // smooths change
            targetSpeed = Mathf.Lerp(_rb2d.velocity.x, targetSpeed, lerpAmount);

            float accelRate;
            if (IsGrounded)
            {
                accelRate = Mathf.Abs(MovementDirection.x) > 0.01f
                    ? Data.runAccelAmount
                    : Data.runDecelAmount;
            }
            else
            {
                accelRate = Mathf.Abs(MovementDirection.x) > 0.01f
                    ? Data.runAccelAmount * Data.accelInAirMult
                    : Data.runDecelAmount * Data.decelInAirMult;
            }
            
            if (canAddBonusJumpApex && Mathf.Abs(_rb2d.velocity.y) < Data.jumpHangTimeThreshold)
            {
                // makes the jump feels a bit more bouncy, responsive and natural
                accelRate *= Data.jumpHangAcceleration;
                targetSpeed *= Data.jumpHangMaxSpeedMult;
            }
            
            // momemtun
            if (Data.doConserveMomentum
                && Mathf.Abs(_rb2d.velocity.x) > Mathf.Abs(targetSpeed)
                && Mathf.Sign(_rb2d.velocity.x) == Mathf.Sign(targetSpeed)
                && Mathf.Abs(targetSpeed) > 0.01f
                && IsGrounded)
            {
                accelRate = 0;
            }
            
            float speedDif = targetSpeed - _rb2d.velocity.x;
            float movement = speedDif * accelRate;
            
            _rb2d.AddForce(movement * Vector2.right, ForceMode2D.Force);
        }

        public void Slide()
        {
            // remove the remaining upwards impulse
            if (_rb2d.velocity.y > 0)
                _rb2d.AddForce(-_rb2d.velocity.y * Vector2.up, ForceMode2D.Impulse);

            float speedDif = Data.slideSpeed - _rb2d.velocity.y;
            float movement = speedDif * Data.slideAccel;
            
            //The force applied can't be greater than the (negative) speedDifference * by how many times a second FixedUpdate() is called. For more info research how force are applied to rigidbodies.
            movement = Mathf.Clamp(movement, -Mathf.Abs(speedDif)  * (1 / Time.fixedDeltaTime), Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime));
            
            _rb2d.AddForce(movement * Vector2.up);
        }
        
        public void FlipSprite()
        {
            if (!_spriteRenderer) return;

            bool lookingLeft = _spriteRenderer.flipX;

            if (MovementDirection.x < 0 && !lookingLeft)
            {
                _spriteRenderer.flipX = true;
            }
            else if (MovementDirection.x > 0 && lookingLeft)
            {
                _spriteRenderer.flipX = false;
            }
        }
        #endregion

        #region Jump Functions
        public void Jump()
        {
            JumpRequest = false;
            
            float force = Data.jumpForce;
            
            // avoid shorter jumps when falling and jumping with coyote time
            if (_rb2d.velocity.y < 0)
                force -= _rb2d.velocity.y;
            
            _rb2d.AddForce(Vector2.up * force, ForceMode2D.Impulse);
        }

        public void WallJump(int dir)
        {
            Vector2 force = Data.wallJumpForce;
            force.x *= dir;

            if (Mathf.Sign(_rb2d.velocity.x) != Mathf.Sign(force.x))
                force.x -= _rb2d.velocity.x;

            if (_rb2d.velocity.y < 0)
                force.y -= _rb2d.velocity.y;
            
            _rb2d.AddForce(force, ForceMode2D.Impulse);
        }

        public void ResetAdditionalJumps()
        {
            AdditionalJumpsAvailable = Data.additionalJumps;
        }
        
        private void OnJumpAction(InputAction.CallbackContext context)
        {
            if (context.ReadValueAsButton())
            {
                JumpRequest = true;
                _lastPressedJumpTime = Data.jumpInputBufferTime;
            }
            
            HandleLongJumps = context.ReadValueAsButton();
        }

        private void ManageJumpBuffer()
        {
            if (!JumpRequest) return;
            
            _lastPressedJumpTime -= Time.deltaTime;
            if (_lastPressedJumpTime <= 0)
            {
                JumpRequest = false;
            }
        }
        #endregion
        
        #region Wall Sliding Functions
        public bool CanWallSlide()
        {
            if ((LeftWallHit || RightWallHit) && MovementDirection != Vector2.zero)
                return true;
            
            return false;
        }
        #endregion

        #region Animation Functions
        private void SetAnimationsHash()
        {
            _idleHash = Animator.StringToHash("Idle");
            _groundedHash = Animator.StringToHash("Grounded");
            _fallingHash = Animator.StringToHash("Falling");
        }

        public void AnimSetIdleVariable(bool value)
        {
            _animator.SetBool(_idleHash, value);
        }

        public void AnimSetGroundedVariable(bool value)
        {
            _animator.ResetTrigger(_fallingHash);
            _animator.SetBool(_groundedHash, value);
        }

        public void AnimSetFallingTrigger()
        {
            _animator.SetTrigger(_fallingHash);
        }
        #endregion
        
        #region Debug
        #if UNITY_EDITOR
        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            string rootStateName = _currentState.Name;
            GUILayout.Label($"<color=black><size=20>State: {rootStateName}</size></color>");
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            GUILayout.Label($"<color=black><size=20>Input: {MovementDirection}</size></color>");
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            GUILayout.Label($"<color=black><size=20>Speed: {Velocity}</size></color>");
            GUILayout.EndHorizontal();
        }
        #endif
        #endregion
    }
}