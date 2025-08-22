using Character_Controller.Runtime.CustomAttributes;
using UnityEngine;

namespace Character_Controller.Runtime.Controller.Collisions
{
    public class RaycastInfo : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private BoxCollider2D boxCollider;
        
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
        private BoxCollider2D _currentCollider;
        
        private Raycaster _raycasterDown;
        private Raycaster _raycasterUp;
        private Raycaster _raycasterLeft;
        private Raycaster _raycasterRight;

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
            _raycasterDown = new Raycaster(boxCollider, CollisionDirection.Down, _collisionLayers,
                _rayLenght, _skinWidth, _verticalRayCount, _showDebugRays);
            _raycasterUp = new Raycaster(boxCollider, CollisionDirection.Up, _collisionLayers,
                _rayLenght, _skinWidth, _verticalRayCount, _showDebugRays);
            _raycasterLeft = new Raycaster(boxCollider, CollisionDirection.Left, _collisionLayers,
                _rayLenght, _skinWidth, _horizontalRayCount, _showDebugRays);
            _raycasterRight = new Raycaster(boxCollider, CollisionDirection.Right, _collisionLayers,
                _rayLenght, _skinWidth, _horizontalRayCount, _showDebugRays);
        }

        private void Update()
        {
            _hitInfo.Below = _raycasterDown.CheckCollision();
            _hitInfo.Above = _raycasterUp.CheckCollision();
            _hitInfo.Left = _raycasterLeft.CheckCollision();
            _hitInfo.Right = _raycasterRight.CheckCollision();
        }

        public void SetCollider(BoxCollider2D boxCollider)
        {
            _raycasterDown.SetCollider(boxCollider);
            _raycasterUp.SetCollider(boxCollider);
            _raycasterLeft.SetCollider(boxCollider);
            _raycasterRight.SetCollider(boxCollider);
        }
    }
}