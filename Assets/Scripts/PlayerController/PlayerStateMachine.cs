using PlayerController.States;
using StateMachine.Hierarchical;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerController
{
    public enum PlayerStates
    {
        Grounded, Jumping, Falling, Idle, Moving
    }
    
    [RequireComponent(typeof(Rigidbody2D), typeof(RaycastInfo))]
    public class PlayerStateMachine : BaseStateMachine<PlayerStates>
    {
        [Header("Movement Settings")]
        [SerializeField] private float _maxSpeed = 10f;
        
        [Header("Jump Settings")]
        [SerializeField] private float _maxJumpHeight = 4f;
        [SerializeField] private float _timeToReachJumpApex = 0.4f;
        [SerializeField] private float _coyoteTime = 0.1f;
        [SerializeField] private float _jumpBufferTime = 0.2f;
        [SerializeField] private float _fallGravityMultiplier = 2.5f;
        [SerializeField] private float _lowJumpGravityMultiplier = 2f;

        [Header("Dependencies")]
        [SerializeField] private SpriteRenderer _spriteRenderer;
        
        private Rigidbody2D _rb2d;
        private RaycastInfo _raycastInfo;
        
        private PlayerInputActions _playerInputActions;
        private InputAction _movementAction;
        
        #region Movement Properties
        public float MaxSpeed => _maxSpeed;
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
        public float FallGravityMultiplier => _fallGravityMultiplier;
        public float LowJumpGravityMultiplier => _lowJumpGravityMultiplier;
        public int JumpRequests { get; private set; }
        public bool HandleLongJumps { get; private set; }
        public float CheckGroundAfterJump => 0.05f;
        public float Gravity { get; private set; }
        public float JumpVelocity { get; private set; }
        #endregion

        #region Animation Properties
        public Animator Animator { get; private set; }
        public int IdleHash { get; private set; }
        public int GroundedHash { get; private set; }
        public int FallingHash { get; private set; }
        #endregion

        #region Unity Functions
        private void Awake()
        {
            _rb2d = GetComponent<Rigidbody2D>();
            _rb2d.gravityScale = 0f;
            _raycastInfo = GetComponent<RaycastInfo>();

            _playerInputActions = new PlayerInputActions();

            Animator = GetComponent<Animator>();
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
            float xVelocity = Mathf.Clamp(value, -1 * _maxSpeed, _maxSpeed);
            Velocity = new Vector2(xVelocity, Velocity.y);
        }
        
        public void SetVerticalVelocity(float value)
        {
            float yVelocity = Mathf.Clamp(value, -2 * JumpVelocity, JumpVelocity);
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
            
            Debug.Log($"JUMP PARAMETERS: Gravity = {Gravity} / JumpVelocity = {JumpVelocity}");
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

        #region Animation Functions
        private void SetAnimationsHash()
        {
            IdleHash = Animator.StringToHash("Idle");
            GroundedHash = Animator.StringToHash("Grounded");
            FallingHash = Animator.StringToHash("Falling");
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
            string subStateName = _currentRootState.CurrentSubState.Name;
            GUILayout.Label($"<color=black><size=20>SubState: {subStateName}</size></color>");
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            string xInput = $"{MovementDirection.x}";
            GUILayout.Label($"<color=black><size=20>X Input: {xInput}</size></color>");
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            string speed = $"({Velocity.x}, {Velocity.y})";
            GUILayout.Label($"<color=black><size=20>Speed: {speed}</size></color>");
            GUILayout.EndHorizontal();
        }
        #endif
        #endregion
    }
}