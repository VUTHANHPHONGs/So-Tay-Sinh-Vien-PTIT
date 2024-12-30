using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static scr_Models;
using static GlobalResponseData_Login;
using PlayFab.Internal;
using UnityEngine.UI;
using Cinemachine;

public class scr_PlayerController : MonoBehaviour
{
    public static scr_PlayerController instance { get; private set; }
    public FastTravel fastTravel;
    CharacterController characterController;
    public Animator characterAnimator;
    PlayerInputActions playerInputActions;
    [HideInInspector]
    public Vector2 input_Movement;
    [HideInInspector]
    public Vector2 input_View;

    Vector3 playerMovement;

    [Header("Settings")]
    public PlayerSettingsModel settings;
    public bool isTargetMode;

    [Header("Camera")]
    public Transform cameraTarget;
    public scr_CameraController cameraController;


    [Header("Movement")]
    public float movementSpeedOffset = 1;
    public float movementSmoothdamp = 0.3f;
    public bool isWalking;
    public bool isSprinting;

    private float verticalSpeed;
    private float targetVerticalSpeed;
    private float verticalSpeedVelocity;

    private float horizontalSpeed;
    private float targetHorizontalSpeed;
    private float horizontalSpeedVelocity;

    public bool isStaminaLocked = false;
    public float speedBoostDuration = 0f;
    public float staminaLockDuration;
    private float speedMultiplier = 1f;

    [Header("Gravity")]
    public float gravity;
    public float CurrentGravity;
    public float ConstantGravity;
    public float maxGravity;

    private Vector3 GravityDirection;
    private Vector3 GravityMovememt;

    public bool isCheck = true;

    [Header("Stats")]
    public PlayerStatsModel playerStats;
    //public HealthBar healthBar;
    public StaminaBar staminaBar;
    private Coroutine damageCoroutine;

    [Header("Jumping/Falling")]
    public float FallingSpeed;
    public float FallingThreshhold;
    public bool JumpingTrigger;
    #region - Start -

    private void Start()
    {
    


        if (GlobalResponseData.FirstTimeQuest == 1)
        {
            Vector3 postion = new Vector3(GlobalResponseData.x, GlobalResponseData.y, GlobalResponseData.z);
            transform.position = postion;
        }
       
      //  postion.x = GlobalResponseData.x;
      //  postion.y = GlobalResponseData.y;
      //  postion.z = GlobalResponseData.z;

        playerStats.currentHealth = playerStats.maxHealth;
     //   healthBar.SetMaxHealth(playerStats.maxHealth);
     //  StartCoroutine(DamageOverTime());

        staminaBar.SetMaxStamina(playerStats.MaxStamina);  // Thiết lập thanh thể lực
        if (damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
        }
       // damageCoroutine = StartCoroutine(DamageOverTime());
    

}

    #endregion




    #region - Awake -

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        characterController = GetComponent<CharacterController>();
        characterAnimator = GetComponent<Animator>();

        playerInputActions = new PlayerInputActions();

        playerInputActions.Movement.Movement.performed += x => input_Movement = x.ReadValue<Vector2>();
        playerInputActions.Movement.View.performed += x => input_View = x.ReadValue<Vector2>();

        playerInputActions.Actions.Jump.performed += x => Jump();

