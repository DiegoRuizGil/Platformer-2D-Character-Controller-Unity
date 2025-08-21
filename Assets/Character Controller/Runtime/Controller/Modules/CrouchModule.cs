using UnityEngine.InputSystem;

namespace Character_Controller.Runtime.Controller.Modules
{
    public class CrouchModule
    {
        public bool Request;

        public void OnInput(InputAction.CallbackContext context)
        {
            Request = context.ReadValueAsButton();
        }
    }
}