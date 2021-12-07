using System;
using Player;
using UnityEngine;

namespace Behaviours
{
    public class GoingForwardFromParentObject : MonoBehaviour
    {
        // ------- Constants -------
        private const int DestructionOffset = 50;
        private int _orientation;

        // ------- References -------
        private PlayerController _playerController;
        private Transform _transform;
        private Orientation.Orientation _orrientation;

        // ------- Private Attributes -------
        private float _speed;

        private void Awake()
        {
            _transform = GetComponentInParent(typeof(Transform)) as Transform;
        }

        private void Start()
        {
            _speed = _playerController.GetProjectileSpeed();
            _orientation = _playerController.GetOrientation();
        }

        private void Update()
        {
            transform.Translate(Vector3.right.normalized * (_orientation * (Time.deltaTime * _speed)));
            if (Math.Abs(_transform.position.x - transform.position.x) > DestructionOffset)
                Destroy(gameObject);
        }
    }
}