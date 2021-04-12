using UnityEngine;

namespace Player
{
    public class GroundCheck : MonoBehaviour
    {
        private bool _groundCheck;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("Rock"))
                _groundCheck = true;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Ground"))
                _groundCheck = false;
        }

        public bool isOnGround()
        {
            return _groundCheck;
        }
    }
}