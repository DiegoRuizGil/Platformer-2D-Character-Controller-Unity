using System;
using UnityEngine;

namespace Character_Controller.Runtime
{
    public class Ladder : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private BoxCollider2D trigger;
        [SerializeField] private Collider2D groundCollider;
        [SerializeField] private Transform topTrigger;
        [SerializeField] private Transform bottomTrigger;

        [Header("Settings")]
        [SerializeField, Min(1)] private int height = 1;
        [SerializeField] private float topTriggerOffset;
        [SerializeField] private float bottomTriggerOffset;

        private void Awake()
        {
            DeactivateCollider();
        }

        public void UpdateHeight()
        {
            spriteRenderer.size = new Vector2(spriteRenderer.size.x, height);
            
            trigger.offset = new Vector2(trigger.offset.x, (height - 1) / -2f);
            trigger.size = new Vector2(trigger.size.x, height + 1);
            
            UpdateTriggersOffset();
        }

        public void UpdateTriggersOffset()
        {
            topTrigger.position = new Vector2(topTrigger.position.x, transform.position.y + topTriggerOffset);
            bottomTrigger.position = new Vector2(bottomTrigger.position.x, transform.position.y - bottomTriggerOffset - height);
        }

        public void ActivateCollider()
        {
            groundCollider.enabled = true;
        }

        public void DeactivateCollider()
        {
            groundCollider.enabled = false;
        }
    }
}