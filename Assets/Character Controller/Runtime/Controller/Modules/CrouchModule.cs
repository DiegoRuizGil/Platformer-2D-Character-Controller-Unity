using Character_Controller.Runtime.Controller.Collisions;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Character_Controller.Runtime.Controller.Modules
{
    public class CrouchModule
    {
        public bool InputRequest;
        public bool CeilingAbove => _ceilingRaycaster.CheckCollision();
        
        private readonly RaycastInfo _raycastInfo;
        private readonly BoxCollider2D _defaultCollider;
        private readonly BoxCollider2D _crouchCollider;
        
        private readonly Raycaster _ceilingRaycaster;
        
        public CrouchModule(RaycastInfo raycastInfo, BoxCollider2D defaultCollider, BoxCollider2D crouchCollider)
        {
            _raycastInfo = raycastInfo;
            _defaultCollider = defaultCollider;
            _crouchCollider = crouchCollider;

            _ceilingRaycaster = new Raycaster(crouchCollider, CollisionDirection.Up, raycastInfo.collisionLayers,
                defaultCollider.size.y - crouchCollider.size.y + 0.05f, 0.015f, 3);
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