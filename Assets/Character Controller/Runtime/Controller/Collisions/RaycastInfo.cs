using Character_Controller.Runtime.CustomAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace Character_Controller.Runtime.Controller.Collisions
{
    public class RaycastInfo : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private BoxCollider2D boxCollider;
        
        [Header("Settings")]
        [Tooltip("The character's collision skin width")]
        [SerializeField] private float skinWidth = 0.015f;
        [Tooltip("Specifies the length of the raycasts used for collision detection")]
        [SerializeField] private float rayLenght = 0.05f;
        [Tooltip("Sets the number of raycasts to be cast for vertical collision detection")]
        [SerializeField] private int verticalRayCount = 4;
        [Tooltip("Sets the number of raycasts to be cast for horizontal collision detection")]
        [SerializeField] private int horizontalRayCount = 4;
        [Tooltip("Specifies the layers for collision detection")]
        public LayerMask collisionLayers;
        
        [Header("Debug")]
        [SerializeField] private bool showDebugRays = true;
        
        [SerializeField] private RaycastHitInfo hitInfo;
        private BoxCollider2D _currentCollider;
        
        private Raycaster _raycasterDown;
        private Raycaster _raycasterUp;
        private Raycaster _raycasterLeft;
        private Raycaster _raycasterRight;

        public RaycastHitInfo HitInfo => hitInfo;

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
            _raycasterDown = new Raycaster(boxCollider, CollisionDirection.Down, collisionLayers,
                rayLenght, skinWidth, verticalRayCount, showDebugRays);
            _raycasterUp = new Raycaster(boxCollider, CollisionDirection.Up, collisionLayers,
                rayLenght, skinWidth, verticalRayCount, showDebugRays);
            _raycasterLeft = new Raycaster(boxCollider, CollisionDirection.Left, collisionLayers,
                rayLenght, skinWidth, horizontalRayCount, showDebugRays);
            _raycasterRight = new Raycaster(boxCollider, CollisionDirection.Right, collisionLayers,
                rayLenght, skinWidth, horizontalRayCount, showDebugRays);
        }

        private void Update()
        {
            hitInfo.Below = _raycasterDown.CheckCollision();
            hitInfo.Above = _raycasterUp.CheckCollision();
            hitInfo.Left = _raycasterLeft.CheckCollision();
            hitInfo.Right = _raycasterRight.CheckCollision();
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