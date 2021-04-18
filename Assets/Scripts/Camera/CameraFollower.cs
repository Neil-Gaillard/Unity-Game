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
        private const float YHighOffset = -6.0f;
        private const float ZOffset = -10.0f;

        private const float SmoothTimeX = 0.4F;
        private const float SmoothTimeYGround = 0.8F;
        private const float SmoothTimeY = 0.2F;

        // ------ Private Attributes ------
        private readonly Vector3 _offset = new Vector3(XOffset, YMiddleOffset, ZOffset);

        // ------GameObject References ------
        private Transform _cameraTransform;

        // ------Other References ------
        private Transform _playerTransform;
        private GroundCheck _playerGroundCheck;

        private Vector3 _targetPosition = new Vector3(0, 0, ZOffset);
        private Vector3 _velocity = Vector3.zero;

        // ------ Event Methods ------

        // Start is called before the first frame update
        private void Awake()
        {
            _playerTransform = GameObject.Find("Player").GetComponent<Transform>();
            _playerGroundCheck = GameObject.Find("Ground Check").GetComponent<GroundCheck>();
            _cameraTransform = transform;
        }

        // LateUpdate is called once per frame
        private void FixedUpdate()
        {
            _targetPosition = _playerTransform.position + _offset;

            float newPositionX = Mathf.SmoothDamp(_cameraTransform.position.x, _targetPosition.x, ref _velocity.x,
                SmoothTimeX);
            
            float newPositionY;
            if (!_playerGroundCheck.isOnGround() && Math.Abs(_playerTransform.position.y - _cameraTransform.position.y) > 10)
                newPositionY = Mathf.SmoothDamp(_cameraTransform.position.y, _playerTransform.position.y,
                    ref _velocity.y, SmoothTimeY);
            else
                newPositionY = Mathf.SmoothDamp(_cameraTransform.position.y, _targetPosition.y, ref _velocity.y,
                    SmoothTimeYGround);

            _cameraTransform.position = new Vector3(newPositionX, newPositionY, ZOffset);
        }
    }
}