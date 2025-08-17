using UnityEngine;
using UnityEngine.InputSystem;

namespace Character_Controller.Runtime.Controller.Domain
{
    public class MovementModule
    {
        public Vector2 Direction => _inputAction.ReadValue<Vector2>();

        public bool IsFacingRight;
        
        private readonly InputAction _inputAction;
        
        public MovementModule(InputAction inputAction)
        {
            _inputAction = inputAction;
            IsFacingRight = true;
        }
    }
}