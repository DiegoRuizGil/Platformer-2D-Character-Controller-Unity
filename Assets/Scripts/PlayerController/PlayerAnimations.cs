using UnityEngine;

namespace PlayerController
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

        private void Awake()
        {
            _player = GetComponent<PlayerController>();
            
            _animator = GetComponent<Animator>();
            _xSpeedHash = Animator.StringToHash("xSpeed");
            _ySpeedHash = Animator.StringToHash("ySpeed");
            _isGroundedHash = Animator.StringToHash("isGrounded");
            _isSlidingHash = Animator.StringToHash("isSliding");
        }

        private void Update()
        {
            FlipSprite();
        }

        private void LateUpdate()
        {
            _animator.SetFloat(_xSpeedHash, Mathf.Abs(_player.Velocity.x));
            _animator.SetFloat(_ySpeedHash, _player.Velocity.y);
            _animator.SetBool(_isGroundedHash, _player.IsGrounded);
            _animator.SetBool(_isSlidingHash, _player.IsWallSliding);
        }
        
        private void FlipSprite()
        {
            if (!_spriteRenderer) return;

            bool lookingLeft = _spriteRenderer.flipX;

            if (_player.MovementDirection.x < 0f && !lookingLeft)
            {
                _spriteRenderer.flipX = true;
            }
            else if (_player.MovementDirection.x > 0f && lookingLeft)
            {
                _spriteRenderer.flipX = false;
            }
        }
    }
}
