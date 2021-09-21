using System.Collections;
using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
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
        private bool _isOnGround;
        
        // --- Key Pressing Check ---
        private bool _jumpKeyHeld;
        
        private float _horizontalInput;

        // --- Initialisation of values from constants ---
        private float _dashSpeed;
        private float _projectileDelay;
        private float _projectileSpeed;
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
            
            //Initializing Attributes
            SetProjectileSpeed(DefaultProjectileSpeed);
            SetProjectileDelay(DefaultProjectileDelay);
            SetDashSpeed(DefaultDashSpeed);
        }
        
        private void FixedUpdate()
        {
            _isOnGround = _groundCheck.isOnGround();
            
            _jumpCount = _isOnGround switch
            {
                true => 0,
                false when _jumpCount == 0 => 1,
                _ => _jumpCount
            };
            
            ManageDashInput();

            ManageMoveInput();
            Move();
        }

        private void Update()
        { 
            ManageJumpInput();
            ManageAttackInput();
            RespawnCheck();
        }
        
        // ------ INPUT MANAGER ------

        private void ManageMoveInput()
        {
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
        }

        private void ManageJumpInput()
        {
            if (!_jumpKeyHeld && Vector2.Dot(_playerRigidbody2D.velocity, Vector2.up) > 0)
                _playerRigidbody2D.AddForce(new Vector2(0, -200) * DefaultMass);
            
            //If the player is releasing jump button, we put the value to true
            //This test will prevent continuous Jumping
            if (!_canJump && Input.GetKeyUp(KeyCode.Space))
                _canJump = true;

            //If the player presses the jump button, it is indicated to _jumpKeyHeld and he jumps if he is on the ground
            //This lauched the Initial Jump from the ground
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _jumpKeyHeld = true;
                if (_isOnGround)
                    Jump(); 
            }

            //Double jump check
            if (_canDoubleJump && !_isOnGround && _canJump && _jumpCount < 2 && Input.GetKeyDown(KeyCode.Space))
                Jump();
            
            //Checks for jump button release
            if (Input.GetKeyUp(KeyCode.Space))
                _jumpKeyHeld = false;
        }

        private void ManageDashInput()
        {
            if (Input.GetKey(KeyCode.A) && _canDash)
                Dash();
        }

        private void ManageAttackInput()
        {
            if (_canLaunchProjectiles && Input.GetKey(KeyCode.Z))
            {
                _attackSpawnManager.SpawnProjectile();
                StartCoroutine(AttackDelay());
            }
        }

        // ------ COLLISIONS DETECTION METHODS ------

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
                _canLaunchProjectiles = true;
                Destroy(other.gameObject);
            }

            if (other.gameObject.CompareTag("PointDash"))
            {
                _canDash = true;
                Destroy(other.gameObject);
            }
        }

        // ------ GETTERS ------
        
        public float GetProjectileSpeed()
        {
            return _projectileSpeed;
        }

        public int GetOrientation()
        {
            return (int) _orientation;
        }

        // ------ SETTERS ------

        private void SetPlayerVelocity(float x, float y)
        {
            _playerRigidbody2D.velocity = new Vector2(x, y);
        }

        private void SetDashSpeed(float speed)
        {
            _dashSpeed = speed;
        }

        private void SetProjectileSpeed(float speed)
        {
            _projectileSpeed = speed;
        }
        
        private void SetProjectileDelay(float delay)
        {
            _projectileDelay = delay;
        }
        
        // ------ PLAYER ACTIONS ------

        private void Move()
        {
            SetPlayerVelocity(_speed * _horizontalInput, _playerRigidbody2D.velocity.y);
        }

        private float CalculateJumpForce()
        {
            return Mathf.Sqrt(2 * Physics2D.gravity.magnitude * DefaultJumpForce);
        }

        private void Jump()
        {
            SetPlayerVelocity(_playerRigidbody2D.velocity.x, 0);
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

        private void RespawnCheck(){
            if (transform.position.y < -40)
            {
                transform.localPosition = _defaultPosition;
                SetPlayerVelocity(0, 0);
            }
        }
        
        // ------ COROUTINES ------

        private IEnumerator AttackDelay()
        {
            _canLaunchProjectiles = false;
            yield return new WaitForSeconds(_projectileDelay);
            _canLaunchProjectiles = true;
        }

        //TODO: Encapsuler l'Orientation
        private enum Orientation
        {
            Left = -1,
            Right = 1
        }
    }
}