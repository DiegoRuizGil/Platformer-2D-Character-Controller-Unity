namespace Character_Controller.Runtime.Controller.Domain
{
    public struct DashParams
    {
        public float LastPressedTime;
        public bool IsRefilling;
        public bool IsActive;
        public bool Request;
        
        public bool CanDash => IsActive && !IsRefilling;
    }
}