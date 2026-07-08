using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class MoveSplatVR : MonoBehaviour
{
    [Header("Splats")]
    public GameObject[] splats;
    public int selectedSplatIndex = 0;

    [Header("Movement")]
    public float moveSpeed = 1.0f;

    [Header("Rotation")]
    public float rotateSpeed = 60f;

    [Header("Scale")]
    public float scaleSpeed = 0.5f;

    [Header("Height Offset")]
    public float minHeightOffset = -1f;
    public float maxHeightOffset = 5f;
    public float verticalSpeed = 1.0f;
    public float manualHeightOffset = 0f;

    public const float minScale = 0.2f;
    public const float maxScale = 2f;

    [Header("Floor Constraint")]
    public bool lockToFloor = true;
    public float floorHeight = 0f;

    [Header("Recenter")]
    public float recenterDistance = 1.5f;

    [Header("External Single Splat Control")]
    public GameObject externalControlledSplat;
    public bool controlExternalSplat = false;

    private InputDevice leftController;
    private InputDevice rightController;

    private bool previousNextButtonState = false;
    private bool previousFloorButtonState = false;
    private bool previousRecenterButtonState = false;


    void Start()
    {
        // Sicherstellen, dass nur das ausgewählte Splat sichtbar ist
        ShowOnlySelectedSplat();
    }

    void Update()
    {
        // Aktualisiert die Controller-Referenzen, falls sie nicht gültig sind.
        UpdateControllers();

        //Aktuelle Splat-Referenz abrufen
        GameObject selectedSplat = GetSelectedSplat();

        if (selectedSplat == null)
        {
            return;
        }

        // Bewegung, Rotation, Skalierung und andere Interaktionen mit dem ausgewählten Splat
        MoveSelectedSplatOnFloor(selectedSplat);
        RotateAndAdjustHeight(selectedSplat);
        ScaleSelectedSplat(selectedSplat);
        HandleSplatSwitch();
        HandleRecenter(selectedSplat);
        HandleResetToFloor(selectedSplat);

        if (lockToFloor)
        {
            KeepOnFloor(selectedSplat);
        }
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

    GameObject GetSelectedSplat()
    {
        // Falls ein externes Einzel-Splat gesteuert werden soll, wird dieses statt des normalen Splat-Arrays zurückgegeben.
        if (controlExternalSplat && externalControlledSplat != null && externalControlledSplat.activeSelf)
        {
            return externalControlledSplat;
        }

        // Falls kein Splat ausgewählt ist oder das Array leer ist, wird null zurückgegeben.
        if (splats == null || splats.Length == 0)
        {
            return null;
        }

        // Sicherstellen, dass der ausgewählte Index innerhalb der Grenzen des Arrays liegt.
        if (selectedSplatIndex < 0 || selectedSplatIndex >= splats.Length)
        {
            selectedSplatIndex = 0;
        }

        return splats[selectedSplatIndex];
    }

    // Diese Methode wird vom SingleSplatManager aufgerufen, um die Kontrolle über ein einzelnes Splat zu übernehmen oder freizugeben.
    public void SetExternalControl(GameObject targetSplat, bool enable)
    {
        externalControlledSplat = targetSplat;
        controlExternalSplat = enable;

        GameObject currentTarget = GetSelectedSplat();

        if (currentTarget != null)
        {
            SyncHeightOffsetFromObject(currentTarget);
        }
    }

    /// Synchronisiert den manuellen Höhenoffset basierend auf der aktuellen Position des übergebenen Splat-Objekts.
    void SyncHeightOffsetFromObject(GameObject targetSplat)
    {
        manualHeightOffset = targetSplat.transform.position.y - floorHeight;

        manualHeightOffset = Mathf.Clamp(
            manualHeightOffset,
            minHeightOffset,
            maxHeightOffset
        );
    }

    void MoveSelectedSplatOnFloor(GameObject selectedSplat)
    {
        Vector2 leftStick;

        if (leftController.TryGetFeatureValue(CommonUsages.primary2DAxis, out leftStick))
        {
            // Deadzone gegen leichtes Driften vom Controllerstick
            if (leftStick.magnitude < 0.15f)
            {
                return;
            }

            Camera vrCamera = Camera.main;

            Vector3 forward = Vector3.forward;
            Vector3 right = Vector3.right;

            if (vrCamera != null)
            {
                forward = vrCamera.transform.forward;
                forward.y = 0f;
                forward.Normalize();

                right = vrCamera.transform.right;
                right.y = 0f;
                right.Normalize();
            }

            // Bewegung relativ zur Blickrichtung, aber nur auf dem Boden
            Vector3 movement = right * leftStick.x + forward * leftStick.y;

            selectedSplat.transform.position += movement * moveSpeed * Time.deltaTime;
        }
    }

    void RotateAndAdjustHeight(GameObject selectedSplat)
    {
        Vector2 rightStick;

        if (rightController.TryGetFeatureValue(CommonUsages.primary2DAxis, out rightStick))
        {
            float deadzone = 0.25f;

            float absX = Mathf.Abs(rightStick.x);
            float absY = Mathf.Abs(rightStick.y);

            // Wenn der Stick kaum bewegt wird, nichts machen
            if (absX < deadzone && absY < deadzone)
            {
                return;
            }

            // Wenn links/rechts stärker ist als hoch/runter: Nur drehen
            if (absX > absY)
            {
                selectedSplat.transform.Rotate(
                    Vector3.up,
                    rightStick.x * rotateSpeed * Time.deltaTime,
                    Space.World
                );
            }
            // Wenn hoch/runter stärker ist als links/rechts: Nur Höhe veraendern
            else
            {
                manualHeightOffset += rightStick.y * verticalSpeed * Time.deltaTime;
                manualHeightOffset = Mathf.Clamp(
                    manualHeightOffset,
                    minHeightOffset,
                    maxHeightOffset
                );
            }
        }
    }

    void ScaleSelectedSplat(GameObject selectedSplat)
    {
        float leftTrigger;
        float rightTrigger;

        bool hasLeftTrigger = leftController.TryGetFeatureValue(CommonUsages.trigger, out leftTrigger);
        bool hasRightTrigger = rightController.TryGetFeatureValue(CommonUsages.trigger, out rightTrigger);

        if (hasRightTrigger && rightTrigger > 0.1f)
        {
            ScaleObject(selectedSplat, 1f + rightTrigger * scaleSpeed * Time.deltaTime);
        }

        if (hasLeftTrigger && leftTrigger > 0.1f)
        {
            ScaleObject(selectedSplat, 1f - leftTrigger * scaleSpeed * Time.deltaTime);
        }
    }

    void ScaleObject(GameObject targetObject, float factor)
    {
        // Es wird uniform skaliert, also auf X/Y/Z gleich.
        // Die Skalierung wird auf die Grenzen minScale und maxScale beschränkt.
        float newScale = targetObject.transform.localScale.x * factor;
        float uniformScale = Mathf.Clamp(newScale, minScale, maxScale);

        targetObject.transform.localScale = new Vector3(
            uniformScale,
            uniformScale,
            uniformScale
        );
    }

    void HandleSplatSwitch()
    {
        bool nextButtonPressed;

        // A-Button auf rechtem Controller
        if (rightController.TryGetFeatureValue(CommonUsages.primaryButton, out nextButtonPressed))
        {
            if (nextButtonPressed && !previousNextButtonState)
            {
                SwitchToNextSplat();
            }

            previousNextButtonState = nextButtonPressed;
        }
    }

    void SwitchToNextSplat()
    {
        if (splats == null || splats.Length == 0)
        {
            return;
        }

        GameObject currentSplat = splats[selectedSplatIndex];

        // Speichert die aktuelle Position, Rotation und Skalierung des aktuellen Splats, damit das nächste Splat an derselben Stelle erscheint.
        Vector3 savedPosition = currentSplat.transform.position;
        Quaternion savedRotation = currentSplat.transform.rotation;
        Vector3 savedScale = currentSplat.transform.localScale;

        selectedSplatIndex++;

        if (selectedSplatIndex >= splats.Length)
        {
            selectedSplatIndex = 0;
        }

        GameObject nextSplat = splats[selectedSplatIndex];

        // Setzt die Position, Rotation und Skalierung des nächsten Splats auf die gespeicherten Werte.
        nextSplat.transform.position = savedPosition;
        nextSplat.transform.rotation = savedRotation;
        nextSplat.transform.localScale = savedScale;

        ShowOnlySelectedSplat();
    }

    void ShowOnlySelectedSplat()
    {
        if (splats == null || splats.Length == 0)
        {
            return;
        }

        if (selectedSplatIndex < 0 || selectedSplatIndex >= splats.Length)
        {
            selectedSplatIndex = 0;
        }

        // Es wird nur das ausgewählte Splat sichtbar gemacht, alle anderen werden deaktiviert.
        for (int i = 0; i < splats.Length; i++)
        {
            if (splats[i] == null) continue;

            splats[i].SetActive(i == selectedSplatIndex);
        }
    }

    void HandleResetToFloor(GameObject selectedSplat)
    {
        bool floorButtonPressed;

        // Y-Button auf dem linken Quest-Controller
        if (leftController.TryGetFeatureValue(CommonUsages.secondaryButton, out floorButtonPressed))
        {
            if (floorButtonPressed && !previousFloorButtonState)
            {
                ResetSelectedSplatToFloor(selectedSplat);
            }

            previousFloorButtonState = floorButtonPressed;
        }
    }

    void ResetSelectedSplatToFloor(GameObject selectedSplat)
    {
        // Höhenkorrektur zurücksetzen
        manualHeightOffset = 0f;

        Vector3 position = selectedSplat.transform.position;
        position.y = floorHeight;
        selectedSplat.transform.position = position;
    }

    void KeepOnFloor(GameObject selectedSplat)
    {
        Vector3 position = selectedSplat.transform.position;

        // Objekt bleibt relativ zum Floor-Level.
        position.y = floorHeight + manualHeightOffset;

        selectedSplat.transform.position = position;
    }

    void HandleRecenter(GameObject selectedSplat)
    {
        bool recenterButtonPressed;

        // B-Button auf dem rechten Quest-Controller
        if (rightController.TryGetFeatureValue(CommonUsages.secondaryButton, out recenterButtonPressed))
        {
            if (recenterButtonPressed && !previousRecenterButtonState)
            {
                RecenterSelectedSplat(selectedSplat);
            }

            previousRecenterButtonState = recenterButtonPressed;
        }
    }

    void RecenterSelectedSplat(GameObject selectedSplat)
    {
        Camera vrCamera = Camera.main;

        if (vrCamera == null)
        {
            return;
        }

        // Ein Ray wird von der Kamera in Blickrichtung ausgesendet, um zu prüfen, ob ein Collider getroffen wird.
        Ray ray = new Ray(vrCamera.transform.position, vrCamera.transform.forward);

        RaycastHit hit;

        // Wenn ein Collider innerhalb von 10 Metern getroffen wird, wird das Splat an dieser Position platziert.
        if (Physics.Raycast(ray, out hit, 10f))
        {
            Vector3 targetPosition = hit.point;

            // Kleiner Offset nach oben, damit das Objekt nicht im Boden steckt.
            targetPosition.y += manualHeightOffset;

            selectedSplat.transform.position = targetPosition;
        }
        else
        {
            // Fallback: Falls kein Collider getroffen wird,
            // wird das Objekt entlang deiner Blickrichtung platziert.
            Vector3 targetPosition = vrCamera.transform.position + vrCamera.transform.forward * recenterDistance;

            selectedSplat.transform.position = targetPosition;
        }
    }
}