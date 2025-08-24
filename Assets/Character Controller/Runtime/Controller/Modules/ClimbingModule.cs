using UnityEngine;
using UnityEngine.Assertions;

namespace Character_Controller.Runtime.Controller.Modules
{
    public class ClimbingModule
    {
        public bool InputUpRequest => InputManager.PlayerActions.Movement.ReadValue<Vector2>().y > _yInputDeadZone;
        public bool InputDownRequest => InputManager.PlayerActions.Movement.ReadValue<Vector2>().y < -_yInputDeadZone;

        public bool CanClimb => OnLadder && !OnTopLadder;
        public bool CanDescend => OnLadder && OnTopLadder;
        private bool OnLadder => _ladder != null;
        public bool OnBottomLadder = false;
        public bool OnTopLadder = false;

        private readonly Rigidbody2D _body;
        private readonly Transform _playerCenterPoint;
        private readonly Transform _playerBottomPoint;
        private readonly float _yInputDeadZone;
        
        private Ladder _ladder;

        public ClimbingModule(Rigidbody2D body, Transform playerCenterPoint, Transform playerBottomPoint, float yInputDeadZone)
        {
            _body = body;
            _playerCenterPoint = playerCenterPoint;
            _playerBottomPoint = playerBottomPoint;
            _yInputDeadZone = yInputDeadZone;
        }

        public void Climb(float direction, float speed, float acceleration)
        {
            float increment = direction * acceleration;
            float newSpeed = Mathf.Clamp(_body.velocity.y + increment, -speed, speed);
            _body.velocity = new Vector2(0f, newSpeed);
        }

        public void EnterLadderTrigger(Ladder ladder) => _ladder = ladder;

        public void ExitLadderTrigger() => _ladder = null;

        public void SetClimbingPosition()
        {
            Assert.IsTrue(OnLadder);

            SetClimbingXPosition();
            if (OnTopLadder)
                SetDescendingYPosition();
        }

        public void SetGroundedPosition()
        {
            var position = _body.transform.position;
            position.y = _ladder.transform.position.y - _playerBottomPoint.localPosition.y + 0.05f;
            _body.MovePosition(position);
        }
        
        private void SetDescendingYPosition()
        {
            var position = _body.transform.position;
            position.y = _ladder.transform.position.y - _playerCenterPoint.localPosition.y;
            _body.transform.position = position;
        }
        
        private void SetClimbingXPosition()
        {
            var position = _body.transform.position;
            position.x = _ladder.transform.position.x;
            _body.transform.position = position;
        }
    }
}