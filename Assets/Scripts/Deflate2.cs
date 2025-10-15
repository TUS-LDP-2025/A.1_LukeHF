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
        // Get the CharacterController on this GameObject
        controller = GetComponent<CharacterController>();

        // Save the player’s starting radius so we can shrink/grow from it
        originalRadius = controller.radius;

        // Set up left and right ray directions
        rayDirectionR = transform.right;   // right side
        rayDirectionL = -transform.right;  // left side

        // Make sure the input action is enabled so we can read input
        if (deflateAction != null)
            deflateAction.action.Enable();
    }

    void Update()
    {
        // Update the starting position for the wall-detecting rays
        rayOrigin = playerTransform.position;

        // Check if the deflate button/key is currently being pressed
        bool deflatePressed = deflateAction != null && deflateAction.action.ReadValue<float>() > 0.5f;

        // If the player just started pressing deflate
        if (deflatePressed && !isDeflating)
        {
            isDeflating = true;
        }
        // If the player released the deflate button AND we aren’t near a wall
        else if (!deflatePressed && isDeflating && !rayHit)
        {
            isDeflating = false;    
        }

        // --- Calculate the collider radius we want to move toward ---

        // If deflating, make the radius smaller; otherwise, restore it
        float targetRadius = isDeflating ? originalRadius * shrinkFactor : originalRadius;

        // If trying to grow back while touching a wall, stop the radius from increasing
        if (!isDeflating && rayHit)
            targetRadius = controller.radius;

        // Smoothly change the collider radius toward the target size over time
        controller.radius = Mathf.Lerp(controller.radius, targetRadius, Time.deltaTime * lerpSpeed);

        // Check for walls on both sides
        RayWallCheck();
    }

    void RayWallCheck()
    {
        // Draw the left and right rays in the Scene view (for debugging)
        Debug.DrawRay(rayOrigin, rayDirectionL, Color.red, rayDistance);
        Debug.DrawRay(rayOrigin, rayDirectionR, Color.red, rayDistance);

        // Check right side
        if (Physics.Raycast(rayOrigin, rayDirectionR, rayDistance, wallLayer))
        {
            rayHit = true;
            Debug.Log("Ray hit right wall");
        }

        // Check left side
        if (Physics.Raycast(rayOrigin, rayDirectionL, rayDistance, wallLayer))
        {
            rayHit = true;
            Debug.Log("Ray hit left wall");
        }
        // If the ray did not hit, reset rayHit to false
        else
        {
            rayHit = false;
            Debug.Log("Ray not hitting wall");
        }
    }
}