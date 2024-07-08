using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float ScreenEdgeBorderThickness = 5.0f; // distance from screen edge. Used for mouse movement

    [Header("Camera Mode")]
    [Space]
    public bool RTSModeWASD = true;
    public bool RTSModeMouse = true;
    public bool isCanZoom = true;

    [Header("Movement Speeds")]
    [Space]
    public float minPanSpeed;
    public float maxPanSpeed;
    public float secToMaxSpeed; //seconds taken to reach max speed;
    public float zoomSpeed;

    [Header("Movement Limits")]
    [Space]
    public bool enableMovementLimits;
    public Vector2 heightLimit;
    public Vector2 widthLimit;
    private Vector2 zoomLimit;

    private float panSpeed;
    private Vector3 initialPos;
    private Vector3 panMovement;
    private Vector3 pos;
    private Vector3 lastMousePosition;
    private float panIncrease = 0.0f;

    // Use this for initialization
    void Start()
    {
        initialPos = transform.position;
        zoomLimit.x = 8;
        zoomLimit.y = 32;
    }

    void Update()
    {
        #region Movement
        panMovement = Vector3.zero;

        if (RTSModeMouse)
        {
            if (Input.mousePosition.y >= Screen.height - ScreenEdgeBorderThickness)
            {
                panMovement += Vector3.up * panSpeed * Time.deltaTime;
            }
            if (Input.mousePosition.y <= ScreenEdgeBorderThickness)
            {
                panMovement += Vector3.down * panSpeed * Time.deltaTime;
            }
            if (Input.mousePosition.x <= ScreenEdgeBorderThickness)
            {
                panMovement += Vector3.left * panSpeed * Time.deltaTime;
            }
            if (Input.mousePosition.x >= Screen.width - ScreenEdgeBorderThickness)
            {
                panMovement += Vector3.right * panSpeed * Time.deltaTime;
            }
        }
        if (RTSModeWASD)
        {
            if (Input.GetKey(KeyCode.W))
            {
                panMovement += Vector3.up * panSpeed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.S))
            {
                panMovement += Vector3.down * panSpeed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.A))
            {
                panMovement += Vector3.left * panSpeed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.D))
            {
                panMovement += Vector3.right * panSpeed * Time.deltaTime;
            }
        }

        if(RTSModeMouse || RTSModeWASD)
        {
            transform.Translate(panMovement, Space.World);
        }

        //increase pan speed
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S)
            || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)
            || Input.mousePosition.y >= Screen.height - ScreenEdgeBorderThickness
            || Input.mousePosition.y <= ScreenEdgeBorderThickness
            || Input.mousePosition.x <= ScreenEdgeBorderThickness
            || Input.mousePosition.x >= Screen.width - ScreenEdgeBorderThickness)
        {
            panIncrease += Time.deltaTime / secToMaxSpeed;
            panSpeed = Mathf.Lerp(minPanSpeed, maxPanSpeed, panIncrease);
        }
        else
        {
            panIncrease = 0;
            panSpeed = minPanSpeed;
        }
        #endregion

        #region Zoom
        if (isCanZoom)
        {
            Camera.main.orthographicSize -= Input.mouseScrollDelta.y * zoomSpeed;
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, zoomLimit.x, zoomLimit.y);
        }
        #endregion

        #region boundaries
        if (enableMovementLimits == true)
        {
            //movement limits
            pos = transform.position;
            pos.y = Mathf.Clamp(pos.y, heightLimit.x, heightLimit.y);
            pos.x = Mathf.Clamp(pos.x, widthLimit.x, widthLimit.y);
            transform.position = pos;
        }
        #endregion

    }
}
