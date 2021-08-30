using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;
using Random = UnityEngine.Random;



[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]
public class CyberSpaceFirstPerson : MonoBehaviour
{
    [SerializeField] private bool m_IsWalking;
    [SerializeField] private float m_WalkSpeed;
    [SerializeField] private float m_RunSpeed;
    [SerializeField] [Range(0f, 1f)] private float m_RunstepLenghten;
    [SerializeField] private float m_JumpSpeed;
    [SerializeField] private float m_StickToGroundForce;
    [SerializeField] private float m_GravityMultiplier;
    [SerializeField] private MouseLook m_MouseLook;
    [SerializeField]
    public MouseLook mLook
    {
        get
        {
            return m_MouseLook;
        }
    }
    [SerializeField]
    public float m_MouseLook_x
    {
        get { return m_MouseLook.yAdjust; }
        set { m_MouseLook.yAdjust = value; }
    }
    [SerializeField] private bool m_UseFovKick;
    [SerializeField] private FOVKick m_FovKick = new FOVKick();
    [SerializeField] private bool m_UseHeadBob;
    [SerializeField] private CurveControlledBob m_HeadBob = new CurveControlledBob();
    [SerializeField] private LerpControlledBob m_JumpBob = new LerpControlledBob();
    [SerializeField] private float m_StepInterval;
    [SerializeField] private AudioClip[] m_FootstepSounds;    // an array of footstep sounds that will be randomly selected from.
    [SerializeField] private AudioClip m_JumpSound;           // the sound played when character leaves the ground.
    [SerializeField] private AudioClip m_LandSound;           // the sound played when character touches back on ground.
    [SerializeField] private float m_dashTime;
    [SerializeField] private Vector3 m_DashDrag;
    [SerializeField] private float m_DashSpeed;
    [SerializeField] private AudioClip m_DashSound;

    private Camera m_Camera;
    //Public reference to the camera
    public Camera PlayerCamera
    {
        get { return m_Camera; }
    }
    private bool m_Jump;
    private Vector2 m_Input;
    private Vector3 m_MoveDir = Vector3.zero;
    private CharacterController m_CharacterController;
    private CollisionFlags m_CollisionFlags;
    private bool m_PreviouslyGrounded;
    private Vector3 m_OriginalCameraPosition;
    private float m_StepCycle;
    private float m_NextStep;
    private bool m_Jumping;
    private AudioSource m_AudioSource;
    public Vector3 AddDirVector;
    public float dirVectorSpeed;
    private Vector3 dirVector;
    [SerializeField] private AudioSource m_DashSource;

    [Header("Double Jump")]
    [SerializeField] private float m_doublejump_speed;
    [SerializeField] private bool m_doublejump;
    [SerializeField] private bool can_doublejump;
    [SerializeField] private float doubleJumpDelay;
    [SerializeField] private float doubleJumpTimer;
    private bool m_doublejumping;
    [SerializeField]
    private AudioClip doubleJumpClip;

    [Header("Grappling Hook")]
    [SerializeField] private Vector3 grapplePosition;
    [SerializeField] private bool grappling;
    public bool IsGrappling
    {
        get { return grappling; }
        set { grappling = value; }
    }
    public Vector3 GrapplePosition { set { grapplePosition = value; } }
    [SerializeField] private float grappleSpeed;
    [SerializeField] private float initialGrappleSpeed;
    [SerializeField] private float maxGrappleSpeed;
    [SerializeField] private float grappleAcceleration;
    [SerializeField] private float grappleGravityModifier;
    [SerializeField] private float grappleCutDistance;
    [SerializeField] private float grappleCameraInfluence;
    [SerializeField] private float grappleEndForce;
    private Vector3 inheritedPlayerVelocity;
    private Vector3 grappleAddSpeed;

    //public CameraRollEffects rollEffects;

    [Header("Aditional Physics")]
    [SerializeField] public Vector3 leftOverVelocity;
    [SerializeField] private float drag;
    [SerializeField] private float airDrag;
    [SerializeField] private float universalDecayAmt;
    [SerializeField] private bool waitForCarryVelocityBeforeMove;

