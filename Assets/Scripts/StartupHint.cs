using System.Collections;
using UnityEngine;

public class StartupHint : MonoBehaviour
{
    [Header("Popup")]
    public GameObject hintCanvas;

    [Header("Timing")]
    public float showDuration = 7f;

    [Header("Placement")]
    public float distanceFromHead = 1.5f;
    public float verticalOffset = -0.2f;
    public bool followHeadWhileVisible = true;

    private bool isVisible = false;

    void Start()
    {
        // Popup beim Start der Anwendung anzeigen und vor dem Nutzer platzieren.
        if (hintCanvas != null)
        {
            hintCanvas.SetActive(true);
            isVisible = true;
            PlaceHintInFrontOfUser();

            StartCoroutine(HideAfterDelay());
        }
    }

    void Update()
    {
        //Folgt dem Kopf, während es sichtbar ist
        if (isVisible && followHeadWhileVisible)
        {
            PlaceHintInFrontOfUser();
        }
    }

    IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(showDuration);

        if (hintCanvas != null)
        {
            hintCanvas.SetActive(false);
        }

        isVisible = false;
    }

    void PlaceHintInFrontOfUser()
    {
        Camera vrCamera = Camera.main;

        if (vrCamera == null || hintCanvas == null)
        {
            return;
        }

        Vector3 headPosition = vrCamera.transform.position;

        // Nur die horizontale Blickrichtung verwenden.
        // Dadurch erscheint der Hinweis nicht zu hoch oder unter dem Nutzer,
        // wenn der Nutzer nach oben oder unten schaut.
        Vector3 forward = vrCamera.transform.forward;
        forward.y = 0f;
        forward.Normalize();

        // Wenn die Blickrichtung null ist, auf die Standardvorwärtsrichtung setzen.
        if (forward == Vector3.zero)
        {
            forward = Vector3.forward;
        }

        // Zielposition vor dem Nutzer berechnen.
        Vector3 targetPosition = headPosition + forward * distanceFromHead;
        targetPosition.y = headPosition.y + verticalOffset;

        hintCanvas.transform.position = targetPosition;

        // Das Popup soll zum Nutzer ausgerichtet sein.
        // Die Y-Komponente wird ignoriert, damit das Canvas gerade bleibt.
        Vector3 directionToCamera = headPosition - hintCanvas.transform.position;
        directionToCamera.y = 0f;

        if (directionToCamera != Vector3.zero)
        {
            hintCanvas.transform.rotation = Quaternion.LookRotation(-directionToCamera, Vector3.up);
        }
    }
}