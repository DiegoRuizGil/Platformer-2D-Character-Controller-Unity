using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Character_Controller.Runtime.Controller.Domain
{
    public class DashModule
    {
        public bool IsActive;
        public bool Request;

        private readonly Timer _inputBuffer;
        private readonly Timer _refillTimer;
        
        private bool _isRefilling;
        
        public bool CanDash => IsActive && !_isRefilling;

        public DashModule(float inputBufferDuration, float refillDuration)
        {
            _inputBuffer = new Timer(inputBufferDuration);
            _refillTimer = new Timer(refillDuration);
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
            _isRefilling = true;
            while (!_refillTimer.Finished)
            {
                _refillTimer.Tick(Time.deltaTime);
                await Task.Yield();
            }
            _refillTimer.Reset();
            _isRefilling = false;
        }
    }
}