    //nat's vars
    [Header("Stuff Nat Added")]

    [Tooltip("The speed of the player in the air")]
    public float airSpeed;
    [Tooltip("The modifier applied to in-air movement. more = more drag")]
    public float groundedDrag;
    /// <summary>
    /// the last movement if the player is grounded
    /// </summary>
    private Vector3 lastMove;
    /// <summary>
    /// same as lastmove if grounded, if in air used to facilitate air calculations.
    /// </summary>
    private Vector3 airMove;


    [Header("Misc")]
    /// <summary>
    /// Tells us whether or not we're travelling on a moving platform
    /// </summary>
    [HideInInspector]
    public bool movingPlatform;
    /// <summary>
    /// Vector3 representing the direction and magnitude of our platform's movement
    /// </summary>
    [HideInInspector]
    public Vector3 platformDirMag;


    [Space(3)]
    [Header("Re-Write")]
    [SerializeField]
    private Vector2 inputVector;
    [SerializeField]
    private Vector3 velocity;
    [SerializeField]
    private float groudAccel;
    [SerializeField]
    private float maxSpeedAD = 8f;
    [SerializeField]
    private float groundAccelCoef = 500.0f;
    [SerializeField]
    private float friction = 15f;
    [SerializeField]
    private float frictionThresh = 0.5f;
    [SerializeField]
    private float jumpStrength;

    [Header("Air Control")]
    [SerializeField]
    private float airAccelCoef = 1f;
    [SerializeField]
    private float airDeccelCoef = 1.5f;
    [SerializeField]
    private float airControlPrecision = 16f;
    [SerializeField]
    private float airControlAdditionForward = 8f;
    [SerializeField]
    private bool grounded;
    [SerializeField]
    private List<Transform> groundedRayPositions;
    [SerializeField]
    private LayerMask excludedLayers;

    /// <summary>
    /// This is a boolean used to stop the RotateView function for one frame so the rotation can be set
    /// </summary>
    [HideInInspector]
    public bool outsideRot;



    // Use this for initialization
    private void Start()
    {
        m_CharacterController = GetComponent<CharacterController>();
        m_Camera = Camera.main;
        m_OriginalCameraPosition = m_Camera.transform.localPosition;
        m_FovKick.Setup(m_Camera);
        m_HeadBob.Setup(m_Camera, m_StepInterval);
        m_StepCycle = 0f;
        m_NextStep = m_StepCycle / 2f;
        m_Jumping = false;
        m_AudioSource = GetComponent<AudioSource>();
        m_MouseLook.Init(transform, m_Camera.transform);
        grapplePosition = Vector3.zero;
        grappling = false;
        grappleSpeed = initialGrappleSpeed;
        grappleAddSpeed = Vector3.zero;
    }


    // Update is called once per frame
    private void Update()
    {

        // the jump state needs to read here to make sure it is not missed
        if (!m_Jump && m_CharacterController.isGrounded)
        {
            m_Jump = Input.GetButtonDown("Jump");

        }

        //Double jump
        if (can_doublejump && !m_doublejump && !m_CharacterController.isGrounded && doubleJumpTimer > doubleJumpDelay)
        {
            m_doublejump = CrossPlatformInputManager.GetButtonDown("Jump");

        }

        if (!m_PreviouslyGrounded && m_CharacterController.isGrounded)
        {
            StartCoroutine(m_JumpBob.DoBobCycle());
            PlayLandingSound();
            m_MoveDir.y = 0f;
            m_Jumping = false;
            doubleJumpTimer = 0;
            can_doublejump = true;
            m_doublejumping = false;
            //rollEffects.vectorAdditions.x += 10f;
        }

        if (!m_CharacterController.isGrounded && !grappling)
        {
            doubleJumpTimer += Time.deltaTime;
        }

        if (!m_CharacterController.isGrounded && !m_Jumping && !m_doublejumping && m_PreviouslyGrounded && !grappling)
        {
            m_MoveDir.y = 0f;
        }

        m_PreviouslyGrounded = m_CharacterController.isGrounded;
    }

