using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RTSCamera : MonoBehaviour
{

    #region Public Variables
    // Controls
    public string verticalAxis = "Vertical";                        // The Input used to move on the vertical Axis
    public string horizontalAxis = "Horizontal";                    // The Input used to move on the horizontal Axis
    public string rotateYAxis = "RotateY";                          // The Input used to rotate along the Y Axis
    public string rotateXAxis = "RotateX";                          // The Input used to rotate along the X Axis
    public KeyCode forwardKey = KeyCode.W;
    public KeyCode backwardKey = KeyCode.S;
    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;
    public KeyCode tiltIncKey = KeyCode.KeypadPlus;
    public KeyCode tiltDecKey = KeyCode.KeypadMinus;
    public KeyCode rotateLeftKey = KeyCode.Q;
    public KeyCode rotateRightKey = KeyCode.E;

    // Control Setup
    public ControlSetup verticalSetup = ControlSetup.Axis;
    public ControlSetup horizontalSetup = ControlSetup.Axis;
    public ControlSetup rotateYSetup = ControlSetup.Axis;
    public ControlSetup rotateXSetup = ControlSetup.Axis;

    // Camera Variables
    public Texture cloudTexture;
    public float startCloudsAtHeightPercent = 90f;
    public float cloudMaxAlpha = 0.15f;
    public float speed = 45f;
    public float zoomSpeed = 1f;
    public float zoomLerpSpeed = 5f;
    public float rotateSpeed = 180.0f;
    public float minHeightDistance = 5f;
    public float tiltMaxHeight = 10f;
    public float lowTilt = 15f;
    public float highTilt = 60f;
    public float tiltIncrement = 5;
    public float tiltSpeed = 2.5f;
    public float maxHeight = 125f;
    public float minimumY = -40F;
    public float maximumY = 80F;
    public bool useDeltaTimeToOne = true;
    public CameraState _state = CameraState.WorldView;

    #endregion

    #region Private Variables

    private float rotationY;
    private float newHeight;
    private float zoomAmount = 0.0f;
    private RotateState _rotateState = RotateState.AutoAdjust;
    /// <summary>
    /// The camera will lerp to this rotation
    /// </summary>
    private Quaternion _newRot;
    /// <summary>
    /// The current value used as high tilt. This is lerped to user input to provide a smooth transcation between 
    /// user view changes.
    /// </summary>
    private float _currentTilt;
    private float _targetTilt;

    #endregion

    #region Properties

    float CameraDeltaTime
    {
        get
        {
            if (useDeltaTimeToOne)
                return Time.deltaTime / Time.timeScale;
            else
                return Time.deltaTime;

        }
    }

    public CameraState RTSCameraState
    {
        get { return _state; }
        set
        {
            _state = value;
        }
    }

    #endregion

    #region States
    public enum CameraState
    {
        WorldView
    }

    public enum RotateState
    {
        AutoAdjust,
        FreeAdjust
    }

    public enum ControlSetup
    {
        Axis,
        KeyCode
    }
    #endregion

    #region Initialization

    void Start()
    {
        rotationY = transform.rotation.eulerAngles.x;
        newHeight = transform.position.y;
        _currentTilt = highTilt;
    }

    #endregion

    #region Logic

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.T))
        {
            _rotateState = _rotateState == RotateState.AutoAdjust ? RotateState.FreeAdjust : RotateState.AutoAdjust;
        }
        zoomAmount = -Input.GetAxis("Mouse ScrollWheel");
    }



    void FreeAdjustTilt()
    {
        rotationY = Mathf.Clamp(rotationY + -Input.GetAxis("Mouse Y") * rotateSpeed * Time.deltaTime, minimumY, maximumY);
        transform.rotation = Quaternion.Euler(rotationY, transform.eulerAngles.y + (Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime), 0);
    }

    void LateUpdate()
    {
        switch (_state)
        {
            case CameraState.WorldView:
                PerformWorldView();
                break;
        }
    }

    void PerformWorldView()
    {
        //Prevent the camera from droppig below the min height distance
        if (Physics.Raycast(new Ray(new Vector3(0, maxHeight, 0), -Vector3.up), minHeightDistance))
        {
            zoomAmount = zoomAmount < 0 ? 0 : zoomAmount;
        }

        if (Input.GetMouseButtonDown(2))
        {
            Screen.lockCursor = true;
        }
        if (Input.GetMouseButtonUp(2))
        {
            Screen.lockCursor = false;
        }

        switch (_rotateState)
        {
            case RotateState.AutoAdjust:
                RotateXAxis();
                break;
            case RotateState.FreeAdjust:
                if (Input.GetMouseButton(2))
                    FreeAdjustTilt();
                break;
        }

        RaycastHit hit;
        Physics.Raycast(new Vector3(transform.position.x, transform.position.y, transform.position.z), -Vector3.up, out hit, Mathf.Infinity, 1 << 9);

        // Calculate the height the camera should lerp up to. 
        newHeight = Mathf.Max(Mathf.Min(newHeight + (zoomAmount * zoomSpeed) * 2, maxHeight), hit.point.y + minHeightDistance);

        // Rotate around the Y axis
        RotateYAxis();

        // Move the Camera left or right depending on the horizontal input
        MoveHorizontal();

        // Move the Camera forward or backward depending on the vertical input
        MoveVertical();

        // Move the transform position into the new positon based off newHeight
        transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, newHeight, transform.position.z), CameraDeltaTime * zoomLerpSpeed);
    }

    void MoveVertical()
    {
        switch (verticalSetup)
        {
            case ControlSetup.Axis:
                transform.position += Quaternion.Euler(0, transform.localEulerAngles.y, transform.localEulerAngles.z) * Vector3.forward * Input.GetAxis(verticalAxis) * speed * CameraDeltaTime;
                break;
            case ControlSetup.KeyCode:
                if (Input.GetKey(forwardKey)) transform.position += Quaternion.Euler(0, transform.localEulerAngles.y, transform.localEulerAngles.z) * Vector3.forward * speed * CameraDeltaTime;

                if (Input.GetKey(backwardKey)) transform.position += Quaternion.Euler(0, transform.localEulerAngles.y, transform.localEulerAngles.z) * Vector3.forward * -1 * speed * CameraDeltaTime;
                break;
        }
    }

    void MoveHorizontal()
    {
        switch (horizontalSetup)
        {
            case ControlSetup.Axis:
                transform.position += Quaternion.Euler(0, transform.localEulerAngles.y, transform.localEulerAngles.z) * Vector3.right * Input.GetAxis(horizontalAxis) * speed * CameraDeltaTime;
                break;
            case ControlSetup.KeyCode:
                if (Input.GetKey(leftKey)) transform.position += Quaternion.Euler(0, transform.localEulerAngles.y, transform.localEulerAngles.z) * Vector3.right * Input.GetAxis(horizontalAxis) * -1 * speed * CameraDeltaTime;

                if (Input.GetKey(rightKey)) transform.position += Quaternion.Euler(0, transform.localEulerAngles.y, transform.localEulerAngles.z) * Vector3.right * Input.GetAxis(horizontalAxis) * speed * CameraDeltaTime;

                break;
        }
    }

    void RotateYAxis()
    {
        transform.Rotate(Vector3.up, Input.GetAxis(rotateYAxis) * rotateSpeed * (Time.deltaTime / Time.timeScale), Space.World);

    }


    /// <summary>
    /// Automatically adjust the tilt depending how close the camera is to the ground
    /// </summary>
    void RotateXAxis()
    {
        if (rotateXSetup == ControlSetup.KeyCode)
        {
            if (Input.GetKey(tiltDecKey))
            {
                _targetTilt = Mathf.Max(lowTilt, _targetTilt - tiltIncrement);
            }
            if (Input.GetKey(tiltIncKey))
            {
                _targetTilt = Mathf.Min(highTilt, _targetTilt + tiltIncrement);
            }
        }
        else if (rotateXSetup == ControlSetup.Axis)
        {
            _targetTilt = Mathf.Clamp(_targetTilt + Input.GetAxis(rotateXAxis) * tiltSpeed, lowTilt, highTilt);
        }

        // Adjust current tilt to the target tilt
        _currentTilt = Mathf.Lerp(_currentTilt, _targetTilt, Time.deltaTime * tiltSpeed);

        RaycastHit hit;
        Physics.Raycast(new Vector3(transform.position.x, transform.position.y, transform.position.z), -Vector3.up, out hit, Mathf.Infinity, 1 << 9);

        // Set the current tilt to rotate the Camera along the x axis. 
        transform.rotation = Quaternion.Euler(new Vector3(Mathf.Lerp(lowTilt, _currentTilt, Mathf.Max(0, GetPercent(hit.distance, tiltMaxHeight) - 100) / 100), transform.localEulerAngles.y, transform.localEulerAngles.z));
    }

    void OnGUI()
    {
        if (GetPercent(transform.position.y, maxHeight) >= startCloudsAtHeightPercent)
        {
            float min, max;
            GetCloudStartToCloudEndMinMax(out min, out max);
            GUI.color = new Color(1, 1, 1, (GetPercent(min, max) / 100) * cloudMaxAlpha);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), cloudTexture);
        }
    }

    #region HelperMethods

    float GetPercent(float number, float maxNumber)
    {
        return (number / maxNumber) * 100;
    }

    float PercentToNumber(float percent, float maxNumber)
    {
        return (percent / 100) * maxNumber;
    }

    void GetCloudStartToCloudEndMinMax(out float min, out float max)
    {
        min = transform.position.y - PercentToNumber(startCloudsAtHeightPercent, maxHeight);
        max = maxHeight - PercentToNumber(startCloudsAtHeightPercent, maxHeight);
    }

    #endregion

    #endregion


}