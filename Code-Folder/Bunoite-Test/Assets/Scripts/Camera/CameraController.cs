using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    public float mouseSensitivity = 2f;
    public float smoothSpeed = 10f;
    public Vector3 offset = new Vector3(0f, 2.5f, -4f);
    public float targetHeight = 1.5f;
    
    [Header("Rotation Limits")]
    public float minVerticalAngle = -20f;
    public float maxVerticalAngle = 45f;
    
    [Header("Collision Settings")]
    public float minDistance = 1f;
    public LayerMask collisionLayers;
    
    private Transform target;
    private float rotationX = 15f;
    private float rotationY = 0f;
    private Vector3 currentRotation;
    private Vector3 smoothVelocity = Vector3.zero;
    private float currentDistance;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
            rotationY = transform.eulerAngles.y;
            rotationX = 15f;
            currentDistance = offset.magnitude;
        }
        else
        {
            Debug.LogError("Player not found! Make sure the player has the 'Player' tag.");
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        rotationY += mouseX;
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, minVerticalAngle, maxVerticalAngle);

        Vector3 nextRotation = new Vector3(rotationX, rotationY, 0);
        currentRotation = Vector3.SmoothDamp(currentRotation, nextRotation, ref smoothVelocity, 0.1f);
        transform.eulerAngles = currentRotation;

        Vector3 targetPos = target.position + Vector3.up * targetHeight;

        Vector3 desiredPosition = targetPos + transform.TransformDirection(offset);

        RaycastHit hit;
        if (Physics.Linecast(targetPos, desiredPosition, out hit, collisionLayers))
        {
            currentDistance = Mathf.Clamp((hit.distance - 0.1f), minDistance, offset.magnitude);
        }
        else
        {
            currentDistance = offset.magnitude;
        }

        Vector3 finalPosition = targetPos + transform.TransformDirection(offset.normalized * currentDistance);
        transform.position = Vector3.Lerp(transform.position, finalPosition, smoothSpeed * Time.deltaTime);
    }
} 