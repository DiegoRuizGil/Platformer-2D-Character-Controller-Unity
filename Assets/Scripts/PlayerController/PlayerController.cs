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
        public bool IsGrounded => _raycastInfo.HitInfo.Below;
        public bool IsTouchingCeiling => _raycastInfo.HitInfo.Above;
        public bool JumpRequest { get; private set; }
        public bool HandleLongJumps { get; private set; }
        public bool IsActiveCoyoteTime { get; set; }
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
        public void Run(float lerpAmount, float accelRate, bool addBonusJumpApex)
        {
            float targetSpeed = MovementDirection.x * Data.runMaxSpeed;
            // smooths change
            targetSpeed = Mathf.Lerp(_rb2d.velocity.x, targetSpeed, lerpAmount);

            if (addBonusJumpApex)
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
            GUILayout.Label($"<color=black><size=20>RootState: {rootStateName}</size></color>");
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            string xInput = $"{MovementDirection.x}";
            GUILayout.Label($"<color=black><size=20>X Input: {xInput}</size></color>");
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            GUILayout.Label($"<color=black><size=20>Speed: {Velocity}</size></color>");
            GUILayout.EndHorizontal();
        }
        #endif
        #endregion
    }
}