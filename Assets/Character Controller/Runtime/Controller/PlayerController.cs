using System.Collections;
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
        
        public PlayerStates CurrentState => _currentState.StateKey;
        public DashModule DashModule;
        public MovementModule MovementModule;
        public JumpModule JumpModule;

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
            _body = GetComponent<Rigidbody2D>();
            _body.gravityScale = 0f;
            _raycastInfo = GetComponent<RaycastInfo>();
            _movementAction = InputManager.PlayerActions.Movement;

            DashModule = new DashModule(_body, VFX, Data);
            MovementModule = new MovementModule(_body, VFX);
            JumpModule = new JumpModule(_body, VFX, Data);
        }

        protected override void Update()
        {
            base.Update();
            
            JumpModule.HandleInputBuffer(Time.deltaTime);
            DashModule.HandleInputBuffer(Time.deltaTime);
            
            if (Direction.x != 0 && _currentState.StateKey != PlayerStates.Dashing)
                MovementModule.SetDirectionToFace(Direction.x > 0, IsGrounded);
        }

        private void OnEnable() => EnableInput();
        private void OnDisable() => DisableInput();

        protected override void SetStates()
        {
            // setting player states
            States.Add(PlayerStates.Grounded, new PlayerGroundedState(PlayerStates.Grounded, this));
            States.Add(PlayerStates.Jumping, new PlayerJumpingState(PlayerStates.Jumping, this));
            States.Add(PlayerStates.Falling, new PlayerFallingState(PlayerStates.Falling, this));
            States.Add(PlayerStates.WallSliding, new PlayerWallSlidingState(PlayerStates.WallSliding, this));
            States.Add(PlayerStates.WallJumping, new PlayerWallJumpingState(PlayerStates.WallJumping, this));
            States.Add(PlayerStates.Dashing, new PlayerDashingState(PlayerStates.Dashing, this));
            
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
        }

        private void DisableInput()
        {
            InputManager.PlayerActions.Movement.Disable();
            InputManager.PlayerActions.Jump.Disable();
            InputManager.PlayerActions.Dash.Disable();
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