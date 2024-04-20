using UnityEngine;

namespace PlayerController
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class RaycastInfo : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("The character's collision skin width")]
        [SerializeField] private float _skinWidth = 0.015f;
        [Tooltip("Specifies the length of the raycasts used for collision detection")]
        [SerializeField] private float _rayLenght = 0.05f;
        [Tooltip("Specifies the length of the raycasts used to detect corners at the top")]
        [SerializeField] private float _cornersRayLenght = 0.1f;
        [Tooltip("Sets the number of raycasts to be cast for vertical collision detection")]
        [SerializeField] private int _verticalRayCount = 4;
        [Tooltip("Sets the number of raycasts to be cast for horizontal collision detection")]
        [SerializeField] private int _horizontalRayCount = 4;
        [Tooltip("Specifies the layers for collision detection")]
        [SerializeField] private LayerMask _collisionLayers;
        
        [Header("Debug")]
        [SerializeField] private bool _showDebugRays = true;
        
        [SerializeField] private RaycastHitInfo _hitInfo;
        private BoxCollider2D _collider;

        private float _verticalRaySpacing;
        private float _horizontalRaySpacing;

        private const int _cornersRayCount = 3;
        private float _cornersRaySpacing;
        
        public RaycastHitInfo HitInfo => _hitInfo;
        
        [System.Serializable]
        public struct RaycastHitInfo
        {
            public bool Left, Right, Above, Below;
            public bool CornerLeft, CornerRight;

            public void Reset()
            {
                Left = false;
                Right = false;
                Above = false;
                Below = false;
            }
        }

        private void Awake()
        {
            _collider = GetComponent<BoxCollider2D>();
            
            SetVerticalRaySpacing();
            SetHorizontalRaySpacing();
            SetCornersRaySpacing();
        }

        private void Update()
        {
            CheckVerticalCollisions();
            CheckHorizontalCollisions();
            CheckForCorners();
        }

        #region Vertical Raycasts
        private void SetVerticalRaySpacing()
        {
            Bounds bounds = _collider.bounds;
            bounds.Expand(_skinWidth * -2);
    
            _verticalRayCount = Mathf.Clamp(_verticalRayCount, 2, int.MaxValue);
            _verticalRaySpacing = bounds.size.x / (_verticalRayCount - 1);
        }

        private void CheckVerticalCollisions()
        {
            CheckLowerVerticalCollisions();
            CheckUpperVerticalCollisions();
        }

        private void CheckLowerVerticalCollisions()
        {
            Bounds bounds = _collider.bounds;
            bounds.Expand(_skinWidth * -2);
            bool hasHit = false;

            for (int i = 0; i < _verticalRayCount; i++)
            {
                Vector2 rayOrigin = new Vector2(bounds.min.x, bounds.min.y);
                rayOrigin += Vector2.right * (_verticalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, _rayLenght, _collisionLayers);

                Color raycastColor = Color.red;
                if (hit)
                {
                    hasHit = true;
                    raycastColor = Color.green;
                }
                
                if (_showDebugRays)
                    Debug.DrawRay(rayOrigin, Vector2.down * _rayLenght, raycastColor);
            }

            _hitInfo.Below = hasHit;
        }
        
        private void CheckUpperVerticalCollisions()
        {
            Bounds bounds = _collider.bounds;
            bounds.Expand(_skinWidth * -2);
            bool hasHit = false;

            for (int i = 0; i < _verticalRayCount; i++)
            {
                Vector2 rayOrigin = new Vector2(bounds.min.x, bounds.max.y);
                rayOrigin += Vector2.right * (_verticalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, _rayLenght, _collisionLayers);
            
                Color raycastColor = Color.red;
                if (hit)
                {
                    hasHit = true;
                    raycastColor = Color.green;
                }
                
                if (_showDebugRays)
                    Debug.DrawRay(rayOrigin, Vector2.up * _rayLenght, raycastColor);
            }

            _hitInfo.Above = hasHit;
        }
        #endregion
        
        #region Horizontal Raycasts
        private void SetHorizontalRaySpacing()
        {
            Bounds bounds = _collider.bounds;
            bounds.Expand(_skinWidth * -2);
    
            _horizontalRayCount = Mathf.Clamp(_horizontalRayCount, 2, int.MaxValue);
            _horizontalRaySpacing = bounds.size.y / (_horizontalRayCount - 1);
        }
        
        private void CheckHorizontalCollisions()
        {
            CheckLeftHorizontalCollisions();
            CheckRightHorizontalCollisions();
        }

        private void CheckLeftHorizontalCollisions()
        {
            Bounds bounds = _collider.bounds;
            bounds.Expand(_skinWidth * -2);
            bool hasHit = false;

            for (int i = 0; i < _horizontalRayCount; i++)
            {
                Vector2 rayOrigin = new Vector2(bounds.min.x, bounds.min.y);
                rayOrigin += Vector2.up * (_horizontalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.left, _rayLenght, _collisionLayers);

                Color raycastColor = Color.red;
                if (hit)
                {
                    hasHit = true;
                    raycastColor = Color.green;
                }
                
                if (_showDebugRays)
                    Debug.DrawRay(rayOrigin, Vector2.left * _rayLenght, raycastColor);
            }

            _hitInfo.Left = hasHit;
        }
        
        private void CheckRightHorizontalCollisions()
        {
            Bounds bounds = _collider.bounds;
            bounds.Expand(_skinWidth * -2);
            bool hasHit = false;

            for (int i = 0; i < _horizontalRayCount; i++)
            {
                Vector2 rayOrigin = new Vector2(bounds.max.x, bounds.min.y);
                rayOrigin += Vector2.up * (_horizontalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right, _rayLenght, _collisionLayers);
            
                Color raycastColor = Color.red;
                if (hit)
                {
                    hasHit = true;
                    raycastColor = Color.green;
                }
                
                if (_showDebugRays)
                    Debug.DrawRay(rayOrigin, Vector2.right * _rayLenght, raycastColor);
            }

            _hitInfo.Right = hasHit;
        }
        #endregion
        
        #region Corners Raycasts
        private void SetCornersRaySpacing()
        {
            Bounds bounds = _collider.bounds;
            bounds.Expand(_skinWidth * -2);
    
            _cornersRaySpacing = bounds.size.x / (_cornersRayCount - 1);
        }
        
        private void CheckForCorners()
        {
            DrawCornersRays();
            
            Bounds bounds = _collider.bounds;
            bounds.Expand(_skinWidth * -2);

            // rays origin
            Vector2 leftRayOrigin = new Vector2(bounds.min.x, bounds.max.y);
            Vector2 middleRayOrigin = leftRayOrigin + Vector2.right * _cornersRaySpacing;
            Vector2 rightRayOrigin = leftRayOrigin + Vector2.right * (_cornersRaySpacing * 2);

            // racyasts hit
            RaycastHit2D leftHit = Physics2D.Raycast(leftRayOrigin, Vector2.up, _cornersRayLenght, _collisionLayers);
            RaycastHit2D middleHit = Physics2D.Raycast(middleRayOrigin, Vector2.up, _cornersRayLenght, _collisionLayers);
            RaycastHit2D rightHit = Physics2D.Raycast(rightRayOrigin, Vector2.up, _cornersRayLenght, _collisionLayers);

            if (!middleHit && !leftHit && !rightHit)
            {
                _hitInfo.CornerLeft = false;
                _hitInfo.CornerRight = false;
                return;
            }
            
            // too far from corners
            if (middleHit)
            {
                _hitInfo.CornerLeft = false;
                _hitInfo.CornerRight = false;
                return;
            }
            
            if (leftHit)
            {
                _hitInfo.CornerLeft = false;
                _hitInfo.CornerRight = true;
                return;
            }
            
            if (rightHit)
            {
                _hitInfo.CornerLeft = true;
                _hitInfo.CornerRight = false;
            }
        }

        private void DrawCornersRays()
        {
            if (!_showDebugRays) return;
            
            Bounds bounds = _collider.bounds;
            bounds.Expand(_skinWidth * -2);

            for (int i = 0; i < _cornersRayCount; i++)
            {
                Vector2 rayOrigin = new Vector2(bounds.min.x, bounds.max.y);
                rayOrigin += Vector2.right * (_cornersRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, _cornersRayLenght, _collisionLayers);
            
                Color raycastColor = Color.blue;
                if (hit)
                {
                    raycastColor = Color.yellow;
                }
                
                if (_showDebugRays)
                    Debug.DrawRay(rayOrigin, Vector2.up * _cornersRayLenght, raycastColor);
            }
        }
        #endregion
    }
}