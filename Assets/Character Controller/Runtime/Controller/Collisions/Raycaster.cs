using System;
using UnityEngine;

namespace Character_Controller.Runtime.Controller.Collisions
{
    public enum CollisionDirection
    {
        Down, Up, Left, Right
    }
    
    public class Raycaster
    {
        private BoxCollider2D _collider;
        private readonly CollisionDirection _direction;
        private readonly LayerMask _layerMask;
        private readonly float _rayLenght;
        private readonly float _skinWidth;
        private int _rayCount;

        private float _raySpacing;
        private Vector2 _rayShitDirection;
        private Vector2 _rayDirection;
        
        private readonly bool _debug;

        public Raycaster(BoxCollider2D collider, CollisionDirection direction, LayerMask layerMask, float rayLenght, float skinWidth, int rayCount, bool debug = false)
        {
            _direction = direction;
            _layerMask = layerMask;
            _rayLenght = rayLenght;
            _skinWidth = skinWidth;
            _rayCount = rayCount;
            
            _debug = debug;
            
            SetCollider(collider);
        }

        public bool CheckCollision()
        {
            var bounds = _collider.bounds;
            bounds.Expand(_skinWidth * -2);
            var hasHit = false;

            for (var i = 0; i < _rayCount; i++)
            {
                var rayOrigin = GetInitialRayOrigin();
                rayOrigin += _rayShitDirection * (_raySpacing * i);
                var hit = Physics2D.Raycast(rayOrigin, _rayDirection, _rayLenght, _layerMask);
                
                var rayCastColor = Color.red;
                if (hit)
                {
                    hasHit = true;
                    rayCastColor = Color.green;
                }
                
                if (_debug)
                    Debug.DrawRay(rayOrigin, _rayDirection * _rayLenght, rayCastColor);
            }
            
            return hasHit;
        }

        public void SetCollider(BoxCollider2D collider)
        {
            _collider = collider;
            
            SetRaySpacing();
            SetRayDirectionInfo();
        }
        
        private void SetRaySpacing()
        {
            var bounds = _collider.bounds;
            bounds.Expand(_skinWidth * -2);

            _rayCount = Mathf.Clamp(_rayCount, 2, int.MaxValue);

            _raySpacing = _direction switch
            {
                CollisionDirection.Down or CollisionDirection.Up => bounds.size.x / (_rayCount - 1),
                CollisionDirection.Left or CollisionDirection.Right => bounds.size.y / (_rayCount - 1),
                _ => _raySpacing
            };
        }

        private void SetRayDirectionInfo()
        {
            switch (_direction)
            {
                case CollisionDirection.Down:
                    _rayShitDirection = Vector2.right;
                    _rayDirection = Vector2.down;
                    break;
                case CollisionDirection.Up:
                    _rayShitDirection = Vector2.right;
                    _rayDirection = Vector2.up;
                    break;
                case CollisionDirection.Left:
                    _rayShitDirection = Vector2.up;
                    _rayDirection = Vector2.left;
                    break;
                case CollisionDirection.Right:
                    _rayShitDirection = Vector2.up;
                    _rayDirection = Vector2.right;
                    break;
            }
        }

        private Vector2 GetInitialRayOrigin()
        {
            var bounds = _collider.bounds;
            bounds.Expand(_skinWidth * -2);

            return _direction switch
            {
                CollisionDirection.Down => new Vector2(bounds.min.x, _collider.bounds.min.y),
                CollisionDirection.Up => new Vector2(bounds.min.x, _collider.bounds.max.y),
                CollisionDirection.Left => new Vector2(bounds.min.x, _collider.bounds.min.y),
                CollisionDirection.Right => new Vector2(bounds.max.x, _collider.bounds.min.y),
                _ => Vector2.zero
            };
        }
    }
}