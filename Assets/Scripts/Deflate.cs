using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Deflate : MonoBehaviour
{
    private CharacterController playerCol;

    private float ccOrigRadius;
    private float ccNewRadius = 0.3f;
    public float deflateWidthScale = 0.5f;
    private Vector3 originalScale;

    public bool holdToDeflate = true;
    public bool rayHit = false;

    public Transform playerTransform;

    private Vector3 rayOrigin;
    private Vector3 rayDirectionR;
    private Vector3 rayDirectionL;
    public float rayDistance = 1f;
    public LayerMask wallLayer;

    void Start()
    {
        playerCol = GetComponent<CharacterController>();
        ccOrigRadius = playerCol.radius;
        rayDirectionR = transform.right;
        rayDirectionL = -transform.right;
        originalScale = transform.localScale;
    }

    void Update()
    {
        rayOrigin = playerTransform.position;
        RayWallCheck();

        Debug.Log(rayHit); 
    }

    public void OnDeflate(InputValue value)
    { 
        if (holdToDeflate)
        {
            if (value.isPressed)
            {
                playerCol.radius = ccNewRadius;
                transform.localScale = new Vector3(originalScale.x * deflateWidthScale, originalScale.y, originalScale.z);

            }
            else if (rayHit == false)
            {
                playerCol.radius = ccOrigRadius;
                transform.localScale = originalScale;
            }
        }
    }

    void RayWallCheck()
    {
        Debug.DrawRay(rayOrigin, rayDirectionL * rayDistance);

        Debug.DrawRay(rayOrigin, rayDirectionR * rayDistance);
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
