/* UNUSED BECAUSE OF CINEMACHINE

using System;
using Player;
using UnityEngine;

namespace Camera
{
    public class CameraFollower : MonoBehaviour
    {
        // ------Constants ------
        private const float XOffset = 0.0f;
        private const float YLowOffset = 7.0f;
        private const float YMiddleOffset = 0f;
        private const float ZOffset = -10.0f;

        private const float SmoothTimeX = 0.3f;
        private const float SmoothTimeYGround = 0.8f;
        private const float SmoothTimeY = 0.3f;

        private const float Tolerence = 15;
        
        // ------GameObject References ------
        private Transform _cameraTransform;

        private CameraCheck _cameraCheck;
        
        // ------Other References ------
        private Transform _playerTransform;
        
        private GroundCheck _playerGroundCheck;

        // ------ Private Attributes ------
        private Vector3 _offset = new Vector3(XOffset, YMiddleOffset, ZOffset);
        private Vector3 _targetPosition = new Vector3(0, 0, ZOffset);
        private Vector3 _velocity = Vector3.zero;

        private bool _low;
        
        // ------ Event Methods ------
        
        private void Awake()
        {
            _playerTransform = GameObject.Find("Player").GetComponent<Transform>();
            _playerGroundCheck = GameObject.Find("Ground Check").GetComponent<GroundCheck>();

            _cameraCheck = GameObject.Find("Camera Check").GetComponent<CameraCheck>();

            _cameraTransform = transform;
        }
        
        private void FixedUpdate()
        {
            ChooseCameraPosition();

            _targetPosition = _playerTransform.position + _offset;

            var newPositionX = Mathf.SmoothDamp(_cameraTransform.position.x, _targetPosition.x, 
                ref _velocity.x, SmoothTimeX);

            float newPositionY;
            
            if (!_playerGroundCheck.isOnGround() &&
                Math.Abs(_playerTransform.position.y - _cameraTransform.position.y) > Tolerence)
                newPositionY = Mathf.SmoothDamp(_cameraTransform.position.y, _playerTransform.position.y,
                    ref _velocity.y, SmoothTimeY);
            else
                newPositionY = Mathf.SmoothDamp(_cameraTransform.position.y, _targetPosition.y,
                    ref _velocity.y, SmoothTimeYGround);

            _cameraTransform.position = new Vector3(newPositionX, newPositionY, ZOffset);
        }

        private void ChooseCameraPosition()
        {
            _low = _cameraCheck.IsColliding();
            _offset.y = _low switch
            {
                false => YMiddleOffset,
                _ => YLowOffset
            };
        }
    }
}
*/