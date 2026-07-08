using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class SplatManagerSingle : MonoBehaviour
{
    [Header("References")]
    public MoveSplatVR moveSplatVR;
    public GameObject singleSplat;

    [Header("Startup")]
    public bool visibleOnStart = false;

    private InputDevice leftController;
    private InputDevice rightController;

    private bool previousToggleVisibilityState = false;
    private bool previousSwitchControlState = false;

    void Start()
    {
        // Hier wird festgelegt, ob das einzelne Splat beim Start sichtbar sein soll oder nicht.
        if (singleSplat != null)
        {
            singleSplat.SetActive(visibleOnStart);
        }

        // Zu Beginn ist die Steuerung standardmäßig auf das Haupt-Splat gesetzt.
        if (moveSplatVR != null)
        {
            moveSplatVR.SetExternalControl(singleSplat, false);
        }
    }

    void Update()
    {
        // Aktualisiert die Controller-Referenzen, falls sie nicht gültig sind.
        UpdateControllers();

        // Linker Grip: Single-Splat ein-/ausblenden
        HandleVisibilityToggle();
        // Rechter Grip: Steuerung zwischen Haupt-Splat und Single-Splat wechseln
        HandleControlSwitch();
    }

    void UpdateControllers()
    {
        // Überprüft, ob der linke Controller gültig ist. Wenn nicht, versucht er, ihn erneut zu finden.
        if (!leftController.isValid)
        {
            List<InputDevice> leftDevices = new List<InputDevice>();

            InputDevices.GetDevicesWithCharacteristics(
                InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller,
                leftDevices
            );

            if (leftDevices.Count > 0)
            {
                leftController = leftDevices[0];
            }
        }

        // Überprüft, ob der rechte Controller gültig ist. Wenn nicht, versucht er, ihn erneut zu finden.
        if (!rightController.isValid)
        {
            List<InputDevice> rightDevices = new List<InputDevice>();

            InputDevices.GetDevicesWithCharacteristics(
                InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller,
                rightDevices
            );

            if (rightDevices.Count > 0)
            {
                rightController = rightDevices[0];
            }
        }
    }

    void HandleVisibilityToggle()
    {
        bool togglePressed;

        // Linker Handtrigger / Grip:
        // Einzelnes Splat ein-/ausblenden
        if (leftController.TryGetFeatureValue(CommonUsages.gripButton, out togglePressed))
        {
            if (togglePressed && !previousToggleVisibilityState)
            {
                ToggleSingleSplatVisibility();
            }

            previousToggleVisibilityState = togglePressed;
        }
    }

    void HandleControlSwitch()
    {
        bool switchPressed;

        // Rechter Handtrigger / Grip:
        // Steuerung zwischen Haupt-Splat und Single-Splat wechseln
        if (rightController.TryGetFeatureValue(CommonUsages.gripButton, out switchPressed))
        {
            if (switchPressed && !previousSwitchControlState)
            {
                ToggleControlTarget();
            }

            previousSwitchControlState = switchPressed;
        }
    }

    void ToggleSingleSplatVisibility()
    {
        if (singleSplat == null)
        {
            return;
        }

        bool newState = !singleSplat.activeSelf;

        // Nur ein-/ausblenden.
        // Transform wird NICHT zurückgesetzt.
        // Unity behält Position, Rotation und Scale automatisch bei.
        singleSplat.SetActive(newState);

        // Wenn das SingleSplat ausgeblendet wird,
        // soll die Steuerung automatisch zurück zum Haupt-Splat gehen.
        if (!newState && moveSplatVR != null)
        {
            moveSplatVR.SetExternalControl(singleSplat, false);
        }
    }

    void ToggleControlTarget()
    {
        if (moveSplatVR == null || singleSplat == null)
        {
            return;
        }

        // Wenn das SingleSplat nicht sichtbar ist,
        // darf nicht darauf gewechselt werden.
        if (!singleSplat.activeSelf)
        {
            moveSplatVR.SetExternalControl(singleSplat, false);
            return;
        }

        bool newExternalControlState = !moveSplatVR.controlExternalSplat;

        moveSplatVR.SetExternalControl(singleSplat, newExternalControlState);
    }
}