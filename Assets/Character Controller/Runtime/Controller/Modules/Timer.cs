using System;

namespace Character_Controller.Runtime.Controller.Modules
{
    public class Timer
    {
        public bool Finished => _remainingTime <= 0;
        
        private float _duration;
        private float _remainingTime;

        public Timer(float duration)
        {
            _remainingTime = duration;
            _duration = duration;
        }

        public void Tick(float delta)
        {
            if (_remainingTime == 0) return;
            
            _remainingTime = Math.Max(_remainingTime - delta, 0);
        }
        
        public void Reset() => _remainingTime = _duration;
    }
}