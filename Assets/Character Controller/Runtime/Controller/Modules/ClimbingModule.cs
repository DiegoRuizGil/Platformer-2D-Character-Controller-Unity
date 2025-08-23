using Character_Controller.Runtime.Controller.Collisions;
using UnityEngine;
using UnityEngine.Assertions;

namespace Character_Controller.Runtime.Controller.Modules
{
    public class ClimbingModule
    {
        public bool InputRequest => InputManager.PlayerActions.Movement.ReadValue<Vector2>().y > 0;
        public bool OnLadder => _ladder != null;

        private readonly Rigidbody2D _body;
        private readonly Raycaster _groundRaycaster;
        
        private Ladder _ladder;

        public ClimbingModule(Rigidbody2D body, BoxCollider2D collider, LayerMask groundLayer)
        {
            _body = body;
            _groundRaycaster = new Raycaster(collider, CollisionDirection.Down, groundLayer, 0.05f, 0.015f, 3);
        }

        public void Climb(float direction, float speed, float acceleration)
        {
            float increment = direction * acceleration;
            float newSpeed = Mathf.Clamp(_body.velocity.y + increment, -speed, speed);
            _body.velocity = new Vector2(0f, newSpeed);
        }

        public void EnterLadder(Ladder ladder) => _ladder = ladder;
        public void ExitLadder() => _ladder = null;

        public void SetPosition()
        {
            Assert.IsTrue(OnLadder);
            
            var position = _body.transform.position;
            position.x = _ladder.transform.position.x;
            _body.transform.position = position;
        }
    }
}