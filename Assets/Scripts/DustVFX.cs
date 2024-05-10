using UnityEngine;

public class DustVFX : MonoBehaviour
{
    // called at the end of the animation
    public void DestroyObject()
    {
        Destroy(gameObject);
    }
}
