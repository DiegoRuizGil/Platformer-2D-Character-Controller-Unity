using System.Collections;
using System.Linq;
using Character_Controller.Runtime.Controller.Collisions;
using Character_Controller.Runtime.Controller.Modules;
using Character_Controller.Runtime.Controller.States;
using Character_Controller.Runtime.StateMachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Character_Controller.Runtime.Controller
{
    [RequireComponent(typeof(Rigidbody2D), typeof(RaycastInfo))]
    public class PlayerController : BaseStateMachine<PlayerStates>
    {
        [field: SerializeField] public PlayerMovementData Data { get; private set; }
        [field: Space(10)]
        [field: SerializeField] public PlayerVFX VFX { get; private set; }

        [Header("Colliders")]
        [SerializeField] private BoxCollider2D defaultCollider;
        [SerializeField] private BoxCollider2D crouchCollider;
        
        public Animator Animator { get; private set; }
        
        public PlayerStates CurrentState => _currentState.StateKey;
        public DashModule DashModule;
        public MovementModule MovementModule;
        public JumpModule JumpModule;
        public CrouchModule CrouchModule;
        public ClimbingModule ClimbingModule;

        public bool IsGrounded => _raycastInfo.HitInfo.Below;
        public bool LeftWallHit => _raycastInfo.HitInfo.Left;
        public bool RightWallHit => _raycastInfo.HitInfo.Right;
        public bool IsWallSliding => (LeftWallHit || RightWallHit) && !IsGrounded;

        public Vector2 Velocity
        {
            get => _body.velocity;
            set => _body.velocity = value;
        }

        public Vector2 Direction => _movementAction.ReadValue<Vector2>();

        private Rigidbody2D _body;
        private RaycastInfo _raycastInfo;

        private InputAction _movementAction;

        private void Awake()
        {
            Animator = GetComponent<Animator>();
            
            _body = GetComponent<Rigidbody2D>();
            _body.gravityScale = 0f;
            _raycastInfo = GetComponent<RaycastInfo>();
            _movementAction = InputManager.PlayerActions.Movement;

            DashModule = new DashModule(_body, VFX, Data);
            MovementModule = new MovementModule(_body, VFX);
            JumpModule = new JumpModule(_body, VFX, Data);
            CrouchModule = new CrouchModule(_raycastInfo, defaultCollider, crouchCollider);
            ClimbingModule = new ClimbingModule(_body);
            
            CrouchModule.SetDefaultCollider();
        }

        protected override void Update()
        {
            base.Update();
            
            JumpModule.HandleInputBuffer(Time.deltaTime);
            DashModule.HandleInputBuffer(Time.deltaTime);
            
            if (Direction.x != 0 && !IsInState(PlayerStates.Dashing, PlayerStates.Climbing))
                MovementModule.SetDirectionToFace(Direction.x > 0, IsGrounded);
        }

        private void OnEnable() => EnableInput();
        private void OnDisable() => DisableInput();

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Ladder"))
                ClimbingModule.EnterLadderTrigger(other.GetComponentInParent<Ladder>());
            if (other.CompareTag("Ladder Bottom"))
                ClimbingModule.OnBottomLadder = true;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Ladder"))
                ClimbingModule.ExitLadderTrigger();
            if (other.CompareTag("Ladder Bottom"))
                ClimbingModule.OnBottomLadder = false;
        }

        protected override void SetStates()
        {
            // setting player states
            States.Add(PlayerStates.Grounded, new PlayerGroundedState(PlayerStates.Grounded, this));
            States.Add(PlayerStates.Jumping, new PlayerJumpingState(PlayerStates.Jumping, this));
            States.Add(PlayerStates.Falling, new PlayerFallingState(PlayerStates.Falling, this));
            States.Add(PlayerStates.WallSliding, new PlayerWallSlidingState(PlayerStates.WallSliding, this));
            States.Add(PlayerStates.WallJumping, new PlayerWallJumpingState(PlayerStates.WallJumping, this));
            States.Add(PlayerStates.Dashing, new PlayerDashingState(PlayerStates.Dashing, this));
            States.Add(PlayerStates.Crouching, new PlayerCrouchState(PlayerStates.Crouching, this));
            States.Add(PlayerStates.Climbing, new PlayerClimbingState(PlayerStates.Climbing, this));
            
            // set the player's initial state
            _currentState = States[PlayerStates.Grounded];
        }
        
        private void EnableInput()
        {
            InputManager.PlayerActions.Movement.Enable();

            InputManager.PlayerActions.Jump.started += JumpModule.OnInput;
            InputManager.PlayerActions.Jump.canceled += JumpModule.OnInput;
            InputManager.PlayerActions.Jump.Enable();

            InputManager.PlayerActions.Dash.performed += DashModule.OnInput;
            InputManager.PlayerActions.Dash.Enable();
            
            InputManager.PlayerActions.Crouch.started += CrouchModule.OnInput;
            InputManager.PlayerActions.Crouch.canceled += CrouchModule.OnInput;
            InputManager.PlayerActions.Crouch.Enable();
        }

        private void DisableInput()
        {
            InputManager.PlayerActions.Movement.Disable();
            InputManager.PlayerActions.Jump.Disable();
            InputManager.PlayerActions.Dash.Disable();
            InputManager.PlayerActions.Crouch.Disable();
        }
        
        public void Sleep(float duration)
        {
            StartCoroutine(nameof(PerformSleep), duration);
        }

        private IEnumerator PerformSleep(float duration)
        {
            Time.timeScale = 0;
            yield return new WaitForSecondsRealtime(duration);
            Time.timeScale = 1;
        }

        private bool IsInState(params PlayerStates[] states)
        {
            return states.Any(state => _currentState.StateKey == state);
        }
        
        #if UNITY_EDITOR
        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            string rootStateName = _currentState.Name;
            GUILayout.Label($"<color=black><size=20>State: {rootStateName}</size></color>");
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            GUILayout.Label($"<color=black><size=20>Input: {Direction}</size></color>");
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            GUILayout.Label($"<color=black><size=20>Speed: {Velocity}</size></color>");
            GUILayout.EndHorizontal();
        }
        #endif
    }
}