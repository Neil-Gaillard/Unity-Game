using System;
using UnityEngine;

namespace Camera
{
    public class CameraFollower : MonoBehaviour
    {
        // ------Constants ------
        private const float XPosBase = 0.0f;
        private const float YPosBase = 6f;
        private const float ZPosBase = -10.0f;

        private const float SmoothTimeX = 0.3F;
        private const float SmoothTimeYSlow = 1.5F;
        private const float SmoothTimeYMiddle = 1.0F;
        private const float SmoothTimeYFast = 0.6F;
        private const float SmoothTimeYFaster = 0.2f;

        private const int YFollowTolerenceMiddle = 9;
        private const int YFollowTolerenceFast = 11;    
        private const int YFollowTolerenceFaster = 13;

        // ------ Private Attributes ------
        private readonly Vector3 _offset = new Vector3(XPosBase, YPosBase, ZPosBase);

        // ------GameObject References ------
        private Transform _cameraTransform;

        // ------Other References ------
        private Transform _playerTransform;

        private Vector3 _targetPosition = new Vector3(0, 0, ZPosBase);
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

            var newPositionX = Mathf.SmoothDamp(_cameraTransform.position.x, _targetPosition.x, ref _velocity.x,
                SmoothTimeX);
            float newPositionY;

            if (Math.Abs(_playerTransform.position.y - _cameraTransform.position.y) > YFollowTolerenceFaster)
                newPositionY = Mathf.SmoothDamp(_cameraTransform.position.y, _playerTransform.position.y - 5,
                    ref _velocity.y, SmoothTimeYFaster);
            else if (Math.Abs(_playerTransform.position.y - _cameraTransform.position.y) > YFollowTolerenceFast)
                newPositionY = Mathf.SmoothDamp(_cameraTransform.position.y, _playerTransform.position.y,
                    ref _velocity.y, SmoothTimeYFast);
            else if (Math.Abs(_playerTransform.position.y - _cameraTransform.position.y) > YFollowTolerenceMiddle)
                newPositionY = Mathf.SmoothDamp(_cameraTransform.position.y, _playerTransform.position.y,
                    ref _velocity.y, SmoothTimeYMiddle);
            else
                newPositionY = Mathf.SmoothDamp(_cameraTransform.position.y, _targetPosition.y, ref _velocity.y,
                    SmoothTimeYSlow);

            _cameraTransform.position = new Vector3(newPositionX, newPositionY, ZPosBase);
        }
    }
}