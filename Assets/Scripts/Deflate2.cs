using UnityEngine;
using UnityEngine.InputSystem;

public class Deflate2 : MonoBehaviour
{
    
    public InputActionReference deflateAction;

    public float shrinkFactor = 0.5f;
    public float lerpSpeed = 5f;
    public float targetRadius = 0.5f;

    private CharacterController controller;
    private float originalRadius;

    private bool isDeflating = false;

    private Vector3 rayOrigin;
    private Vector3 rayDirectionR;
    private Vector3 rayDirectionL;
    public float rayDistance = 1f;
    public LayerMask wallLayer;
    public bool rayHit = false;

    public Transform playerTransform;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        originalRadius = controller.radius;

        rayDirectionR = transform.right;
        rayDirectionL = -transform.right;

        if (deflateAction != null)
            deflateAction.action.Enable();
    }

    void Update()
    {
        rayOrigin = playerTransform.position;

        bool deflatePressed = deflateAction != null && deflateAction.action.ReadValue<float>() > 0.5f; 

        // Handle input and send messages
        if (deflatePressed && !isDeflating)
        {
            isDeflating = true;
            SendMessage("OnDeflateStart", SendMessageOptions.DontRequireReceiver);
        }
        else if (!deflatePressed && isDeflating && !rayHit)
        {
            isDeflating = false;
            SendMessage("OnDeflateEnd", SendMessageOptions.DontRequireReceiver);
        }

        // Calculate the target radius
        float targetRadius = isDeflating ? originalRadius * shrinkFactor : originalRadius;

        // Prevent regrowth in tight spaces
        if (!isDeflating && rayHit)
            targetRadius = controller.radius;

        // Smoothly interpolate radius
        controller.radius = Mathf.Lerp(controller.radius, targetRadius, Time.deltaTime * lerpSpeed);

        RayWallCheck();
    }

    void RayWallCheck()
    {
        Debug.DrawRay(rayOrigin, rayDirectionL,Color.red, rayDistance);

        Debug.DrawRay(rayOrigin, rayDirectionR,Color.red, rayDistance);
        if (Physics.Raycast(rayOrigin, rayDirectionR, rayDistance, wallLayer))
        {
            rayHit = true;

            Debug.Log("Ray hit right wall");

        }
        if (Physics.Raycast(rayOrigin, rayDirectionL, rayDistance, wallLayer))
        {
            rayHit = true;

            Debug.Log("Ray hit left wall");

        }
        else
        {
            rayHit = false;

            Debug.Log("Ray not hitting wall");
        }
    }
}