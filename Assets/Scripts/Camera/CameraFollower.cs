using Player;
using UnityEngine;

namespace Camera
{
    public class CameraFollower : MonoBehaviour
    {
        // ------Constants ------
        private const float XPosBase = 0.0f;
        private const float YPosBase = 4.0f;
        private const float ZPosBase = -10.0f;
        
        private const float SmoothTime = 0.13F;
        
        // ------ References ------
        private Transform _cameraTransform;
        
        private Transform _playerTransform;
        private GroundCheck _groundCheck;
        
        // ------ Private Attributes ------
        private Vector3 _targetPosition = new Vector3(0, 0, ZPosBase);
        private readonly Vector3 _offset = new Vector3(XPosBase, YPosBase, ZPosBase);
        private Vector3 _velocity = Vector3.zero;
        
        private float _yPosition = YPosBase;
        
        // ------ Event Methods ------

        // Start is called before the first frame update
        private void Awake()
        {
            _playerTransform = GameObject.Find("Player").GetComponent<Transform>();
            _groundCheck = GameObject.Find("Ground Check").GetComponent<GroundCheck>();
            _cameraTransform = transform;
        }

        private void Start()
        {
            _yPosition = _playerTransform.position.y + _offset.y;
        }


        // LateUpdate is called once per frame
        private void LateUpdate()
        {
            if (_groundCheck.isOnGround())
                _yPosition = _playerTransform.position.y + _offset.y;
            
            _targetPosition = _playerTransform.position + _offset;
            
            _targetPosition.y = _yPosition;
            
            _cameraTransform.position = Vector3.SmoothDamp(_cameraTransform.position, _targetPosition, ref _velocity, SmoothTime);
        }
    }
}