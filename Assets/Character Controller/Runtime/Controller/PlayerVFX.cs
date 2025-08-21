using UnityEngine;

namespace Character_Controller.Runtime.Controller
{
    [System.Serializable]
    public class PlayerVFX
    {
        [Header("Positions")]
        [SerializeField] private Transform bottomVFXPoint;
        [SerializeField] private Transform leftVFXPoint;
        
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
            
            Vector3 position = new Vector3(leftVFXPoint.position.x, bottomVFXPoint.position.y);
            
            Object.Instantiate(flipDirectionVFXPrefab, position, flipDirectionVFXPrefab.rotation);
        }
        
        public void InstantiateDashVFX(bool isFacingRight)
        {
            Vector3 vfxScale = dashVFXPrefab.localScale;
            vfxScale.x = isFacingRight ? 1 : -1;
            dashVFXPrefab.localScale = vfxScale;

            Object.Instantiate(dashVFXPrefab, leftVFXPoint.position, dashVFXPrefab.rotation);
        }
        
        public void InstantiateJumpDustVFX()
        {
            Object.Instantiate(jumpDustVFXPrefab, bottomVFXPoint.position, jumpDustVFXPrefab.rotation);
        }
    }
}