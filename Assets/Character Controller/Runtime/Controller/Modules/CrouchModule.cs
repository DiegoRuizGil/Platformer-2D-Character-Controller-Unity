using Character_Controller.Runtime.Controller.Collisions;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Character_Controller.Runtime.Controller.Modules
{
    public class CrouchModule
    {
        public bool InputRequest;

        private readonly RaycastInfo _raycastInfo;
        private readonly BoxCollider2D _defaultCollider;
        private readonly BoxCollider2D _crouchCollider;
        
        public CrouchModule(RaycastInfo raycastInfo, BoxCollider2D defaultCollider, BoxCollider2D crouchCollider)
        {
            _raycastInfo = raycastInfo;
            _defaultCollider = defaultCollider;
            _crouchCollider = crouchCollider;
        }

        public void OnInput(InputAction.CallbackContext context)
        {
            InputRequest = context.ReadValueAsButton();
        }

        public void SetDefaultCollider()
        {
            _defaultCollider.enabled = true;
            _crouchCollider.enabled = false;
            _raycastInfo.SetCollider(_defaultCollider);
        }

        public void SetCrouchCollider()
        {
            _defaultCollider.enabled = false;
            _crouchCollider.enabled = true;
            _raycastInfo.SetCollider(_crouchCollider);
        }
    }
}