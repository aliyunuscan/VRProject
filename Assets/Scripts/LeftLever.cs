// RightLever.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class LeftLever : MonoBehaviour
{
    private InputDevice targetDevice;

    public CraneController CraneController;

    void Start()
    {
        List<InputDevice> devices = new List<InputDevice>();
        InputDeviceCharacteristics leftControllerCharacteristics =
            InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller;

        InputDevices.GetDevicesWithCharacteristics(leftControllerCharacteristics, devices);

        foreach (var device in devices)
        {
            Debug.Log($"[LeftLever] Device found: {device.name}, characteristics: {device.characteristics}");
        }

        if (devices.Count > 0)
        {
            targetDevice = devices[0];
        }
        else
        {
            Debug.LogWarning("[LeftLever] Left5 controller bulunamadı! (Cihaz bağlı değilse lütfen Device Simulator ekleyin veya gerçek VR bağlayın)");
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
            float deadzone = 0.05f;

            if (Mathf.Abs(axis.y) > deadzone && Mathf.Abs(axis.y) > Mathf.Abs(axis.x))
            {
                Debug.Log($"[LeftLever] Joystick Y: {axis.y:F2}");
                //car
                CraneController.MoveCar(axis.y);
            }

            if (Mathf.Abs(axis.x) > deadzone && Mathf.Abs(axis.x) > Mathf.Abs(axis.y))
            {
                Debug.Log($"[LeftLever] Joystick X: {axis.x:F2}");
                //boom
                CraneController.CraneRotation(axis.x);
            }
        }
    }

    private void ReinitializeDevice()
    {
        List<InputDevice> devices = new List<InputDevice>();
        InputDeviceCharacteristics leftControllerCharacteristics =
            InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller;

        InputDevices.GetDevicesWithCharacteristics(leftControllerCharacteristics, devices);
        
        if (devices.Count > 0)
        {
            targetDevice = devices[0];
            Debug.Log("[LeftLever] targetDevice yeniden atandı: " + targetDevice.name);
        }
    }
}
