using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Player
{
    public class PlayerController : MonoBehaviour, Input.InputPlayer.IPlayerActions
    {
        // --- Default Physics Values ---
        private const float DefaultMass = 70.0f;
        private const float GravityScale = 4.0f;

        // --- Default Characteristics ---
        private const float DefaultSpeed = 550.5f;
        private const float DefaultDashSpeed = 2000.0f;
        private const float DefaultDashDelay = 0.3f;

        private const float DefaultJumpForce = 2000.0f;

        private const float DefaultDashTime = 0.2f;

        private const float DefaultProjectileSpeed = 30.0f;
        private const float DefaultProjectileDelay = 0.5f;

        private const float DefaultRespawnLimit = -50.0f;

        // --- Player Attributes ---
        private Input.InputPlayer _controls;

        private Orientation.Orientation _orientation;

        // --- Player abilities ---
        private bool _canJump;
        private bool _canLaunchProjectiles;
        private bool _canDash;
        private bool _canDoubleJump;

        // --- Player State ---
        private bool _isOnGround;
        private bool _isDashing;
        private bool _dashDelay;

        private float _projectileDelay;
        private float _projectileSpeed;

        // --- Input State ---
        private bool _moveKeyPressed;
        private bool _jumpKeyPressed;
        private bool _dashKeyPressed;

        private float _xAxisValue;

        [Range(0, 2)] private int _jumpCount;

        // --- Own References ---
        private Rigidbody2D _playerRigidbody2D;
        private SpriteRenderer _playerSpriteRenderer;

        // --- Other References ---
        private AttackSpawnManager _attackSpawnManager;

        // ------ EVENT METHODS ------

        private void Awake()
        {
            _playerRigidbody2D = GetComponent<Rigidbody2D>();
            _playerSpriteRenderer = GetComponent<SpriteRenderer>();
            _attackSpawnManager = GameObject.Find("LaserSpawnManager").GetComponent<AttackSpawnManager>();
        }

        private void Start()
        {
            _playerRigidbody2D.gravityScale = GravityScale;
            _playerRigidbody2D.mass = DefaultMass;
            _projectileSpeed = DefaultProjectileSpeed;
            _projectileDelay = DefaultProjectileDelay;

            _orientation = gameObject.GetComponent<SpriteRenderer>().flipX switch
            {
                true => Orientation.Orientation.Left,
                false => Orientation.Orientation.Right
            };

            _canJump = true;
            _canDoubleJump = true;
            _canLaunchProjectiles = true;
            _canDash = true;
        }

        private void OnEnable()
        {
            if (_controls == null)
            {
                _controls = new Input.InputPlayer();
                _controls.Player.SetCallbacks(this);
            }

            _controls.Player.Enable();
        }

        private void OnDisable()
        {
            _controls.Disable();
        }

        private void FixedUpdate()
        {
            if (_moveKeyPressed && !_isDashing)
                Move();
            else if (!_isDashing)
                _playerRigidbody2D.velocity = new Vector2(0.0f, this._playerRigidbody2D.velocity.y);

            if (_canJump && _jumpKeyPressed && _isOnGround)
                Jump();
            if (_jumpKeyPressed && _canDoubleJump && !_isOnGround && _canJump && _jumpCount < 2)
                Jump();

            if (_canDash && _dashKeyPressed && !_isDashing && !_dashDelay)
                Dash();
        }

        private void Update()
        {
            _jumpCount = _isOnGround switch
            {
                true => 0,
                false when _jumpCount == 0 => 1,
                _ => _jumpCount
            };

            RespawnCheck();
        }

        // ------ COLLISIONS DETECTION METHODS ------

        private void OnCollisionStay2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Wall"))
                _playerRigidbody2D.velocity = new Vector2(0, _playerRigidbody2D.velocity.y);
            else if (other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("Rock"))
                _isOnGround = true;
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("Rock"))
                _isOnGround = false;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Token"))
                Destroy(other.gameObject);
        }

        // ------ PUBLIC GETTERS ------

        public float GetProjectileSpeed()
        {
            return _projectileSpeed;
        }

        public Orientation.Orientation GetOrientation()
        {
            return _orientation;
        }

        // ------ PLAYER ACTIONS ------

        private void Move()
        {
            if (_isDashing) return;

            switch (_xAxisValue)
            {
                case < 0:
                    _orientation = Orientation.Orientation.Left;
                    _playerSpriteRenderer.flipX = true;
                    break;
                case > 0:
                    _orientation = Orientation.Orientation.Right;
                    _playerSpriteRenderer.flipX = false;
                    break;
            }

            _playerRigidbody2D.velocity = new Vector2(DefaultSpeed * _xAxisValue * Time.fixedDeltaTime,
                _playerRigidbody2D.velocity.y);
        }

        private void Jump()
        {
            _playerRigidbody2D.velocity = new Vector2(_playerRigidbody2D.velocity.x, 0);
            _playerRigidbody2D.AddForce(Vector2.up.normalized * DefaultJumpForce, ForceMode2D.Impulse);
            _canJump = false;
            ++_jumpCount;
        }

        private void Dash()
        {
            StartCoroutine(DashTime());
            _playerRigidbody2D.constraints = RigidbodyConstraints2D.FreezePositionY |
                                             RigidbodyConstraints2D.FreezeRotation;
            _playerRigidbody2D.velocity = new Vector2((int)_orientation * DefaultDashSpeed * Time.fixedDeltaTime, 0.0f);
        }

        private void LaunchProjectile()
        {
            _attackSpawnManager.SpawnProjectile();
            StartCoroutine(AttackDelay());
        }

        private void RespawnCheck()
        {
            if (!(transform.position.y < DefaultRespawnLimit)) return;
            Scene thisScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(thisScene.name);
        }

        // ------ COROUTINES ------

        private IEnumerator AttackDelay()
        {
            _canLaunchProjectiles = false;
            yield return new WaitForSeconds(_projectileDelay);
            _canLaunchProjectiles = true;
        }

        private IEnumerator DashTime()
        {
            _canDash = false;
            _isDashing = true;
            yield return new WaitForSeconds(DefaultDashTime);
            _isDashing = false;
            _playerRigidbody2D.constraints &= RigidbodyConstraints2D.FreezePositionX |
                                              ~RigidbodyConstraints2D.FreezePositionY |
                                              RigidbodyConstraints2D.FreezeRotation;
            yield return DashDelay();
        }

        private IEnumerator DashDelay()
        {
            _dashDelay = true;
            yield return new WaitForSeconds(DefaultDashDelay);
            _dashDelay = false;
        }

        // ------ INPUT HANDLING ----

        public void OnJump(InputAction.CallbackContext context)
        {
            switch (context.canceled)
            {
                case true:
                    _jumpKeyPressed = false;
                    _canJump = true;
                    break;
                case false:
                    _jumpKeyPressed = true;
                    break;
            }
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            switch (context.canceled)
            {
                case true:
                    _moveKeyPressed = false;
                    break;
                case false:
                    _moveKeyPressed = true;
                    _xAxisValue = context.ReadValue<float>();
                    break;
            }
        }

        public void OnDash(InputAction.CallbackContext context)
        {
            switch (context.canceled)
            {
                case true:
                    _dashKeyPressed = false;
                    _canDash = true;
                    break;
                case false:
                    if (!_dashDelay)
                        _dashKeyPressed = true;
                    break;
            }
        }

        public void OnShoot(InputAction.CallbackContext context)
        {
            if (_canLaunchProjectiles)
                LaunchProjectile();
        }
    }
}