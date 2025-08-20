using UnityEngine;

namespace Character_Controller.Runtime.Controller.Modules
{
    public class JumpModule
    {
        public bool Request;
        public bool HandleLongJumps;
        public bool IsActiveCoyoteTime;
        public float LastPressedJumpTime;
        
        public int AdditionalJumpsAvailable
        {
            get => _additionalJumpsAvailable;
            set => _additionalJumpsAvailable = Mathf.Clamp(value, 0, _additionalJumps);
        }

        private int _additionalJumpsAvailable;
        private readonly int _additionalJumps;
        
        public JumpModule(int additionalJumps)
        {
            _additionalJumps = additionalJumps;
        }
    }
}