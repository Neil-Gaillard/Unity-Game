using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Player
{
    public class PlayerController : MonoBehaviour, Input.InputPlayer.IPlayerActions
    {
        // ------ CONSTANTS ------

        // --- Default Physics Values ---
        private const float DefaultMass = 80.0f;
        private const float GravityScale = 4.0f;

        // --- Default Characteristics ---
        private const float DefaultSpeed = 550.5f;

        private const float DefaultJumpForce = 17.0f;
        private const float DefaultCounterJumpForce = 100.0f;

        private const float DefaultDashDelay = 0.3f;
        private const float DefaultDashTime = 0.15f;

        private const float DefaultProjectileSpeed = 30.0f;
        private const float DefaultProjectileDelay = 0.5f;

        private const float DefaultRespawnLimit = -50.0f;

        // ------ PRIVATE ATTRIBUTES ------
        private Input.InputPlayer _controls;

        private Orientation.Orientation _orientation = Orientation.Orientation.Left;

        // --- Player abilities ---
        private bool _canJump;
        private bool _canLaunchProjectiles;
        private bool _canDash;
        private bool _canDoubleJump;

        private bool _isOnGround;
        private bool _isDashing;

        private bool _moveKeyPressed;
        private bool _jumpKeyPressed;

        private float _xAxisValue;
        private float _projectileDelay;
        private float _projectileSpeed;

        [Range(0, 2)] private int _jumpCount;

        // ------ PRIVATE REFERENCES ------

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
            //CounterJumpForce();

            _jumpCount = _isOnGround switch
            {
                true => 0,
                false when _jumpCount == 0 => 1,
                _ => _jumpCount
            };

            if (_moveKeyPressed)
                Move();
            else
                this._playerRigidbody2D.velocity = new Vector2(0.0f, this._playerRigidbody2D.velocity.y);

            if (_jumpKeyPressed && _isOnGround)
                Jump();

            if (!_canJump)
                _canJump = true;

            if (_jumpKeyPressed && _canDoubleJump && !_isOnGround && _canJump && _jumpCount < 2)
                Jump();
        }

        private void Update()
        {
            RespawnCheck();
        }

        // ------ COLLISIONS DETECTION METHODS ------

        private void OnCollisionStay2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Wall"))
                _playerRigidbody2D.velocity = new Vector2(0, _playerRigidbody2D.velocity.y);
            else if (other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("Rock"))
                this._isOnGround = true;
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("Rock"))
                this._isOnGround = false;
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

        private static float CalculateJumpForce()
        {
            return Mathf.Sqrt(2 * Physics2D.gravity.magnitude * DefaultJumpForce);
        }

        private void CounterJumpForce()
        {
            if (Vector2.Dot(_playerRigidbody2D.velocity, Vector2.up) > 0)
                _playerRigidbody2D.AddForce(new Vector2(0, -DefaultCounterJumpForce) * _playerRigidbody2D.mass);
        }

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
            //_playerRigidbody2D.MovePosition(gameObject.transform.position + new Vector3(_xAxisValue * DefaultSpeed * Time.deltaTime, 0, 0));

            /*var position = gameObject.transform.position;
            position = Vector3.Lerp(position, position + new Vector3(_xAxisValue, 0, 0), Time.deltaTime * 12);
            gameObject.transform.position = position;*/
        }

        private void Jump()
        {
            _playerRigidbody2D.velocity = new Vector2(_playerRigidbody2D.velocity.x, 0);
            _playerRigidbody2D.AddForce(Vector2.up.normalized * (CalculateJumpForce() * _playerRigidbody2D.mass),
                ForceMode2D.Impulse);
            _canJump = false;
            ++_jumpCount;
        }

        private void Dash()
        {
            StartCoroutine(DashTime());
            StartCoroutine(DashDelay());
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
            _isDashing = true;
            yield return new WaitForSeconds(DefaultDashTime);
            _isDashing = false;
        }

        private IEnumerator DashDelay()
        {
            _canDash = false;
            yield return new WaitForSeconds(DefaultDashDelay);
            _canDash = true;
        }

        // ------ INPUT HANDLING ----

        public void OnJump(InputAction.CallbackContext context)
        {
            switch (context.canceled)
            {
                case true:
                    this._jumpKeyPressed = false;
                    break;
                case false:
                    this._jumpKeyPressed = true;
                    break;
            }
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            switch (context.canceled)
            {
                case true:
                    this._moveKeyPressed = false;
                    break;
                case false:
                    this._moveKeyPressed = true;
                    this._xAxisValue = context.ReadValue<float>();
                    break;
            }
        }

        public void OnDash(InputAction.CallbackContext context)
        {
            if (_canDash)
                Dash();
        }

        public void OnShoot(InputAction.CallbackContext context)
        {
            if (_canLaunchProjectiles)
                LaunchProjectile();
        }
    }
}