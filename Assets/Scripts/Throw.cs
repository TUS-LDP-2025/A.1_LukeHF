using UnityEngine;
using UnityEngine.InputSystem;

public class Throw : MonoBehaviour
{

    public Transform cameraTransform;
    public Transform handPos;

    public float pickUpRange = 2f;
    public float throwForce = 10f;
    public float holdSmoothing = 10f;

    private GameObject heldObject;
    private Rigidbody heldRb;
    private bool isHolding = false;

   
    void Update()
    {
        if (isHolding && heldObject != null)
        {
            // Smoothly move toward hold point
            Vector3 targetPos = handPos.position;
            heldObject.transform.position = Vector3.Lerp(
                heldObject.transform.position,
                targetPos,
                Time.deltaTime * holdSmoothing
            );

            // Match camera rotation
            heldObject.transform.rotation = Quaternion.Lerp(
                heldObject.transform.rotation,
                cameraTransform.rotation,
                Time.deltaTime * holdSmoothing
            );
        }

        TryPickUp();
    }

    void OnThrow(InputValue value)
    {
        if (value.isPressed)
        {
            if (isHolding)
            {
                ThrowObject();
            }
            else
            {
                //TryPickUp();
            }
        }
    }


    void TryPickUp()
    {
        // Look for nearby throwable items
        Collider[] hits = Physics.OverlapSphere(transform.position, pickUpRange);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Throwable"))
            {
                PickUp(hit.gameObject);
                return;
            }
        }
    }

    void PickUp(GameObject obj)
    {
        isHolding = true;
        heldObject = obj;
        heldRb = obj.GetComponent<Rigidbody>();

        if (heldRb)
        {
            heldRb.useGravity = false;
            heldRb.linearVelocity = Vector3.zero;
            heldRb.angularVelocity = Vector3.zero;

            // disable collision between player and object
            Collider objCol = heldObject.GetComponent<Collider>();
            Collider playerCol = GetComponent<Collider>();
            if (playerCol && objCol)
            {
                Physics.IgnoreCollision(playerCol, objCol, true);
            }
        }

    }

    void ThrowObject()
    {
        if (!isHolding || heldObject == null)
            return;

        // Detach from player
        heldObject.transform.SetParent(null);

        // Re-enable physics
        heldRb.useGravity = true;

        // Enable collisions again
        Collider objCol = heldObject.GetComponent<Collider>();
        Collider playerCol = GetComponent<Collider>();
        if (playerCol && objCol)
        {
            Physics.IgnoreCollision(playerCol, objCol, false);
        }

        // Apply throw force in camera's forward direction
        heldRb.AddForce(cameraTransform.forward * throwForce, ForceMode.Impulse);

        // Clear reference
        heldObject = null;
        heldRb = null;
        isHolding = false;
    }

}
