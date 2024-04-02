using PlayerController.States;
using StateMachine.Hierarchical;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerController
{
    public enum PlayerStates
    {
        Grounded, Jumping, Falling, WallSliding, WallJumping, Idle, Moving
    }
    
    [RequireComponent(typeof(Rigidbody2D), typeof(RaycastInfo))]
    public class PlayerStateMachine : BaseStateMachine<PlayerStates>
    {
        #region Serialized Fields
        [Header("Movement Settings")]
        [Tooltip("Specifies the maximum player's horizontal speed")]
        [SerializeField, Min(0f)] private float _maxHorizontalSpeed = 10f;
        
        [Header("Jump Settings")]
        [Tooltip("Specifies  the maximum height the player can reach when jumping, in Unity units")]
        [SerializeField, Min(0f)] private float _maxJumpHeight = 4f;
        [Tooltip("Specifies the time in seconds it takes for the player to reach the apex of a jump")]
        [SerializeField, Min(0f)] private float _timeToReachJumpApex = 0.4f;
        [Tooltip("Defines the time in seconds during which jump input can be processed after falling from a platform")]
        [SerializeField, Min(0f)] private float _coyoteTime = 0.1f;
        [Tooltip("Specifies the duration in seconds during which jump input can be processed")]
        [SerializeField, Min(0f)] private float _jumpBufferTime = 0.2f;

        [Header("Wall Sliding Settings")]
        [Tooltip("Sets the velocity at which the player descends while sliding on a wall")]
        [SerializeField, Min(0f)] private float _wallSlidingVelocity = 5f;
        [Tooltip("Specifies the horizontal distance the player will cover when performing a jump from a wall, in Unity units")]
        [SerializeField, Min(0f)] private float _wallJumpDistance = 1.5f;
        [Tooltip("Determines the horizontal speed gained by the player when jumping off a wall")]
        [SerializeField, Min(0f)] private float _wallJumpHorizontalSpeed = 10f;
        public float verticalVelocityMultiplier = 1.5f;
        
        [Header("Gravity Settings")]
        [Tooltip("Adjusts the gravity scale applied to the player while falling")]
        [SerializeField, Min(1f)] private float _fallGravityMultiplier = 2.5f;
        [Tooltip("Modifies the gravity scale applied to the player when the jump button is released, resulting in a lower jump")]
        [SerializeField, Min(1f)] private float _lowJumpGravityMultiplier = 2f;

        [Header("Corner Correction Settings")]
        [Tooltip("Specifies the horizontal distance the player will be displaced upon colliding with a corner while jumping")]
        [SerializeField, Min(0f)] private float _cornerDistanceCorrection = 0.25f;
        
        [Header("Dependencies")]
        [SerializeField] private SpriteRenderer _spriteRenderer;
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
        
        #region Movement Properties
        public float MaxHorizontalSpeed => _maxHorizontalSpeed;
        public Vector2 MovementDirection => _movementAction.ReadValue<Vector2>();
        public bool LeftWallHit => _raycastInfo.HitInfo.Left;
        public bool RightWallHit => _raycastInfo.HitInfo.Right;
        public Vector2 Velocity { get; private set; }
        private Vector2 _oldVelocity;
        #endregion
        
        #region Jump Properties
        public bool IsGrounded => _raycastInfo.HitInfo.Below;
        public bool IsTouchingCeiling => _raycastInfo.HitInfo.Above;
        public float CoyoteTime => _coyoteTime;
        public bool HasCoyoteTime { get; set; }
        public int JumpRequests { get; private set; }
        public bool HandleLongJumps { get; private set; }
        public float CheckGroundAfterJump => 0.05f;
        public float Gravity { get; private set; }
        public float JumpVelocity { get; private set; }
        #endregion

        #region Corners Detection Properties
        public float CornerDistanceCorrection => _cornerDistanceCorrection;
        public bool IsTouchingLeftCorner => _raycastInfo.HitInfo.CornerLeft;
        public bool IsTouchingRightCorner => _raycastInfo.HitInfo.CornerRight;
        #endregion
        
        #region Wall Sliding Properties
        public float WallSlidingVelocity => _wallSlidingVelocity;
        public float WallJumpDistance => _wallJumpDistance;
        public float WallJumpHorizontalSpeed => _wallJumpHorizontalSpeed;
        #endregion
        
        #region Gravity Properties
        public float FallGravityMultiplier => _fallGravityMultiplier;
        public float LowJumpGravityMultiplier => _lowJumpGravityMultiplier;
        #endregion

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
            SetJumpParameters();
            base.Start();
            
            InvokeRepeating(nameof(ManageJumpRequest), 0f, _jumpBufferTime);
        }

        private void Update()
        {
            _oldVelocity = Velocity;
            
            UpdateState();
        }

        private void FixedUpdate()
        {
            // Vector2 deltaPosition = (_oldVelocity + Velocity) * (0.5f * Time.deltaTime);
            Vector2 deltaPosition = Velocity * Time.deltaTime;
            _rb2d.MovePosition(_rb2d.position + deltaPosition);
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
            States.Add(PlayerStates.Grounded, new PlayerGroundState(PlayerStates.Grounded, this));
            States.Add(PlayerStates.Jumping, new PlayerJumpingState(PlayerStates.Jumping, this));
            States.Add(PlayerStates.Falling, new PlayerFallingState(PlayerStates.Falling, this));
            
            States.Add(PlayerStates.WallSliding, new PlayerWallSlidingState(PlayerStates.WallSliding, this));
            States.Add(PlayerStates.WallJumping, new PlayerWallJumpingState(PlayerStates.WallJumping, this));
            
            States.Add(PlayerStates.Idle, new PlayerIdleState(PlayerStates.Idle, this));
            States.Add(PlayerStates.Moving, new PlayerMovingState(PlayerStates.Moving, this));

            _currentRootState = States[PlayerStates.Grounded];
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
        
        #region Movement Functions
        public void SetHorizontalVelocity(float value)
        {
            float xVelocity = Mathf.Clamp(value, -1 * _maxHorizontalSpeed, _maxHorizontalSpeed);
            Velocity = new Vector2(xVelocity, Velocity.y);
        }
        
        public void SetVerticalVelocity(float value)
        {
            float yVelocity = Mathf.Clamp(value, -1.5f * JumpVelocity, JumpVelocity);
            Velocity = new Vector2(Velocity.x, yVelocity);
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
        private void SetJumpParameters()
        {
            /*
             * g = -2*h/t_h^2
             * v = -g*t_h
             */
            Gravity = -2f * _maxJumpHeight / Mathf.Pow(_timeToReachJumpApex, 2);
            JumpVelocity = Mathf.Abs(Gravity) * _timeToReachJumpApex;
        }

        private void OnJumpAction(InputAction.CallbackContext context)
        {
            if (context.ReadValueAsButton()) // if true -> button just pressed
                JumpRequests += 1;
            
            HandleLongJumps = context.ReadValueAsButton();
        }

        public void ManageJumpRequest()
        {
            JumpRequests = Mathf.Clamp(JumpRequests - 1, 0, int.MaxValue);
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
            string rootStateName = _currentRootState.Name;
            GUILayout.Label($"<color=black><size=20>RootState: {rootStateName}</size></color>");
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            string subStateName = "";
            if (_currentRootState.CurrentSubState != null)
                subStateName = _currentRootState.CurrentSubState.Name;
            GUILayout.Label($"<color=black><size=20>SubState: {subStateName}</size></color>");
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