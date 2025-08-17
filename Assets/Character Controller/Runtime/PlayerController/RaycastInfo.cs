using Character_Controller.Runtime.CustomAttributes;
using UnityEngine;

namespace Character_Controller.Runtime.PlayerController
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class RaycastInfo : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("The character's collision skin width")]
        [SerializeField] private float _skinWidth = 0.015f;
        [Tooltip("Specifies the length of the raycasts used for collision detection")]
        [SerializeField] private float _rayLenght = 0.05f;
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

        private float _cornersRaySpacing;
        
        public RaycastHitInfo HitInfo => _hitInfo;
        
        [System.Serializable]
        public struct RaycastHitInfo
        {
            [ReadOnly] public bool Left, Right, Above, Below;

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
            
            // calculate the space between each raycast
            SetVerticalRaySpacing();
            SetHorizontalRaySpacing();
        }

        private void Update()
        {
            // check for collisions
            CheckVerticalCollisions();
            CheckHorizontalCollisions();
        }

        #region Collisions
        enum CollisionType
        {
            LowerVertical, UpperVertical, LeftHorizontal, RightHorizontal
        }
        
        private void CheckForCollisions(CollisionType type)
        {
            Bounds bounds = _collider.bounds;
            bounds.Expand(_skinWidth * -2);
            
            switch (type)
            {
                case CollisionType.LowerVertical:
                    _hitInfo.Below = CheckForCollisions(
                        _verticalRayCount,
                        _verticalRaySpacing,
                        new Vector2(bounds.min.x, bounds.min.y),
                        Vector2.right,
                        Vector2.down);
                    break;
                
                case CollisionType.UpperVertical:
                    _hitInfo.Above = CheckForCollisions(
                        _verticalRayCount,
                        _verticalRaySpacing,
                        new Vector2(bounds.min.x, bounds.max.y),
                        Vector2.right,
                        Vector2.up);
                    break;
                
                case CollisionType.LeftHorizontal:
                    _hitInfo.Left = CheckForCollisions(
                        _horizontalRayCount,
                        _horizontalRaySpacing,
                        new Vector2(bounds.min.x, bounds.min.y),
                        Vector2.up,
                        Vector2.left);
                    break;
                
                case CollisionType.RightHorizontal:
                    _hitInfo.Right = CheckForCollisions(
                        _horizontalRayCount,
                        _horizontalRaySpacing,
                        new Vector2(bounds.max.x, bounds.min.y),
                        Vector2.up,
                        Vector2.right);
                    break;
            }
        }

        /// <summary>
        /// Check for raycast collisions
        /// </summary>
        /// <param name="rayCount">number of raycasts to be cast</param>
        /// <param name="raySpacing">space between each raycast</param>
        /// <param name="startRayOrigin">starting position of the first raycast on that side</param>
        /// <param name="raycastShiftDirection">direction in which the position of the raycasts will be shifted</param>
        /// <param name="raycastDirection">raycasts direction</param>
        /// <returns>whether or not there has been a collision</returns>
        private bool CheckForCollisions(int rayCount, float raySpacing, Vector2 startRayOrigin,
            Vector2 raycastShiftDirection, Vector2 raycastDirection)
        {
            Bounds bounds = _collider.bounds;
            bounds.Expand(_skinWidth * -2);
            bool hasHit = false;

            for (int i = 0; i < rayCount; i++)
            {
                Vector2 rayOrigin = startRayOrigin;
                rayOrigin += raycastShiftDirection * (raySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, raycastDirection, _rayLenght, _collisionLayers);

                Color raycastColor = Color.red;
                if (hit)
                {
                    hasHit = true;
                    raycastColor = Color.green;
                }
                
                if (_showDebugRays)
                    Debug.DrawRay(rayOrigin, raycastDirection * _rayLenght, raycastColor);
            }

            return hasHit;
        }
        #endregion

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
            CheckForCollisions(CollisionType.LowerVertical);
            CheckForCollisions(CollisionType.UpperVertical);
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
            CheckForCollisions(CollisionType.LeftHorizontal);
            CheckForCollisions(CollisionType.RightHorizontal);
        }
        #endregion
    }
}