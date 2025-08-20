using UnityEngine;

namespace Character_Controller.Runtime.Controller
{
    [System.Serializable]
    public class PlayerVFX
    {
        [Header("Positions")]
        [SerializeField] private Transform bottomVFXPoint;
        [SerializeField] private Transform leftVFXPoint;
        [SerializeField] private Transform rightVFXPoint;
        
        [Header("Prefabs")]
        [SerializeField] private Transform jumpDustVFXPrefab;
        [SerializeField] private Transform dashVFXPrefab;
        [SerializeField] private Transform flipDirectionVFXPrefab;
        [SerializeField] private Transform fallDustVFXPrefab;
        
        public void InstantiateFallDustVFX()
        {
            Object.Instantiate(fallDustVFXPrefab, bottomVFXPoint.position, fallDustVFXPrefab.rotation);
        }
        
        public void InstantiateFlipDirectionVFX(bool isFacingRight)
        {
            Vector3 vfxScale = flipDirectionVFXPrefab.localScale;
            vfxScale.x = isFacingRight ? 1 : -1;
            flipDirectionVFXPrefab.localScale = vfxScale;
            
            Vector3 position = Vector3.zero;
            position.y = bottomVFXPoint.position.y;
            position.x = isFacingRight
                ? leftVFXPoint.position.x
                : rightVFXPoint.position.x;

            Object.Instantiate(flipDirectionVFXPrefab, position, flipDirectionVFXPrefab.rotation);
        }
        
        public void InstantiateDashVFX(bool isFacingRight)
        {
            Vector3 vfxScale = dashVFXPrefab.localScale;
            vfxScale.x = isFacingRight ? 1 : -1;
            dashVFXPrefab.localScale = vfxScale;

            Transform point = isFacingRight ? leftVFXPoint : rightVFXPoint;

            Object.Instantiate(dashVFXPrefab, point.position, dashVFXPrefab.rotation);
        }
        
        public void InstantiateJumpDustVFX()
        {
            Object.Instantiate(jumpDustVFXPrefab, bottomVFXPoint.position, jumpDustVFXPrefab.rotation);
        }
    }
}