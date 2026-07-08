using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class VRHelpMenu : MonoBehaviour
{
    [Header("Menu")]
    public GameObject helpMenu;

    [Header("Placement")]
    public float distanceFromHead = 2f;
    public float verticalOffset = -0.15f;
    public bool followHeadWhileOpen = true;

    private InputDevice leftController;
    private bool previousXButtonState = false;

    void Awake()
    {
        // Menü beim App-Start sicher ausblenden
        if (helpMenu != null)
        {
            helpMenu.SetActive(false);
        }
    }

    void Update()
    {
        UpdateController(); // Aktualisiert den linken Controller, falls er nicht gültig ist
        HandleToggle(); // Überprüft, ob die X-Taste gedrückt wurde, um das Menü umzuschalten

        // Wenn das Menü aktiv ist und die Option aktiviert ist, folge dem Kopf des Nutzers
        if (helpMenu != null && helpMenu.activeSelf && followHeadWhileOpen)
        {
            PlaceMenuInFrontOfUser();
        }
    }

    void UpdateController()
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
    }

    void HandleToggle()
    {
        bool xPressed;

        // X-Button auf dem linken Quest-Controller
        if (leftController.TryGetFeatureValue(CommonUsages.primaryButton, out xPressed))
        {
            // Menü nur beim Moment des Tastendrucks umschalten
            if (xPressed && !previousXButtonState)
            {
                ToggleHelpMenu();
            }

            previousXButtonState = xPressed;
        }
    }

    void ToggleHelpMenu()
    {
        if (helpMenu == null)
        {
            Debug.LogWarning("HelpMenu ist nicht zugewiesen.");
            return;
        }

        // Umschalten des aktiven Zustands des Menüs
        bool newState = !helpMenu.activeSelf;
        helpMenu.SetActive(newState);

        // Wenn das Menü jetzt aktiv ist, platziere es vor dem Nutzer
        if (newState)
        {
            PlaceMenuInFrontOfUser();
        }
    }

    // Platziert das Menü vor dem Nutzer basierend auf der aktuellen Kopfposition und Blickrichtung
    void PlaceMenuInFrontOfUser()
    {
        Camera vrCamera = Camera.main;

        if (vrCamera == null || helpMenu == null)
        {
            Debug.LogWarning("Keine MainCamera gefunden oder HelpMenu nicht zugewiesen.");
            return;
        }

        Vector3 headPosition = vrCamera.transform.position;

        // Es wird nur die horizontale Blickrichtung verwendet.
        // Dadurch bleibt das Menü auf ungefährer Augenhöhe und landet nicht unter dem Nutzer, wenn dieser nach unten schaut.
        Vector3 forward = vrCamera.transform.forward;
        forward.y = 0f;
        forward.Normalize();

        // Wenn die Blickrichtung null ist, auf die Standardvorwärtsrichtung setzen.
        if (forward == Vector3.zero)
        {
            forward = Vector3.forward;
        }

        // Berechnet die Zielposition für das Menü basierend auf der Kopfposition, Blickrichtung und den angegebenen Abständen
        Vector3 targetPosition = headPosition + forward * distanceFromHead;
        targetPosition.y = headPosition.y + verticalOffset;

        helpMenu.transform.position = targetPosition;

        // Menü soll zur Kamera schauen
        // Die Y-Komponente wird ignoriert, damit das Menü gerade bleibt.
        Vector3 directionToCamera = headPosition - helpMenu.transform.position;
        directionToCamera.y = 0f;

        if (directionToCamera != Vector3.zero)
        {
            helpMenu.transform.rotation = Quaternion.LookRotation(-directionToCamera, Vector3.up);
        }
    }
}