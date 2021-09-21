using System;
using UnityEngine;

namespace Player
{
    public class ReWritePlayerController : MonoBehaviour
    {
        // ------ CONSTANTS ------

        // --- Default Physics Values ---
        private const float DefaultMass = 60.0f;
        private const float GravityModifier = 3.0f; //fall speed
        private const float GravityScale = 2.5f; //gravity influence

        // --- Default Position Values ---
        private const float DefaultPositionX = 0.0f;
        private const float DefaultPositionY = 0.0f;
        private const float DefaultPositionZ = 0.0f;

        // --- Default Characteristics ---
        private const float DefaultSpeed = 15.0f;

        private const float DefaultJumpForce = 20.0f;

        private const float DefaultDashSpeed = 5.0f;

        private const float DefaultProjectileSpeed = 30.0f;
        private const float DefaultProjectileDelay = 0.5f;
        
        // ------ PRIVATE ATTRIBUTES ------

        private readonly Vector3 _defaultPosition = new Vector3(DefaultPositionX, DefaultPositionY, DefaultPositionZ);

        // --- Player abilities ---
        private bool _canJump; //(can press the button to jump)
        private bool _canLaunchProjectiles;
        private bool _canDash;
        private bool _canDoubleJump;
        
        // --- Player state ---
        private bool _isJumping;
        private bool _isOnGround;
        
        // --- Key Pressing Check ---
        private bool _jumpKeyHeld;

        private float _horizontalInput;

        // --- Initialisation of values from constants ---
        private float _dashSpeed = DefaultDashSpeed;
        private float _projectileDelay = DefaultProjectileDelay;
        private float _projectileSpeed = DefaultProjectileSpeed;
        private readonly float _speed = DefaultSpeed;

        [Range(0, 2)] private int _jumpCount;
        
        private Orientation _orientation = Orientation.Left;

        // ------ PRIVATE REFERENCES ------
        
        // --- Own References ---
        private Rigidbody2D _playerRigidbody2D;
        private SpriteRenderer _playerSpriteRenderer;
        
        // --- Other References ---
        private AttackSpawnManager _attackSpawnManager;
        private GroundCheck _groundCheck;

        // ------ EVENT METHODS ------

        private void Awake()
        {
            //Initializing References
            _playerRigidbody2D = GetComponent<Rigidbody2D>();
            _playerSpriteRenderer = GetComponent<SpriteRenderer>();
            
            _attackSpawnManager = GameObject.Find("LaserSpawnManager").GetComponent<AttackSpawnManager>();
            _groundCheck = GameObject.Find("Ground Check").GetComponent<GroundCheck>();
        }

        private void Start()
        {
            //Initializing Physics Properties
            Physics2D.gravity *= GravityModifier;
            _playerRigidbody2D.gravityScale = GravityScale;
            _playerRigidbody2D.mass = DefaultMass;
        }
        
        private void FixedUpdate()
        {
            throw new NotImplementedException();
        }

        private void Update()
        {
            throw new NotImplementedException();
        }
        
        // ------ COLLISIONS DETECTION METHODS ------

        private void OnCollisionEnter2D(Collision2D other)
        {
            throw new NotImplementedException();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            throw new NotImplementedException();
        }
        
        // ------ GETTERS ------

        // ------ SETTERS ------

        private void SetPlayerVelocity(float x, float y)
        {
            _playerRigidbody2D.velocity = new Vector2(x, y);
        }
        
        // ------ PLAYER ACTIONS ------

        private void Move()
        {
            
        }

        private void CalculateJumpForce()
        {
            
        }

        private void Jump()
        {
            
        }

        private void Dash()
        {
            
        }
        
        // ------ COROUTINES ------

        //TODO: Encapsuler l'Orientation
        private enum Orientation
        {
            Left = -1,
            Right = 1
        }
    }
}