using UnityEngine;

namespace PlayerController
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class RaycastInfo : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float skinWidth = 0.015f;
        [SerializeField] private float rayLenght = 0.05f;
        [SerializeField] private int verticalRayCount = 4;
        [SerializeField] private int horizontalRayCount = 4;
        [SerializeField] private LayerMask collisionLayers;
        
        private BoxCollider2D _collider;

        private float _verticalRaySpacing;
        private float _horizontalRaySpacing;

        [Header("Debug")]
        [SerializeField] private bool showDebugRays = true;
        [SerializeField] private RaycastHitInfo _hitInfo;
        public RaycastHitInfo HitInfo => _hitInfo;
        
        [System.Serializable]
        public struct RaycastHitInfo
        {
            public bool Left, Right, Above, Below;

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
        }

        private void Update()
        {
            CheckVerticalCollisions();
            CheckHorizontalCollisions();
        }

        #region Vertical Raycasts
        private void SetVerticalRaySpacing()
        {
            Bounds bounds = _collider.bounds;
            bounds.Expand(skinWidth * -2);
    
            verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);
            _verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
        }

        private void CheckVerticalCollisions()
        {
            CheckLowerVerticalCollisions();
            CheckUpperVerticalCollisions();
        }

        private void CheckLowerVerticalCollisions()
        {
            Bounds bounds = _collider.bounds;
            bounds.Expand(skinWidth * -2);
            bool hasHit = false;

            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = new Vector2(bounds.min.x, bounds.min.y);
                rayOrigin += Vector2.right * (_verticalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, rayLenght, collisionLayers);

                Color raycastColor = Color.red;
                if (hit)
                {
                    hasHit = true;
                    raycastColor = Color.green;
                }
                
                if (showDebugRays)
                    Debug.DrawRay(rayOrigin, Vector2.down * rayLenght, raycastColor);
            }

            _hitInfo.Below = hasHit;
        }
        
        private void CheckUpperVerticalCollisions()
        {
            Bounds bounds = _collider.bounds;
            bounds.Expand(skinWidth * -2);
            bool hasHit = false;

            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = new Vector2(bounds.min.x, bounds.max.y);
                rayOrigin += Vector2.right * (_verticalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLenght, collisionLayers);
            
                Color raycastColor = Color.red;
                if (hit)
                {
                    hasHit = true;
                    raycastColor = Color.green;
                }
                
                if (showDebugRays)
                    Debug.DrawRay(rayOrigin, Vector2.up * rayLenght, raycastColor);
            }

            _hitInfo.Above = hasHit;
        }
        #endregion
        
        #region Horizontal Raycasts
        private void SetHorizontalRaySpacing()
        {
            Bounds bounds = _collider.bounds;
            bounds.Expand(skinWidth * -2);
    
            horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
            _horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        }
        
        private void CheckHorizontalCollisions()
        {
            CheckLeftHorizontalCollisions();
            CheckRightHorizontalCollisions();
        }

        private void CheckLeftHorizontalCollisions()
        {
            Bounds bounds = _collider.bounds;
            bounds.Expand(skinWidth * -2);
            bool hasHit = false;

            for (int i = 0; i < horizontalRayCount; i++)
            {
                Vector2 rayOrigin = new Vector2(bounds.min.x, bounds.min.y);
                rayOrigin += Vector2.up * (_horizontalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.left, rayLenght, collisionLayers);

                Color raycastColor = Color.red;
                if (hit)
                {
                    hasHit = true;
                    raycastColor = Color.green;
                }
                
                if (showDebugRays)
                    Debug.DrawRay(rayOrigin, Vector2.left * rayLenght, raycastColor);
            }

            _hitInfo.Left = hasHit;
        }
        
        private void CheckRightHorizontalCollisions()
        {
            Bounds bounds = _collider.bounds;
            bounds.Expand(skinWidth * -2);
            bool hasHit = false;

            for (int i = 0; i < horizontalRayCount; i++)
            {
                Vector2 rayOrigin = new Vector2(bounds.max.x, bounds.min.y);
                rayOrigin += Vector2.up * (_horizontalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right, rayLenght, collisionLayers);
            
                Color raycastColor = Color.red;
                if (hit)
                {
                    hasHit = true;
                    raycastColor = Color.green;
                }
                
                if (showDebugRays)
                    Debug.DrawRay(rayOrigin, Vector2.right * rayLenght, raycastColor);
            }

            _hitInfo.Right = hasHit;
        }
        #endregion
    }
}