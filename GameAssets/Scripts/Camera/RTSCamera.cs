﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RTSCamera : MonoBehaviour
{

    #region Fields

    public Texture cloudTexture;
    public float startCloudsAtHeightPercent = 80f;
    public float cloudMaxAlpha = 1f;
    public float speed = 2.5f;
    public float zoomSpeed = 2.0f;
    public float zoomLerpSpeed = 5.0f;
    public float rotateSpeed = 1.0f;
    public float minHeightDistance;
    public float tiltMaxHeight = 60f;
    public float lowTilt;
    public float highTilt;
    public float maxHeight = 100f;
    public float UnitViewHeightAddition = 10f;
    public float minimumY = -60F;
    public float maximumY = 60F;
    public CameraState _state = CameraState.UnitView;

    private float rotationY;
    private float newHeight;
    private float zoomAmount = 0.0f;
    private RotateState _rotateState = RotateState.AutoAdjust;
    /// <summary>
    /// The camera will lerp to this rotation
    /// </summary>
    private Quaternion _newRot;

    #endregion

    #region Properties

    public CameraState RTSCameraState
    {
        get { return _state; }
        set
        {
            _state = value;
            if (value == CameraState.UnitView)
                transform.rotation = Quaternion.Euler(90, 0, 0);
        }
    }

    #endregion

    #region State
    public enum CameraState
    {
        WorldView,
        UnitView,
    }

    public enum RotateState
    {
        AutoAdjust,
        FreeAdjust
    }
    #endregion

    #region Initialization

    void Start()
    {
        rotationY = transform.rotation.eulerAngles.x;
        newHeight = transform.position.y;
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

    void AdjustTilt()
    {
        transform.rotation = Quaternion.Euler(new Vector3(Mathf.Lerp(lowTilt, highTilt, GetPercent(transform.position.y, tiltMaxHeight) / 100), transform.localEulerAngles.y, transform.localEulerAngles.z));
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
            case CameraState.UnitView:
                PerformUnitView();
                break;
        }
    }

    void PerformUnitView()
    {
        RaycastHit hit;
        if (Physics.Raycast(new Ray(new Vector3(transform.position.x, 1000, transform.position.z), -Vector3.up), out hit, Mathf.Infinity, 1 << 9))
        {
            newHeight = hit.point.y + UnitViewHeightAddition;
        }

        transform.position += new Vector3(transform.right.x, 0, transform.right.z) * Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        transform.position += Quaternion.Euler(0, transform.localEulerAngles.y, transform.localEulerAngles.z) * Vector3.forward * Input.GetAxis("Vertical") * speed * Time.deltaTime;
        transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, newHeight, transform.position.z), Time.deltaTime * zoomLerpSpeed);
        transform.Rotate(Vector3.up, Input.GetAxis("Rotate") * rotateSpeed * Time.deltaTime, Space.World);

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
                AdjustTilt();
                break;
            case RotateState.FreeAdjust:
                if (Input.GetMouseButton(2))
                    FreeAdjustTilt();
                break;
        }

        /*
        if (!Input.GetMouseButton(2))
        {
            newHeight = Mathf.Min(newHeight + (zoomAmount * zoomSpeed) * 2, maxHeight);
            transform.Rotate(Vector3.up, Input.GetAxis("Rotate") * rotateSpeed * Time.deltaTime, Space.World);
        }
        else
        {
            newHeight = Mathf.Min(newHeight + Input.GetAxis("Mouse Y") * zoomSpeed, maxHeight);
            transform.Rotate(Vector3.up, Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime, Space.World);
        }
        */

        RaycastHit hit;
        Physics.Raycast(new Vector3(transform.position.x, 1000, transform.position.z), -Vector3.up, out hit, Mathf.Infinity, 1 << 9);

        newHeight = Mathf.Max(Mathf.Min(newHeight + (zoomAmount * zoomSpeed) * 2, maxHeight), hit.point.y + minHeightDistance);
        transform.Rotate(Vector3.up, Input.GetAxis("Rotate") * rotateSpeed * Time.deltaTime, Space.World);

        transform.position += new Vector3(transform.right.x, 0, transform.right.z) * Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        transform.position += Quaternion.Euler(0, transform.localEulerAngles.y, transform.localEulerAngles.z) * Vector3.forward * Input.GetAxis("Vertical") * speed * Time.deltaTime;
        transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, newHeight, transform.position.z), Time.deltaTime * zoomLerpSpeed);
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