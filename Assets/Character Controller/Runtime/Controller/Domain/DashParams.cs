namespace Character_Controller.Runtime.Controller.Domain
{
    public struct DashParams
    {
        public float LastPressedBuffer { get; private set; }
        public bool IsRefilling;
        public bool IsActive;
        public bool Request;
        
        public bool CanDash => IsActive && !IsRefilling;

        public void TickBuffer(float delta) => LastPressedBuffer -= delta;
        public void ResetBuffer(float duration) => LastPressedBuffer = duration;
    }
}