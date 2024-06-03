using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Foward speed"), SerializeField] private float forwardAcceleration;
    [SerializeField] private float maxFowardVelocity;
    [SerializeField] private float rbVelocity;

    [Header("Backwards speed"), SerializeField] private float backwardAcceleration;
    [SerializeField] private float maxBackwardVelocity;

    [Header("Strafe speed"), SerializeField] private float strafeAcceleration;
    [SerializeField] private float maxStrafeVelocity;

    [Header("Fall/Jump speed"), SerializeField] private float gravityAcceleration;
    [SerializeField] private float jumpAcceleration;
    [SerializeField] private float maxJumpHeight;
    [SerializeField] private float maxFallVelocity;
    [SerializeField] private float extraGravity;
    private float   forceToGoDown;
    private float   heightOfJump;
    private Vector3 defaultGravity;



    [Header("Mouse speed"), SerializeField] private float rotationVelocityFactor;

    [Header("Rotation speed"), SerializeField] private float rotationSpeedPlayer;
    private Vector3 targetTransform;
    private Vector3 targetRotation;

    [Header("Stamina"), SerializeField] private int maxStamina;
    [SerializeField] private int staminaRegenRate;

    [Header("Sprint"), SerializeField] private int sprintStaminaCost;
    [SerializeField] private int sprintVelocityFactor;


    [Header("UI"), SerializeField] private UIManager UIManager;

    [Header("Camera"), SerializeField] private Transform cameraTransform;

    [SerializeField] private Grappling grappling;
    private bool enemyGrab;

    private Animator animator;
    public Animator Animator { get { return animator; } }

    private ControlCamera cameraController;

    private Vector3 acceleration;
    private Vector3 velocity;
    private Vector3 motion;

    private CheckGrounded grounded;
    public bool Grounded => grounded.Grounded;
    private Rigidbody rb;

    private float extraAngleForDirection;
    private float compensationAngleForCamera;

    private bool jump;

    private float targetAngle;
    private float currentAngle;

    private float stamina;
    private bool staminaRegenOn;


    private float dashTimer;

    private bool sprint;
    private bool needSprintRest;

    private bool moving;

    private float sin90;

    private bool freeze;
    private bool activeGrapple;
    public bool ActiveGrapple { get { return activeGrapple; } }

    private PlayerSounds playerSounds;

    // Start is called before the first frame update
    private void Start()
    {
        animator = GetComponentInChildren<Animator>();

        acceleration = Vector3.zero;
        velocity = Vector3.zero;
        motion = Vector3.zero;

        grounded = GetComponentInChildren<CheckGrounded>();
        rb = GetComponent<Rigidbody>();
        defaultGravity = Physics.gravity;
        forceToGoDown = 0;

        extraAngleForDirection = 0;
        compensationAngleForCamera = 0;

        cameraController = GetComponentInChildren<ControlCamera>();

        jump = false;

        targetAngle = transform.rotation.eulerAngles.y;

        targetTransform = Vector3.zero;
        targetRotation = Vector3.zero;

        sprint = false;
        needSprintRest = false;

        dashTimer = 0f;

        moving = false;

        enemyGrab = false;

        stamina = maxStamina;

        sin90 = Mathf.Sin(Mathf.PI / 4);

        freeze = false;
        activeGrapple = false;

        playerSounds = GetComponent<PlayerSounds>();
    }

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        if (!freeze && !activeGrapple)
        {
            UpdateAcceleration();
            UpdateVelocity();
            UpdateMotion();
        }
        else
        {
            rb.velocity = Vector3.zero;
            animator.SetFloat("MovSpeed", 0f);
        }

        
    }

    // Update is called once per frame
    private void Update()
    {


        if (!freeze && !activeGrapple)
        {
            UpdateRotation();

            CheckForJump();
            CheckForSprint();
            CheckForSprintRest();
        }

        animator.SetBool("Grounded", grounded.Grounded);
        animator.SetBool("GrappleActive", activeGrapple);
        animator.SetBool("EnemyGrab", enemyGrab);
        animator.SetBool("Sprinting", sprint);


        currentAngle = GetCurrentAngleBetweenCameraAndPlayer();
    }

    private void UpdateRotation()
    {



        if (Input.GetKey(KeyCode.W))
        {
            if (Input.GetKey(KeyCode.A))
            {
                compensationAngleForCamera = -45;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                compensationAngleForCamera = 45;
            }
            else
            {
                extraAngleForDirection = 0;
                compensationAngleForCamera = 0;
            }

            UpdateCamera(extraAngleForDirection, compensationAngleForCamera);

        }
        else if (Input.GetKey(KeyCode.S))
        {
            if (Input.GetKey(KeyCode.A))
            {
                extraAngleForDirection = -180;
                compensationAngleForCamera = 45;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                extraAngleForDirection = -180;
                compensationAngleForCamera = -45;
            }
            else
            {
                extraAngleForDirection = -180;
                compensationAngleForCamera = 0;
            }

            UpdateCamera(extraAngleForDirection, compensationAngleForCamera);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            extraAngleForDirection = 0;
            compensationAngleForCamera = -90;

            UpdateCamera(extraAngleForDirection, compensationAngleForCamera);
        }
        else if (Input.GetKey(KeyCode.D))
        {

            extraAngleForDirection = 0;
            compensationAngleForCamera = 90;

            UpdateCamera(extraAngleForDirection, compensationAngleForCamera);
        }

    }

    public void UpdateCamera(float extraAngleForDirection, float compensationAngleForCamera)
    {
        targetAngle = cameraTransform.rotation.eulerAngles.y - extraAngleForDirection + compensationAngleForCamera;
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, targetAngle, transform.eulerAngles.z);

        cameraTransform.eulerAngles = new Vector3(cameraTransform.eulerAngles.x, targetAngle - extraAngleForDirection - compensationAngleForCamera, cameraTransform.eulerAngles.z);

    }

    private float GetCurrentAngleBetweenCameraAndPlayer()
    {
        float angle = cameraTransform.eulerAngles.y - transform.eulerAngles.y;

        return (angle > 0 ? angle : 360 + angle);

    }

    private void CheckForJump()
    {
        if (Input.GetButtonDown("Jump") && grounded.Grounded)
        {
            playerSounds.PlayJumpSound();
            jump = true;
            Debug.Log("jump");
            animator.SetTrigger("Jump");
            
            rb.velocity = Vector2.zero;
        }
    }

    private void CheckForSprint()
    {
        sprint = Input.GetButton("Sprint")
                && grounded.Grounded
                && stamina > 0f
                && !needSprintRest;
    }

    private void CheckForSprintRest()
    {
        if (needSprintRest && !Input.GetButton("Sprint"))
        {
            needSprintRest = false;
        }
    }

    private void UpdateAcceleration()
    {
        UpdateXZAcceleration();
    }

    private void UpdateXZAcceleration()
    {


        float forwardAxis = Input.GetAxis("Forward");
        float strafeAxis = Input.GetAxis("Strafe");


        // if the player moves in any direction we give acceleration in the direction we're facing, the direction is determined  by the UpdateRotation method
        if (forwardAxis != 0 || strafeAxis != 0) acceleration.z = forwardAcceleration;
        else if (forwardAxis == 0) acceleration.z = 0;

        animator.SetFloat("MovSpeed", acceleration.z);

    }

    private void UpdateVelocity()
    {
        velocity += acceleration * Time.fixedDeltaTime;

        UpdateForwardVelocity();
        UpdateStrafeVelocity();
        UpdateVerticalVelocity();

        if (cameraController.TargetObject != null)
        {
            if (!(cameraController.Targetting &&
                Vector3.Distance(new Vector3(transform.position.x, 0f, transform.position.z),
                                new Vector3(cameraController.TargetObject.transform.position.x, 0f, cameraController.TargetObject.transform.position.z))
                                < 4))
            {
                UpdateSprint();
            }
        }
        else
            UpdateSprint();

    }

    private void UpdateSprint()
    {
        if (sprint && moving && dashTimer <= 0f)
        {
            velocity.z *= sprintVelocityFactor;
            velocity.x *= sprintVelocityFactor;

            if (stamina <= sprintStaminaCost * Time.fixedDeltaTime)
            {
                needSprintRest = true;
            }

        }
    }


    private void UpdateForwardVelocity()
    {
        if (velocity.z * acceleration.z < 0 || acceleration.z == 0)
        {
            velocity.z = 0;
        }
        else if (acceleration.x == 0f)
        {
            velocity.z = Mathf.Clamp(velocity.z, maxBackwardVelocity, maxFowardVelocity);
        }
        else
        {
            velocity.z = Mathf.Clamp(velocity.z, maxBackwardVelocity * sin90, maxFowardVelocity * sin90);
        }
    }

    private void UpdateStrafeVelocity()
    {
        if (velocity.x * acceleration.x < 0 || acceleration.x == 0)
        {
            velocity.x = 0;
        }
        else if (acceleration.z == 0f)
        {
            velocity.x = Mathf.Clamp(velocity.x, -maxStrafeVelocity, maxStrafeVelocity);
        }
        else
        {
            velocity.x = Mathf.Clamp(velocity.x, -maxStrafeVelocity * sin90, maxStrafeVelocity * sin90);
        }
    }

    private void UpdateVerticalVelocity()
    {

        if (grounded)
        {
            Physics.gravity = defaultGravity;

        }

        if (jump && grounded.Grounded)
        {
            rb.AddForce(transform.up * jumpAcceleration, ForceMode.Impulse);
            heightOfJump = transform.position.y;
            jump = false;
            grounded.Grounded = false;
        }

        if (transform.position.y > heightOfJump + maxJumpHeight || rb.velocity.y < 0)
        {
            Physics.gravity = -new Vector3(0f, extraGravity, 0f);
        }

        rb.AddForce(forceToGoDown * Vector3.down, ForceMode.Force);
    }

    public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
    {

        activeGrapple = true;

        rb.velocity = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);

        animator.SetTrigger("Jump");
    }

    public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = Physics.gravity.y;
        float displacementY = endPoint.y - startPoint.y;

        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);

        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity)
            + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

        return (endPoint - startPoint) * 4;
    }

    private void UpdateMotion()
    {
        motion = velocity * Time.fixedDeltaTime;

        motion = transform.TransformVector(motion);
        motion *= rbVelocity;

        if (rb.velocity.y < 0 && rb.velocity.y > maxFallVelocity && !grappling)
            rb.velocity = new Vector3(motion.x, rb.velocity.y * gravityAcceleration, motion.z);
        else
            rb.velocity = new Vector3(motion.x, rb.velocity.y, motion.z);

        moving = motion.z != 0f || motion.x != 0f;
    }

    public void EnableGrapple()
    {
        extraAngleForDirection = 0;
        compensationAngleForCamera = 0;

        UpdateCamera(0, 0);

        animator.SetTrigger("Grapple");

        activeGrapple = true;
    }

    public void StopMoving()
    {
        velocity = Vector3.zero;
        acceleration = Vector3.zero;

        rb.velocity = Vector3.zero;

        motion = Vector3.zero;
        
        animator.SetFloat("MovSpeed", 0f);
    }

    public void DisableGrapple() => activeGrapple = false;

    public void EnableFreeze() => freeze = true;

    public void DisableFreeze() => freeze = false;

    public void EnableEnemyGrab() => enemyGrab = true;

    public void DisableEnemyGrab() => enemyGrab = false;

    public void SetGrounded(bool grounded) => this.grounded.Grounded = grounded;


    private void OnCollisionEnter(Collision collision)
    {
        activeGrapple = false;
    }

    public void SetVelocity(float speed)
    {
        maxFowardVelocity = speed;
        maxBackwardVelocity = -speed;
        maxStrafeVelocity = speed;
    }

    [System.Serializable]
    public struct SaveData
    {
        public Vector3    position;
        public Quaternion rotation;
        public Vector3    velocity;
        public bool       grounded;
    }

    public SaveData GetSaveData()
    {
        SaveData saveData;

        saveData.position = transform.position;
        saveData.rotation = transform.rotation;
        saveData.velocity = velocity;
        saveData.grounded = grounded.Grounded;

        return saveData;
    }

    public void LoadSaveData(SaveData saveData)
    {
        transform.position = saveData.position;
        transform.rotation = saveData.rotation;
        velocity = saveData.velocity;
        grounded.Grounded = saveData.grounded;
    }
}
