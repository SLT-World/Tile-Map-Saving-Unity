using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    [Header("Default")]
    [SerializeField] bool ScrollInput = true;
    [SerializeField] bool MouseInput = true;
    [SerializeField] bool KeyInput = true;
    [SerializeField] float Speed = 7.5f;
    [SerializeField] float ZoomSpeed = 25;
    [SerializeField] float RotateSpeed = 0.125f;
    [SerializeField] float MinimumHeight = 5;
    [SerializeField] float MaximumHeight = 50;
    [SerializeField] float SmoothTime = 0.125f;

    Vector3 smoothMoveVelocity;
    Vector3 MoveAmount;
    Vector3 VMoveAmount;
    Vector3 DragOrigin;
    Vector3 DragTo;
    Vector3 RotationDragOrigin;
    Vector3 RotationDragTo;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (ScrollInput)
            HandleScrollInput();
        if (MouseInput)
            HandleMouseInput();
        if (KeyInput)
            HandleKeyInput();
    }

    private void FixedUpdate()
    {
        transform.position += MoveAmount * Time.fixedDeltaTime;
        transform.position += VMoveAmount * Time.fixedDeltaTime;
    }

    void HandleScrollInput()
    {
        float Scroll = Input.GetAxis("Mouse ScrollWheel");

        if (Physics.Raycast(transform.position, -transform.up, 5) && Scroll < 0)
        {
            Scroll = 0;
            transform.position += Vector3.SmoothDamp(MoveAmount, new Vector3(0, 0.75f, 0) * Speed, ref smoothMoveVelocity, SmoothTime) * Time.fixedDeltaTime;
        }
        else if (transform.position.y > MaximumHeight && Scroll > 0)
            Scroll = 0;
        else if (transform.position.y < MinimumHeight && Scroll < 0)
            Scroll = 0;

        Vector3 VDirection = new Vector3(0, Scroll, 0).normalized;
        VMoveAmount = Vector3.SmoothDamp(VMoveAmount, transform.TransformDirection(VDirection) * ZoomSpeed, ref smoothMoveVelocity, SmoothTime);
    }

    void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Plane _Plane = new Plane(Vector3.up, Vector3.zero);

            Ray _Ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            float _Entry;

            if (_Plane.Raycast(_Ray, out _Entry))
                DragOrigin = _Ray.GetPoint(_Entry);
        }
        if (Input.GetMouseButton(0))
        {
            Plane _Plane = new Plane(Vector3.up, Vector3.zero);

            Ray _Ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            float _Entry;

            if (_Plane.Raycast(_Ray, out _Entry))
                DragTo = _Ray.GetPoint(_Entry);

                Vector3 Direction = (transform.position + DragOrigin - DragTo).normalized;
                MoveAmount = Vector3.SmoothDamp(MoveAmount, transform.TransformDirection(Direction) * Speed, ref smoothMoveVelocity, SmoothTime);
        }
    }
    void HandleKeyInput()
    {
        float Horizontal = Input.GetAxisRaw("Horizontal");
        float Vertical = Input.GetAxisRaw("Vertical");
        if (Physics.Raycast(transform.position, transform.forward, 5) && Vertical > 0)
        {
            Vertical = 0;
            //transform.position += Vector3.SmoothDamp(MoveAmount, new Vector3(0, 0.75f, 0) * Speed, ref smoothMoveVelocity, SmoothTime) * Time.fixedDeltaTime;
        }
        if (Physics.Raycast(transform.position, -transform.forward, 5) && Vertical < 0)
        {
            Vertical = 0;
            //transform.position += Vector3.SmoothDamp(MoveAmount, new Vector3(0, 0.75f, 0) * Speed, ref smoothMoveVelocity, SmoothTime) * Time.fixedDeltaTime;
        }
        if (Physics.Raycast(transform.position, -transform.right, 5) && Horizontal < 0)
        {
            Horizontal = 0;
            //transform.position += Vector3.SmoothDamp(MoveAmount, new Vector3(0.75f, 0, 0) * Speed, ref smoothMoveVelocity, SmoothTime) * Time.fixedDeltaTime;
        }
        if (Physics.Raycast(transform.position, transform.right, 5) && Horizontal > 0)
        {
            Horizontal = 0;
            //transform.position += Vector3.SmoothDamp(MoveAmount, new Vector3(-0.75f, 0, 0) * Speed, ref smoothMoveVelocity, SmoothTime) * Time.fixedDeltaTime;
        }
        Vector3 Direction = new Vector3(Horizontal, 0, Vertical).normalized;
        MoveAmount = Vector3.SmoothDamp(MoveAmount, transform.TransformDirection(Direction) * Speed, ref smoothMoveVelocity, SmoothTime);
        if (Input.GetMouseButtonDown(2))
        {
            RotationDragOrigin = Input.mousePosition;
        }
        if (Input.GetMouseButton(2))
        {
            RotationDragTo = Input.mousePosition;

            float x = (RotationDragOrigin - RotationDragTo).x * RotateSpeed;
            float y = (RotationDragTo - RotationDragOrigin).y * RotateSpeed;
            transform.rotation *= Quaternion.Euler(new Vector3(0, x, 0));

            RotationDragOrigin = RotationDragTo;
        }
    }
}
