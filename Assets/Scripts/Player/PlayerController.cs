using System.Collections;
using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        // ------Constants ------
        private const float DefaultMass = 50.0f;
        private const float GravityModifier = 10.0f; //fall speed
        private const float GravityScale = 1.0f; //gravity influence

        private const float MaxSpeed = 50.0f;
        private const float MaxFallSpeed = 35.0f;
        
        private const float DefaultProjectileSpeed = 30.0f;
        private const float DefaultProjectileDelay = 0.5f;
        private const float DefaultSpeed = 15.0f;
        private const float DefaultJumpForce = 12000.0f;
        private const float DefaultDashSpeed = 5.0f;
        
        // ------ References ------
        private Rigidbody2D _playerRigidbody2D;
        private SpriteRenderer _playerSpriteRenderer;
        private GroundCheck _groundCheck;
        private AttackSpawnManager _attackSpawnManager;
        
        // ------ Private Attributes ------
        private readonly Vector3 _defaultPosition = new Vector3(0, 3, 0);
        
        private float _speed = DefaultSpeed;
        private float _jumpForce = DefaultJumpForce;
        private float _projectileSpeed = DefaultProjectileSpeed;
        private float _projectileDelay = DefaultProjectileDelay;
        private float _dashSpeed = DefaultDashSpeed;

        private float _horizontalInput;
        
        private bool _canAttack = true;
        private bool _canDash;
        private bool _canJump = true; //In order to prevent jumping with continous pressing
        private bool _canDoubleJump;
        
        private bool _isOnGround; //Tells if the player is on the ground or not

        [Range(0, 2)] private int _jumpCount;

        private Orientation _orientation = Orientation.Left;

        private enum Orientation 
        {
            Left = -1,
            Right = 1
        };

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

            switch (_jumpCount)
            {
                case 0:
                    if (_isOnGround && _canJump && Input.GetKey(KeyCode.Space))
                        Jump();
                    break;
                case 1:
                    if (_canDoubleJump && _canJump && Input.GetKey(KeyCode.Space))
                        Jump();
                    break;
            }

            if (Input.GetKey(KeyCode.A) && _canDash)
            {
                Dash();
            }

            _horizontalInput = Input.GetAxis("Horizontal");

            if (_horizontalInput < 0) {
                _orientation = Orientation.Left;
                _playerSpriteRenderer.flipX = true;
            }
            else if (_horizontalInput > 0)
            {
                _orientation = Orientation.Right;
                _playerSpriteRenderer.flipX = false;
            }

            Move();

            if (_playerRigidbody2D.velocity.y > MaxFallSpeed)
                SetPlayerVelocity(_playerRigidbody2D.velocity.x, MaxFallSpeed);
        }

        // Update is called once per frame
        private void Update()
        {
            if (!_canJump && Input.GetKeyUp(KeyCode.Space))
            {
                _canJump = true;
            }
            
            if (Input.GetKey(KeyCode.Z) && _canAttack)
            {
                _attackSpawnManager.SpawnProjectile();
                StartCoroutine(AttackDelay());
            }

            if (transform.position.y < -20)
            {
                transform.localPosition = _defaultPosition;
                SetPlayerVelocity(0, 0);
            }
        }

        public float GetProjectileSpeed()
        {
            return _projectileSpeed;
        }

        public int GetOrientation()
        {
            return (int) _orientation;
        }
        
        // ------- Basics Movements Methods -------
        
        /**
         * Moves the player in the wanted direction by the user
         */
        private void Move()
        {
            SetPlayerVelocity(_speed * _horizontalInput, _playerRigidbody2D.velocity.y);
        }

        /**
         * Makes the player jump in the air
         */
        private void Jump()
        {
            SetPlayerVelocity(_playerRigidbody2D.velocity.x, 0);
            _playerRigidbody2D.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
            _isOnGround = false;
            _canJump = false;
            ++_jumpCount;
        }
        
        private void Dash()
        {
            var velocity = _playerRigidbody2D.velocity;
            Vector3 movdir = new Vector2(velocity.x,velocity.y).normalized;
            transform.position += movdir * _dashSpeed;
        }

        private void SetPlayerVelocity(float x, float y)
        {
            _playerRigidbody2D.velocity = new Vector2(x, y);
        }

        private void SetProjectileSpeed(float speed)
        {
            _projectileSpeed = speed;
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

        // ------- Coroutines Methods -------
        
        private IEnumerator AttackDelay()
        {
            _canAttack = false;
            yield return new WaitForSeconds(_projectileDelay);
            _canAttack = true;
        }
    }
}