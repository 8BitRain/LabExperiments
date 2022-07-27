using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine.Events;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{

    [Header("Character Settings")]
    public CharacterController _controller;
    public Animator animator;
    protected AnimatorOverrideController animatorOverrideController;
    private AnimationController animationController;

    //TODO: Move to Camera Controller
    //public Transform cam;
    private string currentState;

    [Header("Movement Settings")]
    public float speed = 6.0f;
    public float topSpeed = 10.0f;
    public float defaultSpeed;
    public float acceleration = .1f;
    public float friction = .025f;
    public float gravity = -9.81f;
    public bool applyGravity = true;
    public bool moveCharacter;
    public Vector2 movementInput;
    public float turnSmoothTime = 0.1f;
    public float turnSmoothVelocity;
    private bool dashInput = false;
    //private Vector2 movementDirection;
    private Vector3 movementDirection;
    public MovementComponent movementComponent;


    [Header("Dash Settings")]
    public float dashSpeed = 6.0f;
    private float defaultDashSpeed;
    public float maxSpeed = 10.0f;
    public float dashFriction = .025f;
    public Transform glideAfterImage;
    private float _afterImageTimer = 0;
    private float _dashTimer = 0;
    private bool glide = false;

    [Header("Jump Settings")]
    public Transform _groundChecker;
    public float GroundDistance = 0.2f;
    public LayerMask Ground;
    public float JumpHeight = 2f;
    private Vector3 _velocity;
    public bool _isGrounded = true;
    private bool jumpInput = false;

    [Header("WallRunning Settings")]
    public Transform _wallRunChecker;
    public float wallDistance = 0.2f;
    public LayerMask Wall;
    public float WallRunMaxHeight = 1f;
    bool isWallRight;
    bool isWallLeft;
    bool isWallInFront;
    private Vector3 wallVector;
    private Vector3 wallJumpDirection;
    private bool _canWallRun = false;
    private bool _isWallRunning = false;

    //Button to trigger enviromental action
    [Header("Enviromental Action")]
    private bool _attachedToPlatform = false;
    private bool enviromentActionInput = false;

    [Header("VFX")]
    public GameObject speedLines;
    private GameObject speedLinesInstance;
    public GameObject dustTrails;
    public GameObject _jumpVFX;

    //Input settings
    private bool canMove = true;
    private bool canUseRightSideFaceButtonInputs = true;
    private bool canSteer = false;
    private bool canSteerAndMove = false;
    private bool canPlayerInputMove = true;
    private bool _playerInMovementFrames = false;
    private bool useGravityLockPlayerInput = false;

    private void OnEnable()
    {
        EventsManager.instance.AbilityWindowActiveLockInput += LockInputRightSideControllerFaceButtons;
        EventsManager.instance.AbilityWindowInactiveUnlockInput += UnlockInputRightSideControllerFaceButtons;
        EventsManager.instance.Parried += OnParry;
    }

    private void OnDisable()
    {
        EventsManager.instance.AbilityWindowActiveLockInput -= LockInputRightSideControllerFaceButtons;
        EventsManager.instance.AbilityWindowInactiveUnlockInput -= UnlockInputRightSideControllerFaceButtons;
        EventsManager.instance.Parried -= OnParry;
    }

    private void OnDestroy()
    {
        EventsManager.instance.AbilityWindowActiveLockInput -= LockInputRightSideControllerFaceButtons;
        EventsManager.instance.AbilityWindowInactiveUnlockInput -= UnlockInputRightSideControllerFaceButtons;
        EventsManager.instance.Parried -= OnParry;
    }

    void Start()
    {
        animator = this.GetComponent<Animator>();
        animationController = this.GetComponent<AnimationController>();
        gravity = gravity/2.0f;
        //defaultSpeed = speed;
    }

    void Update()
    {
        UpdateMovementComponent();

        RaycastHit groundedRaycast;
        _isGrounded = Physics.Raycast(_groundChecker.position, Vector3.down, out groundedRaycast, GroundDistance, Ground);
        if(_isGrounded)
        {
            
        }

        Debug.DrawRay(_groundChecker.position, Vector3.down * GroundDistance, Color.red);

        if(canSteer)
        {
            Vector3 direction = new Vector3(0,0,0);
            if(canPlayerInputMove)
            {
                //direction = new Vector3(movementInput.x, 0, movementInput.y).normalized;
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + GetComponent<CameraController>().GetCameraInstance().eulerAngles.y;
                transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
            }

        }

        // We want the melee attack to still incorporate player movement
        if(canSteerAndMove)
        {
            Vector3 direction = new Vector3(movementInput.x, 0, movementInput.y).normalized;
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + GetComponent<CameraController>().GetCameraInstance().eulerAngles.y;
            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
            movementDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            _controller.Move(movementDirection.normalized * speed * Time.deltaTime);
        }

        if(canMove)
        {
            Vector3 direction = new Vector3(0,0,0);
            if(canPlayerInputMove)
            {
                direction = new Vector3(movementInput.x, 0, movementInput.y).normalized;
            }

            if (_isGrounded && _velocity.y < 0)
            {
                //print("grounded");
                _velocity.y = 0f;
                //TODO: Ensure the jumping animation does not play
                animator.SetBool("Jumping", false);
                _canWallRun = false;

                //No longer falling
                if(animator.GetBool("Falling")){
                    print("Not falling");
                    //TODO: Ensure Falling animation does not play
                    animator.SetBool("Falling", false);
                    //TODO: Ensure a landing animation plays
                    animator.SetBool("Landing", true);
                    StartCoroutine(LandTimer(1f));
                    //Roll land if moving, standard land if not moving
                    if(movementInput.x != 0 || movementInput.y != 0)
                        animationController.ChangeAnimationState(animator, "Player_landing_roll");
                    else
                        animationController.ChangeAnimationState(animator, "Player_landing");
                    //TODO: Figure out if we can get the correct frames
                    //Invoke("Land", animator.GetCurrentAnimatorClipInfo(0)[0].clip.length);
                    //TODO: Ensure Sprinting animation does not play
                }

                if(!_isWallRunning)
                {
                    if(enviromentActionInput)
                    {
                        speed = 20;
                        //TODO: Trigger Camera Controller Here
                        //freeLookCamera.GetComponent<CinemachineFreeLook>().m_Lens.FieldOfView = 80;
                        speedLines.SetActive(true);

                        //TODO in con
                        //if(_isGrounded && !animator.GetBool("PerformingSprintingLightAttack"))
                        if(_isGrounded)
                        {
                            //TODO: Ensure sprint animation is played here
                            //ChangeAnimationState("Player_sprint");
                        }
                    }
                    else
                    {
                        //speed = defaultSpeed;

                        //increase speed;
                        //TODO: Trigger Camera Controller Here
                        //freeLookCamera.GetComponent<CinemachineFreeLook>().m_Lens.FieldOfView = 40;
                        //speedLines.SetActive(false);
                        //TODO: Ensure we are not sprinting
                        //animator.SetBool("Sprinting", false);
                        
                        //Adding witch time animation state check to prevent unwnanted speed lines during witch time
                        if(speed > 30 && !animator.GetBool("WitchTime"))
                        {
                           if(speedLinesInstance == null)
                           {
                               TriggerSpeedLinesVFX(true);
                           }
                           else
                           {
                                UpdateSpeedLinesVFXEmission();
                           }
                        }
                        else
                        {
                            TriggerSpeedLinesVFX(false);
                        }


                    }
                }

                //Dust Trails VFX
                TriggerDustTrails(direction);

            }

            if(!_isGrounded)
            {
                _canWallRun = true;
            }

            WallRunController(direction);

            if(direction.magnitude >= .1f && moveCharacter)
            {
                //Acceleration
                if(speed < topSpeed)
                {
                    Accelerate();
                }

                //Atan2 returns angle between x axis and the angle between 0
                //Gives us an angle in radians
                //Add the rotation of the camera on the y axis on to the camera
                /*===== ThirdPersonCamera_GamePad Rotation*/
                
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + GetComponent<CameraController>().GetCameraInstance().eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                //Vector3 moveDir;
                //Standard Movement
                if(!_isWallRunning)
                {
                    transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);

                    //Move Forward as normal
                    movementDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

                    //Moving forward no tilt
                    if(movementInput.x == 0 && movementInput.y > 0)
                    {
                        transform.Rotate(new Vector3(0,0,1), 0f);
                        transform.Rotate(new Vector3(1,0,0), 0f);
                    }

                    RaycastHit slopeHit;
                    Vector3 slopeNormal = Vector3.zero;
                    if(Physics.Raycast(_groundChecker.position, Vector3.down, out slopeHit, GroundDistance, Ground))
                    {
                        Slide(slopeNormal, slopeHit, movementDirection);
                    } 
                    
                    if(!glide && !_isGrounded)
                    {
                        _controller.Move(movementDirection.normalized * speed * Time.deltaTime);
                    } 
                }
                else
                {
                    // The step size is equal to speed times frame time.
                    float singleStep = speed * Time.deltaTime;

                    // Rotate the forward vector towards the target direction by one step
                    Vector3 turnDirection = Vector3.RotateTowards(transform.forward, wallVector, singleStep, 0.0f);
                    // Draw a ray pointing at our target in
                    Debug.DrawRay(transform.position, turnDirection, Color.red);

                    // Calculate a rotation a step closer to the target and applies rotation to this object
                    transform.rotation = Quaternion.LookRotation(turnDirection);

                    
                    movementDirection = wallVector;

                    //Look up parabolic motion. There seem to be animation cuves, bezier curves, and other lines to use.
                    //I'm curious what mathmatical functions simulate parbolas. How do you achieve x^2?

                    _velocity.y = 0;

                    _controller.Move(wallVector * speed * Time.deltaTime); 
                }          
            }

            if(direction.magnitude <= 0)
            {
                if(speed > defaultSpeed)
                {
                    Decelerate();
                }
            }

            if (jumpInput && canUseRightSideFaceButtonInputs){
                if(_isGrounded && !_isWallRunning){
                    
                    print("jump");
                    animationController.ChangeAnimationState(animator,"Player_jump");
                    //reset y_velocity to prevent super bouncing
                    _velocity.y = 0; 
                    print("Player yVelocity before jump: " + _velocity.y);
                    _velocity.y += Mathf.Sqrt(JumpHeight * -2f * gravity);
                    print("Player yVelocity before jump: " + _velocity.y);
                    animator.SetBool("Jumping", true);
                    //play jump vfx
                    GameObject jumpVFXClone;
                    jumpVFXClone = Instantiate(_jumpVFX, transform.position, transform.rotation);
                    Destroy(jumpVFXClone, 2);
                    
                }
                if(_isWallRunning && !_isGrounded){
                    print("WALLJUMP");
                    animator.SetBool("Jumping", true);
                    animationController.ChangeAnimationState(animator,"Player_jump");
                    isWallLeft = false;
                    isWallRight = false;
                    isWallInFront = false;
                    ExitWallRun();
                    //reset y_velocity on wall
                    _velocity.y = 0;
                    print("yVelocity before jump: " + _velocity.y);
                    _velocity.y += Mathf.Sqrt(JumpHeight * -2f * gravity);
                    print("yVelocity after jump: " + _velocity.y);
                    //Jump off wall at 45 degree angle
                    Vector3 jumpDirection;
                    //jump direction uses transform.TransformDirection to move player in a vector 45degrees away from wall
                    jumpDirection = transform.TransformDirection(wallJumpDirection);
                    _controller.Move(jumpDirection * speed * Time.deltaTime); 
                }
                //TODO: Better Jumping Arc //Getting a better jumping arc will probably be factored here
                //_velocity.y += Mathf.Sqrt(JumpHeight * -2f * gravity);
            }
           

            if(movementInput.x != 0 || movementInput.y != 0)
            {
                if(!_isWallRunning && !animator.GetBool("Jumping") && !animator.GetBool("Falling") && !animator.GetBool("Landing") && !animator.GetBool("Attacking"))
                {
                    animationController.ChangeAnimationState(animator,"Player_run");
                }
            } 
            else 
            {
                if(!_isWallRunning && _isGrounded && !animator.GetBool("Landing") && !animator.GetBool("Attacking") && !animator.GetBool("Gaurding") && !animator.GetBool("Dodging") && !animator.GetBool("Damaged"))
                {
                    //TODO Ensure run animation does not play
                    animationController.ChangeAnimationState(animator,"Player_idle");
                }
            }
            
            //Gravity
            Gravity();
            /*if(_velocity.y < 0)
            {
                //animator.SetBool("Falling", true);
                animator.SetBool("Jumping", false);
                animator.SetBool("Falling", true);
                animationController.ChangeAnimationState(animator,"Player_jump_falling");
            }
            if(applyGravity)
            {
                _velocity.y += gravity * Time.deltaTime;
                //Getting a better jumping arc will probably be factored here
                _controller.Move(_velocity * Time.deltaTime);
            }*/
        }

        if(useGravityLockPlayerInput)
        {
            Gravity();
        }
    }

    /* INPUTS */
    public void OnMove(InputAction.CallbackContext ctx) => movementInput = ctx.ReadValue<Vector2>();
    public void OnJump(InputAction.CallbackContext ctx) => jumpInput = ctx.ReadValueAsButton();
    public void OnEnviromentInteraction(InputAction.CallbackContext ctx) => enviromentActionInput = ctx.ReadValueAsButton();
    public void OnDash(InputAction.CallbackContext ctx) => dashInput = ctx.ReadValueAsButton();

    public void EnableMovement()
    {
        canMove = true;
    }

    public void DisableMovement()
    {
        canMove = false; 
    }

    public void EnableApplyGravityLockPlayerInput()
    {
        useGravityLockPlayerInput = true;
    }

    public void DisableApplyGravityLockPlayerInput()
    {
        useGravityLockPlayerInput = false;
    }

    public void EnableSteering(){canSteer = true;}
    public void DisableSteering(){canSteer = false;}
    public void EnableSteeringAndMovement(){canSteerAndMove = true;}
    public void DisableSteeringAndMovement(){canSteerAndMove = false;}

    void Slide(Vector3 slopeNormal, RaycastHit slopeHit, Vector3 moveDir)
    {
        /*Debug.Log("Player touched the ground & is sliding down slope");*/
        slopeNormal = slopeHit.normal;
        Quaternion slopeOffset = Quaternion.FromToRotation(Vector3.up, slopeNormal);
        /*Debug.Log("Player Transform forward:" + transform.forward);
        Debug.Log("Slope Normal:" + slopeHit.normal);*/

        //Ensure the slide animation plays when the player is moving down a slope and not up a slope
        if(transform.forward.z > 0 && slopeOffset.x > 0)
        {
            animator.Play("Grounded.slide");
        }

        //Ensure the slide animation plays when the player is moving down a slope and not up a slope
        if(transform.forward.z < 0 && slopeOffset.x < 0)
        {
            animator.Play("Grounded.slide");
        }

        /*Debug.Log("Slope quaternion: " + slopeOffset);*/
        //Multiply slope offset by move Direction. You can multiply a quaternion x a vector. Not a vector x a quaternion
        _controller.Move(slopeOffset * moveDir.normalized * speed * Time.deltaTime);
        Debug.DrawRay(transform.position, Vector3.down, Color.red);
    }

    /** IN AIR LOGIC/JUMPING LOGIC**/
    void Land()
    {
        animator.SetBool("Landing", false);
    }

    void Gravity()
    {
        if(animator.GetBool("Damaged"))
        {
            return;
        }

        if(_velocity.y < 0)
        {
            animator.SetBool("Jumping", false);
            if(!animator.GetBool("Damaged") && !animator.GetBool("Attacking") && !animator.GetBool("Gaurding") && !animator.GetBool("Dodging") && !animator.GetBool("Parried") && !animator.GetBool("WallRunning"))
            {
                animationController.ChangeAnimationState(animator,"Player_jump_falling");
                animator.SetBool("Falling", true);
            }
        }
        if(applyGravity)
        {
            _velocity.y += gravity * Time.deltaTime;
            //Getting a better jumping arc will probably be factored here
            _controller.Move(_velocity * Time.deltaTime);
        }
    }

    public void Accelerate()
    {
        if(animator.GetBool("WitchTime"))
        {
            return;
        }

        if(dashInput)
        {
            speed += acceleration * Time.deltaTime;
        }
    }

    public void Decelerate()
    {
        if(animator.GetBool("WitchTime"))
        {
            return;
        }

        if(speed > defaultSpeed)
        {
            //animationController.ChangeAnimationState(animator, "Esc_Slide_Left");
            speed -= friction * Time.deltaTime;
            _controller.Move(movementDirection.normalized * speed * Time.deltaTime);
            if(speed < defaultSpeed)
            {
                speed = defaultSpeed;
            }
        }
    }

    /** WALL RUN LOGIC **/
    void WallRunController(Vector3 direction)
    {
        //WALLRUNNING 2.0 using Raycasts
        if(_canWallRun){
            CheckForWall();
        }
        if(isWallRight && enviromentActionInput)
        {
            StartWallRun("right", direction);
        }
        if(isWallLeft && enviromentActionInput)
        {
            StartWallRun("left", direction);
        }
        if(isWallInFront && enviromentActionInput)
        {
            StartWallRun("front", direction);
        }
        if(!isWallLeft && !isWallRight && !isWallInFront || !enviromentActionInput)
        {
            ExitWallRun();
        }
    }

    void CheckForWall()
    {
        RaycastHit hit;
        isWallRight = Physics.Raycast(_wallRunChecker.transform.position, _wallRunChecker.right, out hit, 1.0f, Wall | Ground);
        if(isWallRight){
            Debug.DrawRay(_wallRunChecker.transform.position, _wallRunChecker.right.normalized * hit.distance, Color.magenta );
            wallVector = -Vector3.Cross(hit.normal, Vector3.up).normalized;
            wallJumpDirection = new Vector3(-1,0,1).normalized;
        }

       
        isWallLeft = Physics.Raycast(_wallRunChecker.transform.position, -_wallRunChecker.right, out hit, 1.0f, Wall | Ground);
        if(isWallLeft){
            Debug.DrawRay(_wallRunChecker.transform.position, -_wallRunChecker.right.normalized * hit.distance, Color.green );
            wallVector = Vector3.Cross(hit.normal, Vector3.up).normalized;
            wallJumpDirection = new Vector3(1,0,1).normalized;
        }
       
        Debug.DrawRay(_wallRunChecker.transform.position, wallVector, Color.yellow);

        //Debug ray describing Vector at 45 degree from wall
        Debug.DrawRay(_wallRunChecker.transform.position, transform.TransformDirection(wallJumpDirection), Color.cyan);
    }

    void StartWallRun(string direction, Vector3 directionVector)
    {
        animator.SetBool("WallRunning", true);
        animator.SetBool("Falling", false);
        animator.SetBool("Jumping", false);
        //animator.SetBool("Sprinting", false);

        //TriggerDustTrails(directionVector);

        if(direction == "right")
        { 
            Debug.Log("StartWallRun: " + "Wall is to the Right.");
            animationController.ChangeAnimationState(animator, "WallRunRight");
            //TODO: Replace with more generic name
            //animatorOverrideController["eston_rig|WallRunLeft"] = wallRunningAnimationClip[0];
        }

        if(direction == "left")
        {
            Debug.Log("StartWallRun: " + "Wall is to the Left.");
            animationController.ChangeAnimationState(animator, "WallRunLeft");
            //TODO: Replace with more generic name
            //animatorOverrideController["eston_rig|WallRunLeft"] = wallRunningAnimationClip[1];
        }

        if(direction == "front")
        {
            Debug.Log("StartWallRun: " + "Wall is in Front.");
            //TODO: Replace with more generic name
            //animatorOverrideController["eston_rig|WallRunLeft"] = wallRunningAnimationClip[2];
        }

        print("wallrunning");
        _isWallRunning = true;
    }

    void ExitWallRun()
    {
        animator.SetBool("WallRunning", false);
        _isWallRunning = false;
    }

    /** VFX **/
    void TriggerDustTrails(Vector3 directionVector)
    {
        if(dustTrails != null && directionVector.magnitude != 0) 
        {
            dustTrails.SetActive(true);
        } else if(directionVector.magnitude == 0)
        {
            dustTrails.SetActive(false);
        }
    }

    //Lock, x, circle, square, and triangle button inputs. Based on AbilityWindow active 
    public void LockInputRightSideControllerFaceButtons(GameObject instance)
    {
        if(this.gameObject != instance)
            return;
        canUseRightSideFaceButtonInputs = false;
    }

    //Unlock c, circle, square, and triangle button inputs. Based on AbiltyWindow inactive
    public void UnlockInputRightSideControllerFaceButtons(GameObject instance)
    {
        if(this.gameObject != instance)
            return;
        canUseRightSideFaceButtonInputs = true;
    }

    public void TriggerSpeedLinesVFX(bool isActive)
    {


        if(isActive)
        {
            if(speedLinesInstance == null)
            {
                speedLines.SetActive(true);
                speedLinesInstance = Instantiate(speedLines, transform.position, transform.rotation);

                //Fade speedLines In
                Debug.Log(speedLinesInstance.GetComponent<Renderer>().material.shaderKeywords);
                
                //Set parent of speedLinesInstance to camera instance
                speedLinesInstance.transform.SetParent(GetComponent<CameraController>().GetCameraInstance());
            }
        }
        else
        {
            if(speedLinesInstance != null)
            {
                //Fade speedLines Out, then remove them.
                speedLinesInstance.SetActive(false);

                //define some value for fade time
                Destroy(speedLinesInstance, 1f);
                
            }
        }
    }

    //Increase number of speedLine particles as player speed increases
    public void UpdateSpeedLinesVFXEmission()
    {
        if(animator.GetBool("WitchTime"))
        {
            return;
        }

        if(speedLinesInstance != null)
        {
            if(speed < 30)
            {
                Destroy(speedLinesInstance);
            }
            //Take current speed (speed)
            //Take top speed (topSpeed)
            // Take the speed value that acts as the point where speed lines take effect (30)
            
            //For instance if the speed necessary to display speedlines is 30, then 45 - 30 gives us a range
            float range = topSpeed - 30;

            //Now we can subtract the speedLine range from the top speed subsctracted by the currentSpeed 
            float currentSpeedLineValue = range - (topSpeed - speed);

            //Define a speed rate
            float speedRate = currentSpeedLineValue/range;
            
            ParticleSystem speedLinePS = speedLinesInstance.GetComponent<ParticleSystem>();
            var emissionModule = speedLinePS.emission; 
            emissionModule.rateOverTime = Mathf.Lerp(10, 100, speedRate); 
        
        }
    }

    public IEnumerator LandTimer(float time)
    {
        yield return new WaitForSeconds(time);
        Land();
    } 

    public void UpdateMovementComponent()
    {
        movementComponent.currentSpeed = speed;
        movementComponent.gravity = gravity;
        movementComponent.isGrounded = _isGrounded;
    }

    public void OnParry(GameObject instanceA, GameObject instanceB)
    {
        if(this.gameObject != instanceA && this.gameObject != instanceB)
            return;
        
        DisableMovement();
        animator.SetBool("Attacking", false);
        animator.SetBool("Parried", true);
        //Debug.Break();
        animationController.ChangeAnimationState(animator, "Parried");
        TriggerDustTrails(movementDirection);
        transform.DOMove(transform.position - transform.forward*10, 1f).OnComplete( () => {
            EnableMovement();
            animator.SetBool("Parried", false);
        });
    }
}
