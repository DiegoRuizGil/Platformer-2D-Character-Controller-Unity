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
        #region Serialized Fields
        [field: SerializeField] public PlayerMovementData Data { get; private set; }

        [Header("VFX points")]
        [SerializeField] private Transform _bottonVFXPoint;
        [SerializeField] private Transform _leftVFXPoint;
        [SerializeField] private Transform _rightVFXPoint;
        
        [Header("VFX prefabs")]
        [SerializeField] private Transform _jumpDustVFXPrefab;
        [SerializeField] private Transform _dashVFXPrefab;
        [SerializeField] private Transform _flipDirectionVFXPrefab;
        [SerializeField] private Transform _fallDustVFXPrefab;
        #endregion
        
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
            
            // manage input buffers time
            ManageJumpBuffer();
            DashModule.HandleInputBuffer(Time.deltaTime);
            
            if (MovementModule.Direction.x != 0 && _currentState.StateKey != PlayerStates.Dashing)
                SetDirectionToFace(MovementModule.Direction.x > 0);
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

        #region Jump Functions
        /// <param name="dir">opposite direction of wall</param>
        public void WallJump(int dir)
        {
            Vector2 force = Data.wallJumpForce;
            force.x *= dir; //apply force in opposite direction of wall

            if (Mathf.Sign(_body.velocity.x) != Mathf.Sign(force.x))
                force.x -= _body.velocity.x;

            if (_body.velocity.y < 0)
                force.y -= _body.velocity.y;
            
            _body.AddForce(force, ForceMode2D.Impulse);
            
            InstantiateJumpDustVFX();
        }

        private void ManageJumpBuffer()
        {
            if (!JumpModule.Request) return;
            
            JumpModule.LastPressedJumpTime = JumpModule.LastPressedJumpTime - Time.deltaTime;
            if (JumpModule.LastPressedJumpTime <= 0)
            {
                JumpModule.Request = false;
            }
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
                    InstantiateFlipDirectionVFX();
            }
        }
        #endregion
        
        #region VFX Methods
        public void InstantiateJumpDustVFX()
        {
            Instantiate(_jumpDustVFXPrefab, _bottonVFXPoint.position, _jumpDustVFXPrefab.rotation);
        }

        public void InstantiateDashVFX()
        {
            Vector3 vfxScale = _dashVFXPrefab.localScale;
            vfxScale.x = MovementModule.IsFacingRight ? 1 : -1;
            _dashVFXPrefab.localScale = vfxScale;

            Transform point = MovementModule.IsFacingRight ? _leftVFXPoint : _rightVFXPoint;

            Instantiate(_dashVFXPrefab, point.position, _dashVFXPrefab.rotation);
        }

        public void InstantiateFlipDirectionVFX()
        {
            Vector3 vfxScale = _flipDirectionVFXPrefab.localScale;
            vfxScale.x = MovementModule.IsFacingRight ? 1 : -1;
            _flipDirectionVFXPrefab.localScale = vfxScale;
            
            Vector3 position = Vector3.zero;
            position.y = _bottonVFXPoint.position.y;
            position.x = MovementModule.IsFacingRight
                ? _leftVFXPoint.position.x
                : _rightVFXPoint.position.x;

            Instantiate(_flipDirectionVFXPrefab, position, _flipDirectionVFXPrefab.rotation);
        }

        public void InstantiateFallDustVFX()
        {
            Instantiate(_fallDustVFXPrefab, _bottonVFXPoint.position, _fallDustVFXPrefab.rotation);
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