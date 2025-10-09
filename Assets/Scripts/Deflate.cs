using UnityEngine;
using UnityEngine.InputSystem;
using static Unity.Cinemachine.InputAxisControllerBase<T>;
using static UnityEngine.UI.Image;

public class Deflate : MonoBehaviour
{
    private CharacterController playerCol;

    private float ccOrigRadius;
    private float ccNewRadius;

    public bool holdToDeflate = true;
    public bool rayHit = false;

    public Vector3 rayOrigin;
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
    }

    void Update()
    {
        
    }

    void OnDeflate(InputAction.CallbackContext context)
    {
        if (holdToDeflate)
        {
            if (context.started)
            {
                playerCol.radius = ccOrigRadius * ccNewRadius;
            }
            else if (context.canceled && rayHit == false)
            {
                playerCol.radius = ccOrigRadius;

            }
        }
    }

    void rayWallCheck()
    {
        if (Physics.Raycast(rayOrigin, rayDirectionR, rayDistance, wallLayer))
        {
            rayHit = true;
        }
        if (Physics.Raycast(rayOrigin, rayDirectionL, rayDistance, wallLayer))
        {
            rayHit = true;
        }
        else
        {
            rayHit = false;
        }
    }
}
