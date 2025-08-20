using System.Collections;
using Character_Controller.Runtime.Controller.Modules;
using Character_Controller.Runtime.Controller.States;
using Character_Controller.Runtime.StateMachine;
using UnityEngine;

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

        private Rigidbody2D _body;
        private RaycastInfo _raycastInfo;

        #region Unity Functions
        private void Awake()
        {
            _body = GetComponent<Rigidbody2D>();
            _body.gravityScale = 0f;
            _raycastInfo = GetComponent<RaycastInfo>();

            DashModule = new DashModule(Data.dashInputBufferTime, Data.dashRefillTime);
            MovementModule = new MovementModule(_body, InputManager.PlayerActions.Movement);
            JumpModule = new JumpModule(_body, Data.additionalJumps, Data.jumpInputBufferTime);
        }

        protected override void Start()
        {
            base.Start();
            MovementModule.SetGravityScale(Data.gravityScale);
        }

        protected override void Update()
        {
            base.Update();
            
            JumpModule.HandleInputBuffer(Time.deltaTime);
            DashModule.HandleInputBuffer(Time.deltaTime);
            
            if (MovementModule.Direction.x != 0 && _currentState.StateKey != PlayerStates.Dashing)
                SetDirectionToFace(MovementModule.Direction.x > 0);
        }

        private void OnEnable() => EnableInput();
        private void OnDisable() => DisableInput();

        #endregion

        #region State Machine Functions
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
        #endregion
        
        #region Input
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
        #endregion
        
        #region General Methods
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
        
        public void SetDirectionToFace(bool isMovingRight)
        {
            MovementModule.SetDirectionToFace(isMovingRight);
            if (isMovingRight != MovementModule.IsFacingRight)
            {
                if (IsGrounded)
                    VFX.InstantiateFlipDirectionVFX(MovementModule.IsFacingRight);
            }
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
            GUILayout.Label($"<color=black><size=20>Input: {MovementModule.Direction}</size></color>");
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            GUILayout.Label($"<color=black><size=20>Speed: {Velocity}</size></color>");
            GUILayout.EndHorizontal();
        }
        #endif
        #endregion
    }
}