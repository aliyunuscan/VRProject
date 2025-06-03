// RightLever.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class RightLever : MonoBehaviour
{
    private InputDevice targetDevice;

    public CraneController CraneController;

    void Start()
    {
        List<InputDevice> devices = new List<InputDevice>();
        InputDeviceCharacteristics rightControllerCharacteristics =
            InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller;

        InputDevices.GetDevicesWithCharacteristics(rightControllerCharacteristics, devices);

        foreach (var device in devices)
        {
            Debug.Log($"[RightLever] Device found: {device.name}, characteristics: {device.characteristics}");
        }

        if (devices.Count > 0)
        {
            targetDevice = devices[0];
        }
        else
        {
            Debug.LogWarning("[RightLever] Right controller bulunamadı! (Cihaz bağlı değilse lütfen Device Simulator ekleyin veya gerçek VR bağlayın)");
        }
    }

    void Update()
    {
        if (!targetDevice.isValid)
        {
            ReinitializeDevice();
            return;
        }

        if (targetDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 axis))
        {
            float deadzone = 0.1f;

            if (Mathf.Abs(axis.y) > deadzone)
            {
                Debug.Log($"[RightLever] Joystick Y: {axis.y:F2}");
                CraneController.MoveHookY(axis.y);
            }

            if (Mathf.Abs(axis.x) > deadzone)
            {
                Debug.Log($"[RightLever] Joystick X: {axis.x:F2}");
            }
        }
    }

    private void ReinitializeDevice()
    {
        List<InputDevice> devices = new List<InputDevice>();
        InputDeviceCharacteristics rightControllerCharacteristics =
            InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller;

        InputDevices.GetDevicesWithCharacteristics(rightControllerCharacteristics, devices);
        
        if (devices.Count > 0)
        {
            targetDevice = devices[0];
            Debug.Log("[RightLever] targetDevice yeniden atandı: " + targetDevice.name);
        }
    }
}
