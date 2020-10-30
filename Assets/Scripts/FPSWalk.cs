using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class FPSWalk : MonoBehaviour
{

    // this is a First Person movement controller that uses a box collider and a boxcast to interact with map geometry.
    
    //TODO:
    //fix falling into the floor near tall steps - fixed: ensure that tall steps are not a part of the same collider
    

    [Header("Size")]
    [SerializeField] private float height = 2;
    [SerializeField] private float width = 1;
    [SerializeField] private float stepUpDistance = 0.5f;
    private float _cameraHeight;


    [Header("Movement Characteristics")]
    [SerializeField] private float lookSensitivity = 1f;
    [SerializeField] private float cameraStepUpSpeed = 10f;
    [SerializeField] private float walkSpeed = 8f;
    [SerializeField] private float maxAirSpeed = 1f; // the maximum horizontal speed that the player can reach in the air by pressing directions from stationary. should be ~10% of walk speed.
    [SerializeField] private float acceleration = 75;
    [SerializeField] private float skidAccel = 100f;
    [SerializeField] private float airStrafeForce = 5f;
    [SerializeField] private float maxSlope = 40f;
    [SerializeField] private float jumpForce = 5.3f;
    public bool LockLook;
    [SerializeField] private string[] footCollisionLayerMask = new []{"Terrain", "TransparentTerrain", "PlayerClip"};
    

    [Header("Effects")]
    [SerializeField] private TextMeshProUGUI groundedIndicator;
    [SerializeField] private TextMeshProUGUI slidingIndicator;

    // --- Private Variables ---

    // Transforms
    private Transform _playerCamera;
    private Transform _rotator;
    private float _spawnFacing;
    
    // Components
    private Rigidbody _rigidbody;
    private Player _player;

    // Input handling
    private float _cameraX;
    private float _cameraY;
    private bool _jumpLock;
    private bool _airJumpLock;
    private Controls _input;

    // Status
    private bool _grounded;
    private Vector3 _normal;
    private bool _sliding;
    private bool _skidding;

    // Boxcast parameters
    private Vector3 _boxCastHalfExtents;
    private int _layerMask;

    private void Jump()
    {
        Vector3 v = Vector3.ProjectOnPlane(_rigidbody.velocity, _rotator.up);
        _rigidbody.velocity = v;
        
        _rigidbody.AddForce(_rotator.up * jumpForce, ForceMode.VelocityChange);

        _grounded = false;
        _jumpLock = true;
    }

    private void TerrainCollide(RaycastHit hit)
    {
        var pos = _playerCamera.position;
        pos = new Vector3(pos.x, pos.y - (hit.point.y - _rotator.position.y), pos.z);
        _playerCamera.position = pos;

        // move to collision
        transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
        
        if (Vector3.Dot(_rigidbody.velocity.normalized, hit.normal) <= 0)
        {
            _rigidbody.velocity = Vector3.ProjectOnPlane(_rigidbody.velocity, hit.normal);
        }
    }
    
    private void GroundMovement(Vector3 moveVector)
    {
        // skidding behavior
        if (_rigidbody.velocity.magnitude > walkSpeed)
        {
            _skidding = true;
            _rigidbody.velocity = _rigidbody.velocity.normalized * Mathf.Max(walkSpeed, _rigidbody.velocity.magnitude - skidAccel * Time.deltaTime);
        }
        else
        {
            _skidding = false;
        }

        //project the direction to move in onto the surface the player is standing on
        Vector3 targetVelocity = moveVector;
        targetVelocity = Vector3.ProjectOnPlane(targetVelocity, _normal).normalized; //risky
        targetVelocity *= walkSpeed;

        // Apply a force that attempts to reach our target velocity
        Vector3 vc = Vector3.ClampMagnitude(targetVelocity - _rigidbody.velocity, acceleration * Time.deltaTime);
        _rigidbody.AddForce(vc, ForceMode.VelocityChange);
    }


    void AirMovement(Vector3 moveVector3)
    {
        if (_sliding && Vector3.Dot(_rigidbody.velocity.normalized, _normal) >= 0)
        {
            _sliding = false;
            _grounded = false;
        }

        if (_sliding)
        {
            // apply gravity
            _rigidbody.AddForce(Vector3.ProjectOnPlane(Physics.gravity, _normal), ForceMode.Acceleration);
            
            // rotate move vector to align with plane
            moveVector3 = Vector3.ProjectOnPlane(moveVector3, _normal).normalized;
        }
        else
        {
            // apply gravity
            _rigidbody.AddForce(Physics.gravity, ForceMode.Acceleration);
        }

        // air control
        // project the velocity onto the movevector
        Vector3 projVel = Vector3.Project(_rigidbody.velocity, moveVector3);

        // check if the movevector is moving towards or away from the projected velocity
        bool isAway = Vector3.Dot(moveVector3, projVel.normalized) <= 0f;

        // only apply force if moving away from velocity or velocity is below maxStrafeSpeed
        if (projVel.magnitude < maxAirSpeed || isAway)
        {
            // calculate the ideal movement force
            Vector3 vc = moveVector3.normalized * (airStrafeForce * Time.deltaTime);

            // cap it if it would accelerate beyond maxStrafeSpeed directly.
            if (!isAway)
            {
                vc = Vector3.ClampMagnitude(vc, maxAirSpeed - projVel.magnitude);
            }
            else
            {
                vc = Vector3.ClampMagnitude(vc, maxAirSpeed + projVel.magnitude);
            }

            // Apply the force
            _rigidbody.AddForce(vc, ForceMode.VelocityChange);

            if (_sliding)
            {
                _rigidbody.velocity = Vector3.ProjectOnPlane(_rigidbody.velocity, _normal);
            }

            // set airstrafe indicator size and color
            //_airStrafeIndicatorForce = vc.magnitude / strafeForce;
        }
    }

    private void ReleaseJump()
    {
        _jumpLock = false;
        _airJumpLock = false;
    }

    public void Awake()
    {
        _input = new Controls();
        _input.Player.Enable();
        _input.Player.Jump.canceled += ctx => ReleaseJump();
        
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.centerOfMass = Vector3.up * height / 2f;
    }

    // Use this for initialization
    void Start()
    {

        _player = GetComponent<Player>();

        _layerMask = LayerMask.GetMask(footCollisionLayerMask);
        
        // set the rotator and camera
        _rotator = transform.GetChild(0);
        _playerCamera = _rotator.GetChild(0);
        _cameraHeight = _playerCamera.localPosition.y;
        
        // set the facing
        _spawnFacing = transform.rotation.eulerAngles.y;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        _cameraX = _spawnFacing;
        
        // set the camera sensitivity
        lookSensitivity = 0.2f;
        // set the boxcast parameters
        _boxCastHalfExtents = new Vector3(width / 2, width / 2, width / 2);
    }

    // FixedUpdate is called once per physics tick
    void FixedUpdate()
    {
        //check for a collision with ground, and fake a collision if one is detected
        Vector3 center = transform.position + transform.up * (height - width/2);
        Vector3 direction = -transform.up;
        RaycastHit hit;
        bool didHit;
        float distance = height - width;
        if (_grounded)
        {
            distance += stepUpDistance;
        }

        didHit = Physics.BoxCast(center, _boxCastHalfExtents, direction, out hit, transform.rotation, distance, _layerMask);
        _normal = hit.normal;

        // decide if sliding
        if (didHit)
        {
            _sliding = Vector3.Angle(transform.up, hit.normal) > maxSlope;
        }
        else
        {
            _sliding = false;
        }
        
        // read player input
        Vector3 moveVector = new Vector3();
        moveVector += _rotator.forward * _input.Player.Move.ReadValue<Vector2>()[1];
        moveVector += _rotator.right * _input.Player.Move.ReadValue<Vector2>()[0];


        if (didHit)
        {
            TerrainCollide(hit);
        }
        
        if (didHit && _sliding)
        {
            _grounded = true;
            AirMovement(moveVector);            
        }
        else if (didHit && Vector3.Dot(_rigidbody.velocity.normalized, hit.normal) < 0.01f)
        {
            _grounded = true;
            GroundMovement(moveVector);
        }
        else
        {
            _grounded = false;
            _sliding = false;
            AirMovement(moveVector);
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        // update sounds
        //airSound.volume = airSoundVolumeCurve.Evaluate(_rigidbody.velocity.magnitude);
        
        Cursor.lockState = CursorLockMode.Locked;


        if(!LockLook)
        {
            // return the camera to it's resting height after stepping up
            //_playerCamera.position = new Vector3(_playerCamera.position.x, Mathf.Min(_rotator.position.y + _cameraHeight, _playerCamera.position.y + Time.deltaTime * CameraStepUpSpeed), _playerCamera.position.z);
            _playerCamera.localPosition = Vector3.MoveTowards(_playerCamera.localPosition, Vector3.up * _cameraHeight, cameraStepUpSpeed * Time.deltaTime);
            
            // jump when appropriate
            if (_grounded && !_sliding && !_skidding && !_jumpLock && !_airJumpLock && _input.Player.Jump.ReadValue<float>() > 0.5f)
            {
                Jump();
            }

            // rotate the camera
            _cameraX += Mouse.current.delta.x.ReadValue() * lookSensitivity;// * Time.deltaTime;
            if (_cameraX > 180)
            {
                _cameraX -= 360;
            }
            else if (_cameraX < -180)
            {
                _cameraX += 360;
            }
            
            _cameraY += -Mouse.current.delta.y.ReadValue() * lookSensitivity;// * Time.deltaTime;
            _cameraY = Mathf.Clamp(_cameraY, -90, 90);
            
            _rotator.eulerAngles = new Vector2(_rotator.eulerAngles.x, _cameraX);
            _playerCamera.eulerAngles = new Vector2(_cameraY, _playerCamera.eulerAngles.y);
        }
    }

    public Vector3 GetCenter()
    {
        return transform.position + (Vector3.up * height / 2f);
    }

    public Vector3 GetVelocity()
    {
        return _rigidbody.velocity;
    }

    public bool IsGrounded()
    {
        return _grounded;
    }
}