    private void LateUpdate()
    {

        RotateView();
    }

    private void PlayLandingSound()
    {
        m_AudioSource.clip = m_LandSound;
        m_AudioSource.Play();
        m_NextStep = m_StepCycle + .5f;
    }


    private void FixedUpdate()
    {
        OldMovement();
        //NewMovement();
    }

    void NewMovement()
    {
        //The get input is what sets the m_Input vector2 from our control button or key presses. 
        //This is basically the first 'are we moving' or 'are we trying to move'
        //This gets input from the input axes in the input manager
        //This also sets a variable, 'speed', to show how fast we are currently moving
        //This is done using the getrawinput because that is more responsive
        float speed = m_WalkSpeed;
        m_Input.x = Input.GetAxisRaw("Player1LR");
        m_Input.y = Input.GetAxisRaw("Player1UD");


        //This is jsut storing out input into another input vector. This originally had a purpose but does not any longer
        //TODO: Replace all instances of this in this function with the m_input variable 
        inputVector.x = m_Input.x;
        inputVector.y = m_Input.y;


        //I am defining a Vector3 called desired move. This is essentially a vector that says 'I want to move to this location, this is where I intend to go"
        //I'm not entirely sure what transform direction horizontal does other than project the movement vector into a Vector3 using the cameras direction
        Vector3 desiredMove = m_Camera.transform.TransformDirectionHorizontal(new Vector3(inputVector.x, 0, inputVector.y));


        //Now this is some math I'm not too familiar with, but I will do my best to explain it. A sphere cast is just a 'thick' raycast. Simple enough, what this is doing it sending
        //a raycast directly at out feet. We're getting the normal direction of the surface we're colliding with, essentially. In order to make sure we are moving parallel with this normal
        //(Which means basically if you press forward on a slope you don't just fly forward into space, you follow the slope) We use project on plane to make sure our movement vector stays paralell
        //with the ground
        RaycastHit hitInfo;
        bool b = Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo, m_CharacterController.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);

        Vector3 groundNormal;
        bool isGrounded = m_CharacterController.isGrounded;
        isGrounded = IsGrounded(out groundNormal);



        //If the character controller is touching the ground, there are several operations we perform when it is, and isn't. All of them will be in this block of code
        if (isGrounded)
        {
            //So what this does is basically say how well we stick to the ground if we're already touching it. This is in the case of going up a hill that is bumpy or going up a slope that suddenly changes
            //its angle. This basically keeps us stuck to the ground unless we are jumping;
            //May be unecessary
            //desiredMove.y = -m_StickToGroundForce;

            //Okay, so what this does is uses fancy math to apply friction to the character when it is collided with the ground.
            //The reason we check if we were grounded in the previous frame is, presumably for game feel reasons, that we do not want to apply friction right before we jump or right after we land
            if (m_PreviouslyGrounded && !m_Jump)
            {
                ApplyFriction(ref velocity);
            }




            //Accelerate our velocity in the direction of our desired movement
            Accelerate(ref velocity, desiredMove, groundAccelCoef);

            //Project our velocity to the ground normal we calculated earlier
            velocity = Vector3.ProjectOnPlane(velocity, groundNormal);


            //This is the m_Jump variable, it is checked in the update event so that jumping can happen more accurately as fixed update may not register the input
            if (m_Jump)
            {
                Debug.Log("HELLO");
                velocity += Vector3.up * jumpStrength;
                m_Jump = false;
            }



        }
        else
        {
            //If the controller is not grounded



            //Okay, so I'm not a scientist and i'm not sure what this is trying to achieve or why, but here's my basic understanding of it
            //If we are moving in the direction of our desired velocity then we use the acceleration coefficient, but if we are not then we use the other one
            //Unsure how this improves the experience
            float coeff = Vector3.Dot(velocity, desiredMove) > 0 ? airAccelCoef : airDeccelCoef;
            Accelerate(ref velocity, desiredMove, coeff);

            //This controls air control somehow. Through some math and vector checking that I don't entirely understand the purpose of. Basically if you are moving forward,
            //Do some air control
            if (Mathf.Abs(inputVector.y) > 0.0001) // Pure side velocity doesn't allow air control
            {
                ApplyAirControl(ref velocity, desiredMove);
            }

            //For now, this is a function that applies the basic gravity to our movement vector. Eventually this function might also calculate drag
            ApplyGravity();



        }

