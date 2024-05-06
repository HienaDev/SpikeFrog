using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent (typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{

    [Header("Foward speed"), SerializeField]    private float forwardAcceleration;
    [SerializeField]                            private float maxFowardVelocity;
    [SerializeField]                            private float rbVelocity;

    [Header("Backwards speed"), SerializeField] private float backwardAcceleration;
    [SerializeField]                            private float maxBackwardVelocity;

    [Header("Strafe speed"), SerializeField]    private float strafeAcceleration;
    [SerializeField]                            private float maxStrafeVelocity;

    [Header("Fall/Jump speed"), SerializeField] private float gravityAcceleration;
    [SerializeField]                            private float jumpAcceleration;
    [SerializeField]                            private float maxJumpHeight;
    [SerializeField]                            private float maxFallVelocity;
    [SerializeField]                            private float extraGravity;
    private float forceToGoDown;
    private float heightOfJump;
    private Vector3 defaultGravity;
    


    [Header("Mouse speed"), SerializeField]     private float rotationVelocityFactor;

    [Header("Rotation speed"), SerializeField]  private float rotationSpeedPlayer;
    private float lerpPercentage;
    private Vector3 targetTransform;
    private Vector3 targetRotation;

    [Header("Stamina"), SerializeField]         private int maxStamina;
    [SerializeField]                            private int staminaRegenRate;

    [Header("Sprint"), SerializeField]          private int sprintStaminaCost;
    [SerializeField]                            private int sprintVelocityFactor;

    [Header("Dash"), SerializeField]            private int dashStaminaCost;
    [SerializeField]                            private int dashVelocity;
    [SerializeField]                            private float dashDuration;



    [Header("Skate"), SerializeField]           private GameObject skate;
    [SerializeField]                            private float skateRotateSpeed;
    private bool skateRotating;
    private float previousRotation;

    [Header("UI"), SerializeField]              private UIManager UIManager;

    [Header("Camera"), SerializeField]          private Transform cameraTransform;

    [SerializeField] private Grappling grappling;

    private Animator animator;
    public Animator Animator {  get { return animator; } }

    //private CharacterController characterController;

    private Vector3 acceleration;
    private Vector3 velocity;
    private Vector3 motion;

    private CheckGrounded grounded;
    private Rigidbody rb;

    private float extraAngleForDirection;
    private float compensationAngleForCamera;
    //private float currentAngleForDirection;
    //private float currentCompensationAngleForCamera;

    private bool    jump;

    private float targetAngle;
    private float currentAngle;

    private float stamina;
    private bool staminaRegenOn;

    private bool dash;
    private float dashTimer;

    private bool    sprint;
    private bool    needSprintRest;

    private bool moving;

    private float   sin90;

    private bool freeze;
    private bool activeGrapple;
    public bool ActiveGrapple { get {  return activeGrapple; } }

    // Start is called before the first frame update
    private void Start()
    {
        //characterController = GetComponent<CharacterController>();
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
        //currentAngleForDirection = 0;
        //currentCompensationAngleForCamera = 0;


        jump = false;

        targetAngle = transform.rotation.eulerAngles.y;
        lerpPercentage = 0;
        targetTransform = Vector3.zero;
        targetRotation = Vector3.zero;

        sprint = false;
        needSprintRest = false;

        dash = false;
        dashTimer = 0f;

        moving = false;

        stamina = maxStamina;

        skateRotating = false;
        previousRotation = 0f;

        sin90 = Mathf.Sin(Mathf.PI / 4);

        freeze = false;
        activeGrapple = false;

        UpdateUI();
        HideCursor();
    }

    private void FixedUpdate()
    {
        

        if (!freeze && !activeGrapple)
        {
            UpdateAcceleration();
            UpdateVelocity();
            UpdateMotion();

            RegenStamina();

            RotateSkate();
        }
        else if (!activeGrapple)
        {
            rb.velocity = Vector3.zero;
        }

        animator.SetBool("Grounded", grounded.Grounded);

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
            //CheckForDash();
        }
        else if (!activeGrapple)
        {
            rb.velocity = Vector3.zero;
        }

        RotateCamera();


        //if( PLAYER NOT)
        currentAngle = GetCurrentAngleBetweenCameraAndPlayer();
        //Debug.Log(currentAngle);

        //animator.SetBool("Swinging", grappling.IsGrappling());
        
        
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




        if(Input.anyKey)
        { 
            //UpdateCamera(); 
        }
    }

    public void UpdateCamera(float extraAngleForDirection, float compensationAngleForCamera)
    {
        targetAngle = cameraTransform.rotation.eulerAngles.y - extraAngleForDirection + compensationAngleForCamera;
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, targetAngle, transform.eulerAngles.z);

        cameraTransform.eulerAngles = new Vector3(cameraTransform.eulerAngles.x, targetAngle - extraAngleForDirection - compensationAngleForCamera, cameraTransform.eulerAngles.z);


        //targetTransform = new Vector3(transform.eulerAngles.x, targetAngle, transform.eulerAngles.z);
        //targetRotation = new Vector3(cameraTransform.eulerAngles.x, targetAngle - extraAngleForDirection - compensationAngleForCamera, cameraTransform.eulerAngles.z);
        //lerpPercentage = 0;
    }

    // maybe change everything from Vector3 angles to quaternions
    private void RotateCamera()
    {
        //cameraTransform.eulerAngles = new Vector3(cameraTransform.eulerAngles.x, targetAngle - extraAngleForDirection - compensationAngleForCamera, cameraTransform.eulerAngles.z);

        //transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, targetTransform, lerpPercentage);

        //cameraTransform.eulerAngles = Vector3.Lerp(cameraTransform.eulerAngles, targetRotation, lerpPercentage);
        //lerpPercentage += rotationSpeedPlayer * Time.deltaTime;
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
            jump = true;
            skateRotating = true;

            animator.SetTrigger("Jump");
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
        if(needSprintRest && !Input.GetButton("Sprint"))
        {
            needSprintRest = false;
        }
    }

    private void CheckForDash()
    {


        if(Input.GetButtonDown("Dash")
            && stamina > dashStaminaCost)
        {
            dash = true;
        }
    }

    private void RotateSkate()
    {
        if (skateRotating) 
        { 
           skate.transform.Rotate(0f, 0f, skateRotateSpeed);
        }

        if (skate.transform.eulerAngles.z < previousRotation)
        {
            skate.transform.Rotate(0f, 0f, 0f);
            skateRotating = false;
        }

        previousRotation = skate.transform.eulerAngles.z;

    }

    private void RegenStamina()
    {
        if (stamina < maxStamina && (!sprint || !moving))
        {
            AddStamina(staminaRegenRate * Time.fixedDeltaTime);
        }
           
    }


    private void AddStamina(float amount)
    {
        stamina = Mathf.Min(stamina + amount, maxStamina);

        UpdateUI();
    }

    private void DecStamina(float amount)
    {
        stamina = Mathf.Max(stamina - amount, 0f);

        UpdateUI();
    }


    private void UpdateUI()
    {
        //UIManager.SetStaminaFill(stamina / maxStamina);
    }
    

    private void UpdateAcceleration()
    {
        UpdateXZAcceleration();
        //UpdateVerticalAcceleration();
    }

    private void UpdateXZAcceleration()
    {
        

        float forwardAxis = Input.GetAxis("Forward");
        float strafeAxis = Input.GetAxis("Strafe");

        
        // if the player moves in any direction we give acceleration in the direction we're facing, the direction is determined  by the UpdateRotation method
        if (forwardAxis != 0 || strafeAxis != 0) acceleration.z = forwardAcceleration;
        //else if (forwardAxis < 0 && GetCurrentAngleBetweenCameraAndPlayer() > 90 && GetCurrentAngleBetweenCameraAndPlayer() < 270) acceleration.z = forwardAcceleration;
        else if (forwardAxis == 0) acceleration.z = 0;

        animator.SetFloat("MovSpeed", acceleration.z);

    }

    private void UpdateStrafeAcceleration()
    {   
        
    }

    //private void UpdateVerticalAcceleration()
    //{
    //    if (jump)
    //    {
    //        acceleration.y = jumpAcceleration;

    //    }
    //    else
    //    {
    //        acceleration.y = gravityAcceleration;
    //    }
    //}

    private void UpdateVelocity()
    {
        velocity += acceleration * Time.fixedDeltaTime;

        UpdateForwardVelocity();
        UpdateStrafeVelocity();
        UpdateVerticalVelocity();
        UpdateSprint();
        //UpdateDash();
        
    }

    private void UpdateSprint()
    {
        if (sprint && moving && dashTimer <= 0f)
        {
            velocity.z *= sprintVelocityFactor;
            velocity.x *= sprintVelocityFactor;

            DecStamina(sprintStaminaCost * Time.fixedDeltaTime);

            //Debug.Log(stamina);
            //Debug.Log(sprintStaminaCost * Time.fixedDeltaTime);


            if (stamina <= sprintStaminaCost * Time.fixedDeltaTime)
            {
                needSprintRest = true;
            }

        }
    }

    //private void UpdateDash()
    //{
    //    if (dash)
    //    {

    //        dash = false;

    //        velocity.z = dashVelocity;
    //        dashTimer = dashDuration;

    //        DecStamina(dashStaminaCost);
    //    }
    //    else if(dashTimer > 0f)
    //    {
    //        velocity.z = dashVelocity;
    //        dashTimer -= Time.fixedDeltaTime;
    //    }
    //}

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
        }

        if (transform.position.y > heightOfJump + maxJumpHeight || rb.velocity.y < 0)
        {
            //rb.velocity = new Vector3(rb.velocity.x, Mathf.Min(rb.velocity.y, 5f), rb.velocity.z);
            //forceToGoDown = extraGravity;
            Physics.gravity = -new Vector3 (0f, extraGravity, 0f);
        }

       

        


        rb.AddForce(forceToGoDown * Vector3.down, ForceMode.Force);
        //Debug.Log(forceToGoDown);
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

        //return (velocityXZ + velocityY);

        return (endPoint - startPoint) * 4;
    }

    private void UpdateMotion()
    {
        motion = velocity * Time.fixedDeltaTime;

        motion = transform.TransformVector(motion);

        //Debug.Log(motion);

        motion *= rbVelocity;

        if (rb.velocity.y < 0 && rb.velocity.y > maxFallVelocity && !grappling)
            rb.velocity = new Vector3(motion.x, rb.velocity.y * gravityAcceleration, motion.z);
        else
            rb.velocity = new Vector3(motion.x, rb.velocity.y, motion.z);




        //characterController.Move(motion);

        moving = motion.z != 0f || motion.x != 0f;
    }

    public void ActivateGrapple()
    {



        extraAngleForDirection = 0;
        compensationAngleForCamera = 0;

        UpdateCamera(0, 0);

        animator.SetTrigger("Grapple");
    }

    private void HideCursor() => Cursor.lockState = CursorLockMode.Locked;

    public void EnableFreeze() => freeze = true;

    public void DisableFreeze() => freeze = false;  

    private void DeactiveGrapple() => activeGrapple = false;


    private void OnCollisionEnter(Collision collision)
    {
        activeGrapple = false;
        grappling.ResetFov();
    }
}