      //  playerInputActions.Actions.WalkingToggle.performed += x => ToggleWalking();
        playerInputActions.Actions.Sprint.performed += x => Sprint();
        playerInputActions.Actions.Sprint.canceled += x => StopSprinting();
        GravityDirection = Vector3.down;
    }

    #endregion

    #region - Jumping -

    private void Jump()
    {
        if (JumpingTrigger)
        {
            return;
        }
        characterAnimator.SetTrigger("Jump");
        JumpingTrigger = true;

    }

    public void ApplyJumpForce()
    {
        CurrentGravity = settings.JumpingForce;
    }

    #endregion

    #region - Sprinting -
    public void StartSpeedBoost(float multiplier, float duration)
    {
        speedMultiplier = multiplier;
        speedBoostDuration = duration;
        settings.RunningSpeed *= speedMultiplier;
        settings.SprintingSpeed *= speedMultiplier;
        StartCoroutine(ApplySpeedBoost());
    }

    private IEnumerator ApplySpeedBoost()
    {
        while (speedBoostDuration > 0)
        {
            speedBoostDuration -= Time.deltaTime;
            yield return null;
        }
       // speedMultiplier = 1f; // Reset speed multiplier after effect duration
        settings.RunningSpeed /= speedMultiplier;
        settings.SprintingSpeed /= speedMultiplier;
    }

    public void StartStaminaLock(float duration)
    {
        staminaLockDuration = duration;
        isStaminaLocked = true;
        StartCoroutine(ApplyStaminaLock(duration));
    }

    private IEnumerator ApplyStaminaLock(float duration)
    {
      
        yield return new WaitForSeconds(duration);
        isStaminaLocked = false;
    }


    private void Sprint()
    {
        if (!CanSprint())
        {
            return;
        }

        if (playerStats.Stamina > (playerStats.MaxStamina / 4))
        {
            isSprinting = true;
        }
    }
    private void StopSprinting()
    {
        isSprinting = false;
    }

    private bool CanSprint()
    {
        



        if (isTargetMode)
        {
            return false;
        }

        var sprintFalloff = 0.8f;

        if ((input_Movement.y < 0 ? input_Movement.y * -1 : input_Movement.y) < sprintFalloff && (input_Movement.x < 0 ? input_Movement.x * -1 : input_Movement.x) < sprintFalloff)
        {
            return false;
        }

        return true;
    }

    private void CalculateSprint()
    {
       if(isStaminaLocked)
        {
            staminaBar.slider.value = 100f;
            playerStats.Stamina = 100f;
           staminaBar.fill.color = Color.yellow;
        }
       else
        {
            if (!CanSprint())
            {
                isSprinting = false;
            }

            if (isSprinting && !isStaminaLocked)
            {
                if (playerStats.Stamina > 0)
                {
                    playerStats.Stamina -= playerStats.StaminaDrain * Time.deltaTime;
                }
                else
                {
                    isSprinting = false;
                }
                playerStats.StaminaCurrentDelay = playerStats.StaminaDelay;
            }
            else if (!isStaminaLocked)
            {
                if (playerStats.StaminaCurrentDelay <= 0)
                {
                    if (playerStats.Stamina < playerStats.MaxStamina)
                    {
                        playerStats.Stamina += playerStats.StaminaRestore * Time.deltaTime;
                    }
                    else
                    {
                        playerStats.Stamina = playerStats.MaxStamina;
                    }
                }
                else
                {
                    playerStats.StaminaCurrentDelay -= Time.deltaTime;
                }
            }

            staminaBar.SetStamina(playerStats.Stamina);
        }
           
       
    }

    #endregion

    /*
        #region - Health -
        IEnumerator DamageOverTime()
        {
            while (playerStats.currentHealth > 0)
            {
                yield return new WaitForSeconds(1f);
                TakeDamage(1f);
            }
        }

        void TakeDamage(float damage)
        {
            playerStats.currentHealth -= damage;
            if (playerStats.currentHealth < 0) playerStats.currentHealth = 0;
            healthBar.SetHealth(playerStats.currentHealth);
        }

        #endregion

        */


    #region - Movement -

    private void ToggleWalking()
    {
        isWalking = !isWalking;
    }

    public Transform newCam;

    private void Movement()
    {
      

        characterAnimator.SetBool("IsTargetMode", isTargetMode);

        if (isTargetMode)
        {
            if (input_Movement.y > 0)
            {
                targetVerticalSpeed = (isWalking ? settings.WalkingSpeed : settings.RunningSpeed);
            }
            else
            {
                targetVerticalSpeed = (isWalking ? settings.WalkingBackwardSpeed : settings.RunningBackwardSpeed);
            }

            targetHorizontalSpeed = (isWalking ? settings.WalkingStrafingSpeed : settings.RunningStrafingSpeed);
        }
        else
        {
            var orginalRotation = transform.rotation;
            transform.LookAt(playerMovement + transform.position, Vector3.up);
            var newRotation = transform.rotation;

            transform.rotation = Quaternion.Lerp(orginalRotation, newRotation, settings.CharacterRotationSmoothdamp);

            float playerSpeed = 0;

            if (isSprinting)
            {
                playerSpeed = settings.SprintingSpeed;
            }
            else
            {
                playerSpeed = (isWalking ? settings.WalkingSpeed : settings.RunningSpeed);
            }

            targetVerticalSpeed = playerSpeed;
            targetHorizontalSpeed = playerSpeed;
        }

        targetVerticalSpeed = (targetVerticalSpeed * movementSpeedOffset) * input_Movement.y;
        targetHorizontalSpeed = (targetHorizontalSpeed * movementSpeedOffset) * input_Movement.x;

        verticalSpeed = Mathf.SmoothDamp(verticalSpeed, targetVerticalSpeed, ref verticalSpeedVelocity, movementSmoothdamp);
        horizontalSpeed = Mathf.SmoothDamp(horizontalSpeed, targetHorizontalSpeed, ref horizontalSpeedVelocity, movementSmoothdamp);

        if (isTargetMode)
        {
            characterAnimator.SetFloat("Vertical", verticalSpeed);
            characterAnimator.SetFloat("Horizontal", horizontalSpeed);
        }
        else
        {
            float verticalActualSpeed = verticalSpeed < 0 ? verticalSpeed * -1 : verticalSpeed;
            float horizontalActualSpeed = horizontalSpeed < 0 ? horizontalSpeed * -1 : horizontalSpeed;

            float animatorVertical = verticalActualSpeed > horizontalActualSpeed ? verticalActualSpeed : horizontalActualSpeed;
            if(isCheck)
                characterAnimator.SetFloat("Vertical", animatorVertical);
            else
                characterAnimator.SetFloat("Vertical", 0);
        }

        playerMovement = cameraController.transform.forward * verticalSpeed * Time.deltaTime;
        playerMovement += cameraController.transform.right * horizontalSpeed * Time.deltaTime;

       

        print(isCheck);
        if(isCheck)
            characterController.Move(playerMovement + GravityMovememt);
        else
            characterController.Move(Vector3.zero);
    }

    #endregion

    #region - Update -

    private void Update()
    {
        CaculateGravity();
        Movement();
        CalculateSprint();
       
    }

    #endregion

    #region - Enable/Disable -

    private void OnEnable()
    {
        playerInputActions.Enable();
    }

    private void OnDisable()
    {
        playerInputActions.Disable();
    }

    private void OnTriggerEnter(Collider other)
    {
       
    }

    #endregion


    #region - Gravity -
    private bool IsGround()
    {
        return characterController.isGrounded;
    }

    private bool IsFalling()
    {
        return characterController.isGrounded;
    }
    private void CaculateGravity()
    {
        if (IsGround() && !JumpingTrigger)
        {
            CurrentGravity = ConstantGravity;
        }
        else
        {
            if( CurrentGravity > maxGravity)
            {
                CurrentGravity -= gravity * Time.deltaTime;
            }
        }
        GravityMovememt = GravityDirection * -CurrentGravity * Time.deltaTime;
    }


    private void CalculateFalling()
    {

    }
    #endregion
}
