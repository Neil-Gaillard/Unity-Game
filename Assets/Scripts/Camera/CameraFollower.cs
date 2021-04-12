using System;
using Player;
using UnityEngine;

namespace Camera
{
    public class CameraFollower : MonoBehaviour
    {
        // ------Constants ------
        private const float XPosBase = 0.0f;
        private const float YPosBase = 5.0f;
        private const float ZPosBase = -10.0f;
        private const float Tolerance = 0.01f;
        
        // ------ References ------
        private Transform _playerTransform;
        private Transform _cameraTransform;
        private GroundCheck _groundCheck;
        
        // ------ Private Attributes ------
        private Vector3 _targetPosition = new Vector3(0, 0, ZPosBase);
        private readonly Vector3 _offset = new Vector3(XPosBase, YPosBase, ZPosBase);

        private FollowMode _followMode = FollowMode.Basic;

        private float _newYPosition = YPosBase;

        private enum FollowMode
        {
            Basic,
            CenterPlayer
        }
        
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
            _newYPosition = _playerTransform.position.y + _offset.y;
        }

        private void Update()
        {
            _followMode = UpdateFollowMode();
        }

        // LateUpdate is called once per frame
        private void LateUpdate()
        {
            _targetPosition = _playerTransform.position + _offset;

            switch (_followMode)
            {
                case FollowMode.Basic:
                    _targetPosition.y = _newYPosition;
                    break;
            }

            //_cameraTransform.position = _targetPosition;
            Vector3.Lerp(_cameraTransform.position, _targetPosition, Time.deltaTime * 6);

        }

        private FollowMode UpdateFollowMode()
        {
            if (Math.Abs(_playerTransform.position.y - _cameraTransform.position.y) < Tolerance && !_groundCheck.isOnGround())
                return FollowMode.CenterPlayer;
            if (_groundCheck.isOnGround())
            {
                _newYPosition = _playerTransform.position.y + _offset.y;
                
            }

            return FollowMode.Basic;
        }
    }
}