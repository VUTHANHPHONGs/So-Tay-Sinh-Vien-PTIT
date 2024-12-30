using static scr_Models;
using UnityEngine;

public class scr_CameraController : MonoBehaviour
{
    [Header("References")]
    public scr_PlayerController playerController;
    public static scr_CameraController instance { get; private set; }
    private Vector3 targetRotation;
    private Vector3 CloneRotation;
    public GameObject yGimbal;
    private Vector3 yGibalRotation;
    private Vector3 CloneyGibalRotation;


    [Header("Settings")]
    public CameraSettingsModel settings;
    public float movementSmoothTime = 0.1f;
    private Vector3 movementVelocity;

    public bool isCheck = true;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    #region - Update -

    private void LateUpdate()
    {
        FollowPlayerCameraTarget();
        CameraRotation();
    }

    #endregion

    #region - Position / Rotation -

    private void CameraRotation()
    {
        print(isCheck);

        var viewInput = playerController.input_View;

        targetRotation.y += (settings.InvertedX ? -(viewInput.x * settings.SensitivityX) : (viewInput.x * settings.SensitivityX)) * Time.deltaTime;
      // transform.rotation = Quaternion.Euler(targetRotation);

        if (isCheck)
        {
            transform.rotation = Quaternion.Euler(targetRotation);
            CloneRotation = targetRotation;
        }

        else
        {
            transform.rotation = Quaternion.Euler(CloneRotation);
        }
           

        yGibalRotation.x += (settings.InvertedY ? (viewInput.y * settings.SensitivityY) : -(viewInput.y * settings.SensitivityY)) * Time.deltaTime;
        yGibalRotation.x = Mathf.Clamp(yGibalRotation.x, settings.YClampMin, settings.YClampMax);

      //  yGimbal.transform.localRotation = Quaternion.Euler(yGibalRotation);

        if (isCheck)
        {
            yGimbal.transform.localRotation = Quaternion.Euler(yGibalRotation);
        }

        else
        {
            yGimbal.transform.localRotation = Quaternion.Euler(Vector3.zero);
        }
            

        if (playerController.isTargetMode)
        {
            var currentRotation = playerController.transform.rotation;

            var newRotation = currentRotation.eulerAngles;
            newRotation.y = targetRotation.y;

            currentRotation = Quaternion.Lerp(currentRotation, Quaternion.Euler(newRotation), settings.CharacterRotationSmoothdamp);

            playerController.transform.rotation = currentRotation;
        }
        
    }

    private void FollowPlayerCameraTarget()
    {
        transform.position = Vector3.SmoothDamp(transform.position, playerController.cameraTarget.position, ref movementVelocity, movementSmoothTime);
    }

    #endregion


    
}
