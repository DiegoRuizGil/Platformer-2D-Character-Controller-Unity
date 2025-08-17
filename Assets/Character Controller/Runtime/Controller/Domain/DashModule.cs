using UnityEngine.InputSystem;

namespace Character_Controller.Runtime.Controller.Domain
{
    public class DashModule
    {
        public Timer InputBuffer { get; private set; }
        public bool IsRefilling;
        public bool IsActive;
        public bool Request;

        public bool CanDash => IsActive && !IsRefilling;

        public DashModule(float inputBufferDuration)
        {
            InputBuffer = new Timer(inputBufferDuration);
        }

        public void OnInput(InputAction.CallbackContext context)
        {
            if (context.ReadValueAsButton())
            {
                Request = true;
                InputBuffer.Reset();
            }
        }
    }
}