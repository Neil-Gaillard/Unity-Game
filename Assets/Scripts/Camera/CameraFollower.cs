using Player;
using UnityEngine;

namespace Camera
{
    public class CameraFollower : MonoBehaviour
    {
        // ------Constants ------
        private const float XPosBase = 0.0f;
        private const float YPosBase = 6f;
        private const float ZPosBase = -10.0f;
        
        private const float SmoothTime = 0.4F;
        
        // ------ References ------
        private Transform _cameraTransform;
        
        private Transform _playerTransform;
        
        // ------ Private Attributes ------
        private Vector3 _targetPosition = new Vector3(0, 0, ZPosBase);
        private readonly Vector3 _offset = new Vector3(XPosBase, YPosBase, ZPosBase);
        private Vector3 _velocity = Vector3.zero;
        
        // ------ Event Methods ------

        // Start is called before the first frame update
        private void Awake()
        {
            _playerTransform = GameObject.Find("Player").GetComponent<Transform>();
            _cameraTransform = transform;
        }

        // LateUpdate is called once per frame
        private void FixedUpdate()
        {
            //if (_groundCheck.isOnGround())
                //_yPosition = _playerTransform.position.y + _offset.y;
            
            _targetPosition = _playerTransform.position + _offset;
            
            //_targetPosition.y = _yPosition;
            
            _cameraTransform.position = Vector3.SmoothDamp(_cameraTransform.position, _targetPosition, ref _velocity, SmoothTime);
        }
    }
}