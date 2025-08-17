using Character_Controller.Runtime.Controller.States;
using UnityEngine;

namespace Character_Controller.Runtime.Controller
{
    public class PlayerAnimations : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private SpriteRenderer _spriteRenderer;
        
        private PlayerController _player;
        
        private Animator _animator;
        private int _xSpeedHash;
        private int _ySpeedHash;
        private int _isGroundedHash;
        private int _isSlidingHash;
        private int _isDashingHash;

        private void Awake()
        {
            _player = GetComponent<PlayerController>();
            _animator = GetComponent<Animator>();
            
            // set animations hashes
            _xSpeedHash = Animator.StringToHash("xSpeed");
            _ySpeedHash = Animator.StringToHash("ySpeed");
            _isGroundedHash = Animator.StringToHash("isGrounded");
            _isSlidingHash = Animator.StringToHash("isSliding");
            _isDashingHash = Animator.StringToHash("isDashing");
        }

        private void Update()
        {
            FlipSprite();
            
            if (_player.DashModule.CanDash && _player.DashModule.Request)
                _animator.SetTrigger(_isDashingHash);
        }

        private void LateUpdate()
        {
            _animator.SetFloat(_xSpeedHash, Mathf.Abs(_player.Velocity.x));
            _animator.SetFloat(_ySpeedHash, _player.Velocity.y);
            _animator.SetBool(_isGroundedHash, _player.IsGrounded);
            _animator.SetBool(_isSlidingHash, _player.IsWallSliding);
            _animator.SetBool(_isDashingHash, _player.CurrentState == PlayerStates.Dashing);
        }
        
        private void FlipSprite()
        {
            if (!_spriteRenderer) return;
            if (_player.MovementModule.Direction.x == 0) return;

            _spriteRenderer.flipX = !_player.MovementModule.IsFacingRight;
        }
    }
}
