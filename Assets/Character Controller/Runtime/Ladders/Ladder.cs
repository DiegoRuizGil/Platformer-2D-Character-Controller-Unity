using UnityEngine;

namespace Character_Controller.Runtime
{
    public class Ladder : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private BoxCollider2D trigger;

        [Header("Settings")]
        [SerializeField, Min(1)] private int height = 1;
        
        public void UpdateHeight()
        {
            spriteRenderer.size = new Vector2(spriteRenderer.size.x, height);
            
            trigger.offset = new Vector2(trigger.offset.x, height / -2f);
            trigger.size = new Vector2(trigger.size.x, height);
        }
    }
}