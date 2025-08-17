using System.Collections;
using Character_Controller.Runtime.Controller.Domain;
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
        
        #region Private variables
        private Rigidbody2D _rb2d;
        private RaycastInfo _raycastInfo;
        
        private InputAction _movementAction;
        #endregion

        public DashModule DashModule;
        
        #region Movement Parameters
        public Vector2 MovementDirection => _movementAction.ReadValue<Vector2>();
        public bool LeftWallHit => _raycastInfo.HitInfo.Left;
        public bool RightWallHit => _raycastInfo.HitInfo.Right;
        public Vector2 Velocity
        {
            get => _rb2d.velocity;
            set => _rb2d.velocity = value;
        }
        public bool IsFacingRight { get; private set; }
        #endregion
        
        #region Jump Parameters
        private float _lastPressedJumpTime;
        private int _additionalJumps;
        public bool IsGrounded => _raycastInfo.HitInfo.Below;
        public bool IsWallSliding => (LeftWallHit || RightWallHit) && !IsGrounded;
        public bool JumpRequest { get; private set; }
        public bool HandleLongJumps { get; private set; }
        public bool IsActiveCoyoteTime { get; set; }
        public int AdditionalJumpsAvailable
        {
            get => _additionalJumps;
            set => _additionalJumps = Mathf.Clamp(value, 0, Data.additionalJumps);
        }
        #endregion

        #region Unity Functions
        private void Awake()
        {
            _rb2d = GetComponent<Rigidbody2D>();
            _rb2d.gravityScale = 0f;
            _raycastInfo = GetComponent<RaycastInfo>();

            DashModule = new DashModule(Data.dashInputBufferTime);
        }

        protected override void Start()
        {
            base.Start();
            SetGravityScale(Data.gravityScale);

            IsFacingRight = true;
        }

        protected override void Update()
        {
            base.Update();
            
            // manage input buffers time
            ManageJumpBuffer();
            ManageDashBuffer();
            
            if (MovementDirection.x != 0 && _currentState.StateKey != PlayerStates.Dashing)
                SetDirectionToFace(MovementDirection.x > 0);
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
            _movementAction = InputManager.PlayerActions.Movement;
            _movementAction.Enable();

            InputManager.PlayerActions.Jump.started += OnJumpAction;
            InputManager.PlayerActions.Jump.canceled += OnJumpAction;
            InputManager.PlayerActions.Jump.Enable();

            InputManager.PlayerActions.Dash.performed += DashModule.OnInput;
            InputManager.PlayerActions.Dash.Enable();
        }

        private void DisableInput()
        {
            _movementAction.Disable();
            InputManager.PlayerActions.Jump.Disable();
            InputManager.PlayerActions.Dash.Disable();
        }
        #endregion
        
        #region Movement Functions
        public void Run(float lerpAmount, bool canAddBonusJumpApex)
        {
            float targetSpeed = MovementDirection.x * Data.runMaxSpeed;
            // smooths change
            targetSpeed = Mathf.Lerp(_rb2d.velocity.x, targetSpeed, lerpAmount);

            // Gets an acceleration value based on if we are accelerating (includes turning) 
            // or trying to decelerate (stop). As well as applying a multiplier if we're air borne.
            float accelRate;
            if (IsGrounded)
            {
                accelRate = Mathf.Abs(targetSpeed) > 0.01f
                    ? Data.runAccelAmount
                    : Data.runDecelAmount;
            }
            else
            {
                accelRate = Mathf.Abs(targetSpeed) > 0.01f
                    ? Data.runAccelAmount * Data.accelInAirMult
                    : Data.runDecelAmount * Data.decelInAirMult;
            }
            
            if (canAddBonusJumpApex && Mathf.Abs(_rb2d.velocity.y) < Data.jumpHangTimeThreshold)
            {
                // makes the jump feels a bit more bouncy, responsive and natural
                accelRate *= Data.jumpHangAccelerationMult;
                targetSpeed *= Data.jumpHangMaxSpeedMult;
            }
            
            float speedDif = targetSpeed - _rb2d.velocity.x;
            float movement = speedDif * accelRate;
            
            _rb2d.AddForce(movement * Vector2.right, ForceMode2D.Force);
            // same as:
            // _rb2d.velocity = new Vector2(
            //      _rb2d.velocity.x + (Time.fixedDeltaTime * speedDif * accelRate) / _rb2d.mass,
            //      _rb2d.velocity.y);
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

            InstantiateJumpDustVFX();
        }

        /// <param name="dir">opposite direction of wall</param>
        public void WallJump(int dir)
        {
            Vector2 force = Data.wallJumpForce;
            force.x *= dir; //apply force in opposite direction of wall

            if (Mathf.Sign(_rb2d.velocity.x) != Mathf.Sign(force.x))
                force.x -= _rb2d.velocity.x;

            if (_rb2d.velocity.y < 0)
                force.y -= _rb2d.velocity.y;
            
            _rb2d.AddForce(force, ForceMode2D.Impulse);
            
            InstantiateJumpDustVFX();
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
                _lastPressedJumpTime = Data.jumpInputBufferTime; // reset buffer time
            }
            
            // if still pressing jump button, perform long jump
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
        
        #region Dash Functions
        public void RefillDash()
        {
            StartCoroutine(nameof(PerformRefillDash));
        }
        
        private IEnumerator PerformRefillDash()
        {
            DashModule.IsRefilling = true;
            yield return new WaitForSeconds(Data.dashRefillTime);
            DashModule.IsRefilling = false;
        }

        private void ManageDashBuffer()
        {
            if (!DashModule.Request) return;
            
            DashModule.InputBuffer.Tick(Time.deltaTime);
            if (DashModule.InputBuffer.Finished)
            {
                DashModule.Request = false;
            }
        }
        #endregion
        
        #region General Methods
        public void SetGravityScale(float scale)
        {
            _rb2d.gravityScale = scale;
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
        
        public void SetDirectionToFace(bool isMovingRight)
        {
            if (isMovingRight != IsFacingRight)
            {
                IsFacingRight = !IsFacingRight;
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
            vfxScale.x = IsFacingRight ? 1 : -1;
            _dashVFXPrefab.localScale = vfxScale;

            Transform point = IsFacingRight ? _leftVFXPoint : _rightVFXPoint;

            Instantiate(_dashVFXPrefab, point.position, _dashVFXPrefab.rotation);
        }

        public void InstantiateFlipDirectionVFX()
        {
            Vector3 vfxScale = _flipDirectionVFXPrefab.localScale;
            vfxScale.x = IsFacingRight ? 1 : -1;
            _flipDirectionVFXPrefab.localScale = vfxScale;
            
            Vector3 position = Vector3.zero;
            position.y = _bottonVFXPoint.position.y;
            position.x = IsFacingRight
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