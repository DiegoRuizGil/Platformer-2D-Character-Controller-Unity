using PlayerController.States;
using StateMachine.Hierarchical;
using UnityEngine;

namespace PlayerController
{
    public enum PlayerStates
    {
        Grounded, Jumping, Falling, Idle, Moving
    }
    
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerStateMachine : BaseStateMachine<PlayerStates>
    {
        [Header("Movement Settings")]
        [SerializeField, Range(0f, 100f)] private float _maxSpeed = 10f;

        private Rigidbody2D _rb2d;
        
        #region Movement Properties

        public float MaxSpeed => _maxSpeed;
        public Vector2 MovementDirection { get; private set; }
        public Vector2 Velocity { get; set; }

        #endregion

        private void Awake()
        {
            _rb2d = GetComponent<Rigidbody2D>();
            _rb2d.gravityScale = 0f; // controlamos la gravidad nosotros
        }

        private void Update()
        {
            // manage input
            // manage rigidbody velocity/movement
            
            UpdateState();
            _rb2d.MovePosition(_rb2d.position + Velocity * Time.deltaTime);
        }

        protected override void SetStates()
        {
            States.Add(PlayerStates.Grounded, new PlayerGroundState(PlayerStates.Grounded, this));
            States.Add(PlayerStates.Jumping, new PlayerJumpingState(PlayerStates.Jumping, this));
            States.Add(PlayerStates.Falling, new PlayerFallingState(PlayerStates.Falling, this));
            States.Add(PlayerStates.Idle, new PlayerIdleState(PlayerStates.Idle, this));
            States.Add(PlayerStates.Moving, new PlayerMovingState(PlayerStates.Moving, this));

            _currentRootState = States[PlayerStates.Grounded];
        }
    }
}