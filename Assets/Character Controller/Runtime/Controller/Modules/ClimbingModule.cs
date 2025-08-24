using UnityEngine;
using UnityEngine.Assertions;

namespace Character_Controller.Runtime.Controller.Modules
{
    public class ClimbingModule
    {
        public bool InputRequest => InputManager.PlayerActions.Movement.ReadValue<Vector2>().y > 0;
        public bool OnLadder => _ladder != null;
        public bool OnBottomLadder;

        private readonly Rigidbody2D _body;
        
        private Ladder _ladder;

        public ClimbingModule(Rigidbody2D body)
        {
            _body = body;
        }

        public void Climb(float direction, float speed, float acceleration)
        {
            float increment = direction * acceleration;
            float newSpeed = Mathf.Clamp(_body.velocity.y + increment, -speed, speed);
            _body.velocity = new Vector2(0f, newSpeed);
        }

        public void EnterLadderTrigger(Ladder ladder) => _ladder = ladder;

        public void ExitLadderTrigger() => _ladder = null;

        public void SetPosition()
        {
            Assert.IsTrue(OnLadder);
            
            var position = _body.transform.position;
            position.x = _ladder.transform.position.x;
            _body.transform.position = position;
        }

        public void ActivateLadderCollider()
        {
            Assert.IsTrue(OnLadder);
            
            _ladder.ActivateCollider();
        }

        public void DeactivateLadderCollider()
        {
            Assert.IsTrue(OnLadder);
            
            _ladder.DeactivateCollider();
        }
    }
}