        //And finally, the moment of truth applying our fancy new movement vector to the character controller and moving it.
        m_CollisionFlags = m_CharacterController.Move((velocity) * Time.fixedDeltaTime);

    }


    private void Accelerate(ref Vector3 playerVelocity, Vector3 accelDir, float accelCoeff)
    {
        //How much speed we already have in the direction we want to go. Prevents us from accelerating past a certain limit if we're already moving that quickly.
        float projSpeed = Vector3.Dot(playerVelocity, accelDir);

        // How much speed we need to add (in that direction) to reach max speed
        float addSpeed = maxSpeedAD - projSpeed;
        if (addSpeed <= 0)
        {
            return;
        }

        // How much we are gonna increase our speed
        // maxSpeed * dt => the real deal. a = v / t
        // accelCoeff => ad hoc approach to make it feel better
        float accelAmount = accelCoeff * maxSpeedAD * Time.fixedDeltaTime;

        // If we are accelerating more than in a way that we exceed maxSpeedInOneDimension, crop it to max
        if (accelAmount > addSpeed)
        {
            accelAmount = addSpeed;
        }

        playerVelocity += accelDir * accelAmount;
    }

    private void ApplyFriction(ref Vector3 playerVelocity)
    {
        //Get the players current speed as a magnitude of their velocity
        float speed = playerVelocity.magnitude;
        //If the player is already effectively stopped, do not apply friction any more
        if (speed <= 0.00001)
        {
            return;
        }

        //Basically keep whether or not the speed is less than the friction threshold
        float downLimit = Mathf.Max(speed, frictionThresh); // Don't drop below treshold
        //Not sure what this does yet
        float dropAmount = speed - (downLimit * friction * Time.fixedDeltaTime);
        if (dropAmount < 0)
        {
            dropAmount = 0;
        }

        //Decrease the players velocity according to friction
        playerVelocity *= dropAmount / speed; // Reduce the velocity by a certain percent
    }

    private void ApplyDrag(ref Vector3 playerVelocity)
    {
        //Get the players current speed as a magnitude of their velocity
        float speed = playerVelocity.magnitude;
        //If the player is already effectively stopped, do not apply friction any more
        if (speed <= 0.00001)
        {
            return;
        }

        //Basically keep whether or not the speed is less than the friction threshold
        float downLimit = Mathf.Max(speed, frictionThresh); // Don't drop below treshold
        //Not sure what this does yet
        float dropAmount = speed - (downLimit * airDeccelCoef * Time.fixedDeltaTime);
        if (dropAmount < 0)
        {
            dropAmount = 0;
        }

        //Decrease the players velocity according to friction
        playerVelocity *= dropAmount / speed; // Reduce the velocity by a certain percent
    }

    private void ApplyAirControl(ref Vector3 playerVelocity, Vector3 accelDir)
    {
        // This only happens in the horizontal plane
        // TODO: Verify that these work with various gravity values
        var playerDirHorz = playerVelocity.ToHorizontal().normalized;
        var playerSpeedHorz = playerVelocity.ToHorizontal().magnitude;

        var dot = Vector3.Dot(playerDirHorz, accelDir);
        if (dot > 0)
        {
            var k = airControlPrecision * dot * dot * Time.fixedDeltaTime;

            // CPMA thingy:
            // If we want pure forward movement, we have much more air control
            bool isPureForward = Mathf.Abs(inputVector.x) < 0.0001 && Mathf.Abs(inputVector.y) > 0;
            if (isPureForward)
            {
                k *= airControlAdditionForward;
            }

            // A little bit closer to accelDir
            playerDirHorz = playerDirHorz * playerSpeedHorz + accelDir * k;
            playerDirHorz.Normalize();



            // Assign new direction, without touching the vertical speed
            playerVelocity = (playerDirHorz * playerSpeedHorz).ToHorizontal() + Gravity.Up * playerVelocity.VerticalComponent();
        }

    }

    void ApplyGravity()
    {
        velocity += Physics.gravity * m_GravityMultiplier * Time.fixedDeltaTime;
    }

    // If one of the rays hit, we're considered to be grounded
    private bool IsGrounded(out Vector3 groundNormal)
    {
        groundNormal = -Physics.gravity * m_GravityMultiplier;

        bool isGrounded = false;
        foreach (var t in groundedRayPositions)
        {
            // The last one is reserved for ghost jumps
            // Don't check that one if already on the ground
            if (isGrounded)
            {
                continue;
            }

            RaycastHit hit;
            if (Physics.Raycast(t.position, Physics.gravity * m_GravityMultiplier, out hit, 0.51f, ~excludedLayers))
            {
                groundNormal = hit.normal;
                isGrounded = true;
            }
        }

        return isGrounded;
    }

    void OldMovement()
    {
        float speed;
        GetInput(out speed);
        // always move along the camera forward as it is the direction that it being aimed at
        Vector3 desiredMove = transform.forward * m_Input.y + transform.right * m_Input.x;



        //Air Control while carrying over velocity
        if (waitForCarryVelocityBeforeMove)
        {
            desiredMove = transform.forward * m_Input.y + transform.right * m_Input.x;
            if (leftOverVelocity.magnitude > m_WalkSpeed)
            {
                desiredMove = desiredMove * 0.2f;
            }
        }

        // get a normal for the surface that is being touched to move along it
        RaycastHit hitInfo;
        Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo, m_CharacterController.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
        desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

        //Get the grapple vector
        Vector3 grappleVal = Vector3.zero;
        if (grappling && grapplePosition != Vector3.zero)
        {
            grappleVal = grapplePosition - transform.position;
            grappleVal = grappleVal.normalized;
        }

        //movetowards to make the movement flow
        if (m_CharacterController.isGrounded)
        {
            lastMove = Vector3.MoveTowards(lastMove, desiredMove, groundedDrag * Time.fixedDeltaTime);

            m_MoveDir.x = lastMove.x * speed;
            m_MoveDir.z = lastMove.z * speed;

            airMove = m_MoveDir;
        }
        else
        {
            airMove = Vector3.MoveTowards(airMove, desiredMove.normalized * airSpeed, airDrag * Time.fixedDeltaTime);

            m_MoveDir.x = airMove.x;
            m_MoveDir.z = airMove.z;

        }

        if (m_CharacterController.isGrounded)
        {
            m_MoveDir.y = -m_StickToGroundForce;

            if (m_Jump)
            {
                m_MoveDir.y = m_JumpSpeed;
                PlayJumpSound();
                m_Jump = false;
                m_Jumping = true;
            }
        }
        else
        {
            //Double Jump
            if (m_doublejump && !grappling)
            {
                m_MoveDir.y = m_doublejump_speed;
                can_doublejump = false;
                m_doublejump = false;
                //rollEffects.vectorAdditions.x += 15f;
                m_doublejumping = true;
                PlayDoubleJumpSound();
            }
            //Add gravity while in the air, but not when grappling
            if (!grappling && AddDirVector == Vector3.zero)
            {
                m_MoveDir += Physics.gravity * m_GravityMultiplier * Time.fixedDeltaTime;
            }
        }

        dirVector = Vector3.MoveTowards(dirVector, AddDirVector, dirVectorSpeed);


        if (dirVector != Vector3.zero)
        {
            m_MoveDir = dirVector;
        }

        //Handling the grapple vectors
        if (grappleVal != Vector3.zero && grappling)
        {
            grappleSpeed += grappleAcceleration * Time.fixedDeltaTime;
            grappleSpeed = Mathf.Clamp(grappleSpeed, 0, maxGrappleSpeed);
            grappleAddSpeed.y = Mathf.Clamp(grappleAddSpeed.y, -maxGrappleSpeed, maxGrappleSpeed);
            grappleAddSpeed += Physics.gravity * grappleGravityModifier * Time.fixedDeltaTime;

            float dot = Vector3.Dot(grappleVal, m_Camera.transform.forward);
            dot = Mathf.Clamp(dot, -0.5f, 1);
            dot = Remap(dot, -0.5f, 1f, 0, 1f);




            m_MoveDir = (grappleVal * grappleSpeed) + grappleAddSpeed + inheritedPlayerVelocity + (m_Camera.transform.forward * grappleCameraInfluence * (1 - Mathf.Pow(dot, 10f)));




            if (Vector3.Distance(transform.position, grapplePosition) < grappleCutDistance)
            {
                ResetGrapple();
                leftOverVelocity += m_Camera.transform.forward * grappleEndForce;

            }

        }

        //If we hit the ceiling, set the vertical speed of the leftover velocity to zero
        if ((m_CharacterController.collisionFlags & (CollisionFlags.Above)) != 0)
        {
            leftOverVelocity.y = 0;
            m_MoveDir.y = -1;
        }

        if (m_CharacterController.isGrounded)
        {
            ApplyFriction(ref leftOverVelocity);
        }
        else
        {
            //leftOverVelocity.y = 0;
            ApplyAirControl(ref leftOverVelocity, Vector3.zero);
            ApplyDrag(ref leftOverVelocity);
            //  leftOverVelocity = Vector3.MoveTowards(leftOverVelocity, Vector3.zero, airDrag * Time.fixedDeltaTime);
        }




        #region Moving Platform Stuff


        if (platformDirMag.y > 0)
        {
            Physics.SyncTransforms();
        }


        if (m_CollisionFlags != CollisionFlags.Below)
        {
            movingPlatform = false;
        }
        #endregion




        m_CollisionFlags = m_CharacterController.Move((m_MoveDir + leftOverVelocity) * Time.fixedDeltaTime);

        ProgressStepCycle(speed);
        UpdateCameraPosition(speed);

        m_MouseLook.UpdateCursorLock();
        AddDirVector = Vector3.zero;
    }

    public float Remap(float from, float fromMin, float fromMax, float toMin, float toMax)
    {
        var fromAbs = from - fromMin;
        var fromMaxAbs = fromMax - fromMin;

        var normal = fromAbs / fromMaxAbs;

        var toMaxAbs = toMax - toMin;
        var toAbs = toMaxAbs * normal;

        var to = toAbs + toMin;

        return to;
    }

    public void ResetGrapple()
    {

        grappling = false;
        grappleSpeed = initialGrappleSpeed;
        grapplePosition = Vector3.zero;
        grappleAddSpeed = Vector3.zero;
        leftOverVelocity.x = m_MoveDir.x;
        leftOverVelocity.z = m_MoveDir.z;

        can_doublejump = true;
        //leftOverVelocity.y = m_MoveDir.y;

    }

    private void PlayJumpSound()
    {
        m_AudioSource.clip = m_JumpSound;
        m_AudioSource.Play();
    }

    public void GiveCurrentPlayerVelocityToGrapple()
    {
        leftOverVelocity.x = m_MoveDir.x;
        leftOverVelocity.z = m_MoveDir.z;
        //grappleAddSpeed.y = m_MoveDir.y;
    }

    private void ProgressStepCycle(float speed)
    {
        if (m_CharacterController.velocity.sqrMagnitude > 0 && (m_Input.x != 0 || m_Input.y != 0))
        {
            m_StepCycle += (m_CharacterController.velocity.magnitude + (speed * (m_IsWalking ? 1f : m_RunstepLenghten))) *
                         Time.fixedDeltaTime;
        }

        if (!(m_StepCycle > m_NextStep))
        {
            return;
        }

        m_NextStep = m_StepCycle + m_StepInterval;

        PlayFootStepAudio();
    }


    private void PlayFootStepAudio()
    {
        if (!m_CharacterController.isGrounded)
        {
            return;
        }
        // pick & play a random footstep sound from the array,
        // excluding sound at index 0
        int n = Random.Range(1, m_FootstepSounds.Length);
        m_AudioSource.clip = m_FootstepSounds[n];
        m_AudioSource.PlayOneShot(m_AudioSource.clip);
        // move picked sound to index 0 so it's not picked next time
        m_FootstepSounds[n] = m_FootstepSounds[0];
        m_FootstepSounds[0] = m_AudioSource.clip;
    }

    private void PlayDoubleJumpSound()
    {
        m_AudioSource.PlayOneShot(doubleJumpClip);
    }


    private void UpdateCameraPosition(float speed)
    {
        Vector3 newCameraPosition;
        if (!m_UseHeadBob)
        {
            return;
        }
        if (m_CharacterController.velocity.magnitude > 0 && m_CharacterController.isGrounded)
        {
            m_Camera.transform.localPosition =
                m_HeadBob.DoHeadBob(m_CharacterController.velocity.magnitude +
                                  (speed * (m_IsWalking ? 1f : m_RunstepLenghten)));
            newCameraPosition = m_Camera.transform.localPosition;
            newCameraPosition.y = m_Camera.transform.localPosition.y - m_JumpBob.Offset();
        }
        else
        {
            newCameraPosition = m_Camera.transform.localPosition;
            newCameraPosition.y = m_OriginalCameraPosition.y - m_JumpBob.Offset();
        }
        m_Camera.transform.localPosition = newCameraPosition;
    }


    private void GetInput(out float speed)
    {
        // Read input
        float horizontal = CrossPlatformInputManager.GetAxisRaw("Player1LR");
        float vertical = CrossPlatformInputManager.GetAxisRaw("Player1UD");

        bool waswalking = m_IsWalking;

#if !MOBILE_INPUT
        // On standalone builds, walk/run speed is modified by a key press.
        // keep track of whether or not the character is walking or running
        m_IsWalking = !Input.GetKey(KeyCode.LeftShift);
#endif
        // set the desired speed to be walking or running
        speed = m_IsWalking ? m_WalkSpeed : m_RunSpeed;
        m_Input = new Vector2(horizontal, vertical);

        // normalize input if it exceeds 1 in combined length:
        if (m_Input.sqrMagnitude > 1)
        {
            m_Input.Normalize();
        }

        // handle speed change to give an fov kick
        // only if the player is going to a run, is running and the fovkick is to be used
        if (m_IsWalking != waswalking && m_UseFovKick && m_CharacterController.velocity.sqrMagnitude > 0)
        {
            StopAllCoroutines();
            StartCoroutine(!m_IsWalking ? m_FovKick.FOVKickUp() : m_FovKick.FOVKickDown());
        }
    }


    private void RotateView()
    {
        if (outsideRot)
        {

            outsideRot = false;
            return;
        }

        //if (MenuPause.GamePaused)
        //{
        //    Cursor.lockState = CursorLockMode.None;
        //    Cursor.visible = true;
        //    return;
        //}
        m_MouseLook.LookRotation(transform, m_Camera.transform);
    }


    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.layer == 16) return;

        Debug.DrawRay(hit.point, transform.up);
        if (hit.gameObject.tag.Equals("Moving Platform"))
        {
            print("a");
            print(hit.transform.GetComponent<SimpleMoveTowards>().movedir);
            transform.Translate(hit.transform.GetComponent<SimpleMoveTowards>().movedir);
        }


        Rigidbody body = hit.collider.attachedRigidbody;
        //dont move the rigidbody if the character is on top of it
        if (m_CollisionFlags == CollisionFlags.Below)
        {
            return;
        }
        else
        {
            movingPlatform = false;
            platformDirMag = Vector3.zero;
        }

        if (body == null || body.isKinematic)
        {
            return;
        }
        body.AddForceAtPosition(m_CharacterController.velocity * 0.1f, hit.point, ForceMode.Impulse);

    }


}
