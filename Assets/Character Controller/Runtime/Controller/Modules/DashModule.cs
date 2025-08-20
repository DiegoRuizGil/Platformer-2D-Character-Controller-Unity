using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Character_Controller.Runtime.Controller.Modules
{
    public class DashModule
    {
        public bool IsActive;
        public bool Request;

        public bool Completed => _dashTimer.Finished;
        public bool CanDash => IsActive && !_isRefilling;

        private readonly Rigidbody2D _body;
        private readonly PlayerVFX _playerVFX;
        private readonly Timer _inputBuffer;
        private readonly Timer _refillTimer;
        private readonly Timer _dashTimer;


        private bool _isRefilling;

        public DashModule(Rigidbody2D body, PlayerVFX playerVFX, PlayerMovementData data)
        {
            _body = body;
            _playerVFX = playerVFX;
            _inputBuffer = new Timer(data.dashInputBufferTime);
            _refillTimer = new Timer(data.dashRefillTime);
            _dashTimer = new Timer(data.dashDuration);
        }

        public void OnInput(InputAction.CallbackContext context)
        {
            if (context.ReadValueAsButton())
            {
                Request = true;
                _inputBuffer.Reset();
            }
        }
        
        public void HandleInputBuffer(float delta)
        {
            if (!Request) return;
            
            _inputBuffer.Tick(delta);
            if (_inputBuffer.Finished)
                Request = false;
        }

        public async Task Refill()
        {
            _dashTimer.Reset();
            _isRefilling = true;
            while (!_refillTimer.Finished)
            {
                _refillTimer.Tick(Time.deltaTime);
                await Task.Yield();
            }
            _refillTimer.Reset();
            _isRefilling = false;
        }

        public void Dash(Vector2 direction, float speed)
        {
            IsActive = false;
            _body.velocity = direction * speed;
            
            _playerVFX.InstantiateDashVFX(direction.x > 0);
        }

        public void UpdateTimer(float delta) => _dashTimer.Tick(delta);
    }
}