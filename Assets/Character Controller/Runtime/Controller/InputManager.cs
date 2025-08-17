using UnityEngine;

namespace Character_Controller.Runtime.Controller
{
    public static class InputManager
    {
        public static PlayerInputActions.PlayerActions PlayerActions => PlayerInputActions.Player;
        
        private static PlayerInputActions _playerInputActions;

        private static PlayerInputActions PlayerInputActions
        {
            get
            {
                if (_playerInputActions == null)
                {
                    _playerInputActions = new PlayerInputActions();
                    _playerInputActions.Enable();
                }
                return _playerInputActions;
            }
        }
        
#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            _playerInputActions = null;
        }
#endif
    }
}