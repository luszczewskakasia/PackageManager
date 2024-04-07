using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class CameraMovement : MonoBehaviour
{
    private Vector2 _delta;

    private bool _isMoving;
    private bool _isRotating;

    private float _xRotation;
        
    [SerializeField] private float movespeed = 10f;
    [SerializeField] private float rotationspeed = 1f;

    public float ZoomChange;
    public float SmoothChange;
    public float MinSize,MaxSize;
    private Camera Cam;

    private void Awake()
    {
        _xRotation = transform.rotation.eulerAngles.x;
    }

    private void Start() 
    {
        Cam = GetComponent<Camera>();
    }

    private void Update() 
    {
        if (Input.mouseScrollDelta.y > 0) 
        { 
            Cam.orthographicSize -= ZoomChange * Time.deltaTime * SmoothChange;
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            Cam.orthographicSize += ZoomChange * Time.deltaTime * SmoothChange;
        }
        Cam.orthographicSize = Mathf.Clamp(Cam.orthographicSize,MinSize,MaxSize);
    }


    public void OnLook(InputAction.CallbackContext context) 
    {
        _delta = context.ReadValue<Vector2>();
    }

    public void OnMove(InputAction.CallbackContext context) 
    {
        _isMoving = context.started || context.performed;
    }


    public void OnRotate(InputAction.CallbackContext context) 
    {
        _isRotating = context.started || context.performed;

    }


    private void LateUpdate() 
    {
        if (_isMoving) 
        {
            var position = transform.right * (_delta.x * (-movespeed));
            position += transform.up * (_delta.y * (-movespeed));
            transform.position += position*Time.deltaTime;
              
        }

        if (_isRotating) 
        {
            transform.Rotate(new Vector3(_xRotation, _delta.x * rotationspeed, 0f));
            transform.rotation = Quaternion.Euler(_xRotation, transform.rotation.eulerAngles.y, 0f);
            
        }


    }

}
