using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

public class RightLever : MonoBehaviour
{
    [Header("CraneController Reference")]
    public CraneController CraneController;

    [Header("Right Contr. Thumstick Action")]
    [Tooltip("XRI Default Input Actions → XRI RightHand → Thumbstick")]
    public InputActionReference rightThumbstick;

    // Lever’ın grab durumunu tutacak bayrak
    private bool isGrabbed = false;

    // XRGrabInteractable referansı (aynı GameObject üzerinde olmalı)
    private XRGrabInteractable grabInteractable;

    private float deadzone = 0.1f;

    private Coroutine holdCoroutine = null;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        if(grabInteractable == null)
        {
            Debug.LogError("XRGrabInteractable could ntot found on RightLever");
            return;
        }

        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);

        isGrabbed = true; //Test purpose
    }

    private void OnGrabbed(SelectEnterEventArgs enterArgs)
    {
        isGrabbed = true;

        if(rightThumbstick != null && rightThumbstick.action != null)
        {
            rightThumbstick.action.Enable();
        }
    }

    private void OnReleased(SelectExitEventArgs exitArgs)
    {
        isGrabbed = false;

        if(holdCoroutine != null)
        {
            StopCoroutine(holdCoroutine);
            holdCoroutine = null;
        }

        if(rightThumbstick != null && rightThumbstick.action != null)
        {
            rightThumbstick.action.Disable();
        }
    }

    private void OnEnable()
    {
        if(rightThumbstick != null && rightThumbstick.action != null)
        {
            rightThumbstick.action.performed += OnThumbstickChanged;
            rightThumbstick.action.canceled += OnThumbstickChanged;
        }
    }

    private void OnDisable()
    {
        if(rightThumbstick != null && rightThumbstick.action != null)
        {
            rightThumbstick.action.performed -= OnThumbstickChanged;
            rightThumbstick.action.canceled -= OnThumbstickChanged;
            rightThumbstick.action.Disable();
        }
    }

    private void OnThumbstickChanged(InputAction.CallbackContext context)
    {
        if (!isGrabbed) return;

        Vector2 axis = context.ReadValue<Vector2>();

        if (Mathf.Abs(axis.y) <= deadzone)
        {
            if(holdCoroutine != null)
            {
                StopCoroutine(holdCoroutine);
                holdCoroutine = null;
            }
            return;
        }

        if(holdCoroutine == null)
        {
            holdCoroutine = StartCoroutine(HoldMoveRoutine());
        }

        //No need to use axis.x for now
    }

    private IEnumerator HoldMoveRoutine()
    {
        while (true)
        {
            if (!isGrabbed || rightThumbstick == null || rightThumbstick.action == null)
            {
                holdCoroutine = null;
                yield break;
            }

            float currentY = rightThumbstick.action.ReadValue<Vector2>().y;

            if (Mathf.Abs(currentY) <= deadzone)
            {
                holdCoroutine = null;
                yield break;
            }

            CraneController.MoveHookY(currentY);
            Debug.Log($"[RightLever] (Hold) Joystick Y: {currentY:F2}");

            yield return null;//wait till next frame
        }
    }

    private void OnDestroy()
    {
        if(grabInteractable!= null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrabbed);
            grabInteractable.selectExited.RemoveListener(OnReleased);
        }

        if(rightThumbstick != null && rightThumbstick.action != null)
        {
            rightThumbstick.action.performed -= OnThumbstickChanged;
            rightThumbstick.action.canceled -= OnThumbstickChanged;
        }
    }
}
