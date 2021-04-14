using System;
using Player;
using UnityEngine;

namespace Behaviours
{
    public class ProjectileTrajectoire : MonoBehaviour
    {
        private PlayerController _playerController;

        private int offset = 40;
        private float _speed;
        private int _orientation;

        private void Awake()
        {
            _playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        }

        private void Start()
        {
            _speed = _playerController.GetProjectileSpeed();
            _orientation = _playerController.GetOrientation();
        }

        void Update()
        {
            transform.Translate(Vector3.right.normalized * (_orientation * (Time.deltaTime * _speed)));
            
            if (Math.Abs(_playerController.transform.position.x - transform.position.x ) > offset)
                Destroy(gameObject);
        }
    }
}