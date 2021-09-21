/* UNUSED BECAUSE OF CINEMACHINE
 
using UnityEngine;

namespace Camera
{
    public class CameraCheck : MonoBehaviour
    {
        private bool _colliding;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Ground"))
                _colliding = true;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Ground"))
                _colliding = false;
        }

        public bool IsColliding()
        {
            return _colliding;
        }
    }
}
*/