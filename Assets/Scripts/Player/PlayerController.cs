using System.Collections;
using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        // ------ Constants ------
        private const float DefaultMass = 60.0f;
        private const float GravityModifier = 3f; //fall speed
        private const float GravityScale = 2.5f; //gravity influence


        private const float DefaultPositionX = 0.0f;
        private const float DefaultPositionY = 0.0f;
        private const float DefaultPositionZ = 0.0f;

        private const float DefaultSpeed = 15.0f;

        private const float DefaultProjectileSpeed = 30.0f;
        private const float DefaultProjectileDelay = 0.5f;

        private const float DefaultJumpForce = 20;

        private const float DefaultDashSpeed = 5.0f;

        // ------ Private Attributes ------
        private readonly Vector3 _defaultPosition = new Vector3(DefaultPositionX, DefaultPositionY, DefaultPositionZ);
        
        private AttackSpawnManager _attackSpawnManager;
        
        private bool _canAttack;
        private bool _canDash;
        private bool _canDoubleJump = true;

        private bool _canJump = true; //In order to prevent jumping with continous pressing

        private readonly float _dashSpeed = DefaultDashSpeed;
        private GroundCheck _groundCheck;

        private float _horizontalInput;

        private bool _isJumping;
        private bool _isOnGround; //Tells if the player is on the ground or not

        [Range(0, 2)] private int _jumpCount;

        private bool _jumpKeyHeld;

        private Orientation _orientation = Orientation.Left;

        // ------ Player References ------
        private Rigidbody2D _playerRigidbody2D;
        private SpriteRenderer _playerSpriteRenderer;
        private readonly float _projectileDelay = DefaultProjectileDelay;

        private float _projectileSpeed = DefaultProjectileSpeed;

        private readonly float _speed = DefaultSpeed;

        // ------ Event Methods ------

        private void Awake()
        {
            //Initializing References
            _playerRigidbody2D = GetComponent<Rigidbody2D>();
            _playerSpriteRenderer = GetComponent<SpriteRenderer>();
            _groundCheck = GameObject.Find("Ground Check").GetComponent<GroundCheck>();
            _attackSpawnManager = GameObject.Find("LaserSpawnManager").GetComponent<AttackSpawnManager>();
        }

        // Start is called before the first frame update
        private void Start()
        {
            //Initializing Physics Properties
            Physics2D.gravity *= GravityModifier;
            _playerRigidbody2D.gravityScale = GravityScale;
            _playerRigidbody2D.mass = DefaultMass;

            SetProjectileSpeed(DefaultProjectileSpeed);
        }

        // Update is called once per frame
        private void Update()
        {
            if (!_canJump && Input.GetKeyUp(KeyCode.Space))
                _canJump = true;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                _jumpKeyHeld = true;
                if (_isOnGround)
                    Jump();
            }

            if (_canDoubleJump && !_isOnGround && _canJump && _jumpCount < 2 && Input.GetKeyDown(KeyCode.Space))
                Jump();

            else if (Input.GetKeyUp(KeyCode.Space))
                _jumpKeyHeld = false;

            if (Input.GetKey(KeyCode.Z) && _canAttack)
            {
                _attackSpawnManager.SpawnProjectile();
                StartCoroutine(AttackDelay());
            }

            if (transform.position.y < -40)
            {
                transform.localPosition = _defaultPosition;
                SetPlayerVelocity(0, 0);
            }
        }

        // FixedUpdate is called once per frame
        private void FixedUpdate()
        {
            _isOnGround = _groundCheck.isOnGround();

            _jumpCount = _isOnGround switch
            {
                true => 0,
                false when _jumpCount == 0 => 1,
                _ => _jumpCount
            };

            if (_isJumping)
                if (!_jumpKeyHeld && Vector2.Dot(_playerRigidbody2D.velocity, Vector2.up) > 0)
                    _playerRigidbody2D.AddForce(new Vector2(0, -150) * DefaultMass);

            if (Input.GetKey(KeyCode.A) && _canDash)
                Dash();

            _horizontalInput = Input.GetAxis("Horizontal");

            if (_horizontalInput < 0)
            {
                _orientation = Orientation.Left;
                _playerSpriteRenderer.flipX = true;
            }
            else if (_horizontalInput > 0)
            {
                _orientation = Orientation.Right;
                _playerSpriteRenderer.flipX = false;
            }

            Move();
        }

        // ------- Collisions and Triggers Methods -------
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Ground"))
                SetPlayerVelocity(_playerRigidbody2D.velocity.x, 0);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Token"))
                Destroy(other.gameObject);
            if (other.gameObject.CompareTag("PointDouble"))
            {
                _canDoubleJump = true;
                Destroy(other.gameObject);
            }

            if (other.gameObject.CompareTag("PointProjectile"))
            {
                _canAttack = true;
                Destroy(other.gameObject);
            }

            if (other.gameObject.CompareTag("PointDash"))
            {
                _canDash = true;
                Destroy(other.gameObject);
            }
        }

        // ------- GETTERS -------

        public float GetProjectileSpeed()
        {
            return _projectileSpeed;
        }

        public int GetOrientation()
        {
            return (int) _orientation;
        }

        // ------- SETTERS -------

        private void SetPlayerVelocity(float x, float y)
        {
            _playerRigidbody2D.velocity = new Vector2(x, y);
        }

        private void SetProjectileSpeed(float speed)
        {
            _projectileSpeed = speed;
        }

        // ------- Basics Movements Methods -------
        
        private void Move()
        {
            SetPlayerVelocity(_speed * _horizontalInput, _playerRigidbody2D.velocity.y);
        }
        
        private void Jump()
        {
            SetPlayerVelocity(_playerRigidbody2D.velocity.x, 0);
            _isJumping = true;
            _playerRigidbody2D.AddForce(Vector2.up * (CalculateJumpForce() * _playerRigidbody2D.mass),
                ForceMode2D.Impulse);
            _isOnGround = false;
            _canJump = false;
            ++_jumpCount;
        }

        private void Dash()
        {
            var velocity = _playerRigidbody2D.velocity;
            Vector3 movdir = new Vector2(velocity.x, velocity.y).normalized;
            transform.position += movdir * _dashSpeed;
        }

        private float CalculateJumpForce()
        {
            return Mathf.Sqrt(2 * Physics2D.gravity.magnitude * DefaultJumpForce);
        }

        // ------- Coroutines Methods -------

        private IEnumerator AttackDelay()
        {
            _canAttack = false;
            yield return new WaitForSeconds(_projectileDelay);
            _canAttack = true;
        }

        private enum Orientation
        {
            Left = -1,
            Right = 1
        }
    }
}