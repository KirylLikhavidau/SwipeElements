using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class SwipeHandler : IInitializable, IDisposable
{
    public delegate void StartTouch(Vector2 position, float time);
    public delegate void EndTouch(Vector2 position, float time);
    
    public event StartTouch OnStartTouch;
    public event EndTouch OnEndTouch;

    private PlayerControls _playerControls;
    private Camera _mainCamera;

    public void Initialize()
    {
        _playerControls = new PlayerControls();
        _playerControls.Enable();

        _mainCamera = Camera.main;

        _playerControls.Touchscreen.PrimaryContact.started += StartTouchPrimary;
        _playerControls.Touchscreen.PrimaryContact.canceled += EndTouchPrimary;
    }

    private void StartTouchPrimary(InputAction.CallbackContext ctx)
    {
        if (OnStartTouch != null)
        {
            OnStartTouch(ScreenToWorld(_mainCamera, _playerControls.Touchscreen.PrimaryPosition.ReadValue<Vector2>()), (float)ctx.startTime);
        }
    }

    private void EndTouchPrimary(InputAction.CallbackContext ctx)
    {
        if (OnEndTouch != null)
        {
            OnEndTouch(ScreenToWorld(_mainCamera, _playerControls.Touchscreen.PrimaryPosition.ReadValue<Vector2>()), (float)ctx.time);
        }
    }

    public Vector3 ScreenToWorld(Camera camera, Vector3 position)
    {
        if (camera == null)
            return Vector3.zero;

        if (float.IsNaN(position.x) || float.IsNaN(position.y) ||
            float.IsInfinity(position.x) || float.IsInfinity(position.y))
            return Vector3.zero;

        position.z = camera.nearClipPlane;

        return camera.ScreenToWorldPoint(position);
    }

    public void Dispose()
    {
        _playerControls.Touchscreen.PrimaryContact.started -= StartTouchPrimary;
        _playerControls.Touchscreen.PrimaryContact.canceled -= EndTouchPrimary;

        _playerControls?.Disable();
    }
}
