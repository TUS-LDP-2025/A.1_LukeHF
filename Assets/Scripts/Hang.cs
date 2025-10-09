using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Hang : MonoBehaviour
{
    // how far the player can be from a hangable object
    public float hangCheckRadius = 1.0f;

    // how far below the hang point the player should be when hanging
    public float hangOffset = 0.5f;

    // how fast the player moves into position when starting to hang
    public float moveToHangSpeed = 5f;

    // what tag to look for
    public string hangableTag = "Hangable";

    // reference to the CharacterController
    private CharacterController controller;

    // bools to let me know if I'm hanging or holding the button
    public bool isHanging = false;
    private bool hangButtonHeld = false;

    private Vector3 spherePos;
    public Transform sphereTransform;

    // where the player is currently hanging from
    private Transform currentHangPoint;

    // store the normal gravity for when the player stops hanging
    private Vector3 playerVelocity;
    public float gravity = -9.81f;

    private float groundedCooldown = 0;

    public FirstPersonController fpc;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // this is called automatically because of PlayerInput with "Send Messages"
    void OnHang(InputValue value)
    {
        hangButtonHeld = value.isPressed;

        if (hangButtonHeld)
        {
            TryStartHanging();
        }
        else
        {
            StopHanging();
        }
    }

    void TryStartHanging()
    {
        if (isHanging)
        {
            return; // already hanging
        }

        // find all colliders near the player
        Collider[] hits = Physics.OverlapSphere(spherePos, hangCheckRadius);

        foreach (Collider hit in hits)
        {
            // check if it's hangable
            if (hit.CompareTag(hangableTag))
            {
                // start hanging
                StartCoroutine(HangRoutine(hit.transform));
                return;
            }
        }
    }

    IEnumerator HangRoutine(Transform hangPoint)
    {
        isHanging = true;
        currentHangPoint = hangPoint;

        controller.enabled = false;

        // target position = hang point - offset down
        Vector3 targetPos = hangPoint.position - Vector3.up / hangOffset;

        // move smoothly to hang position
        while (Vector3.Distance(transform.position, targetPos) > 0.5f && hangButtonHeld)
        {
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * moveToHangSpeed);
            yield return null;
        }

        transform.position = targetPos;

        // now stay hanging in place
        while (hangButtonHeld)
        {
            yield return null;
        }

        // when H released
        StopHanging();
    }

    /*IEnumerator EnableControllerNextFrame()
    {
        yield return null; // wait one frame
        transform.position += -transform.forward * 0.3f + Vector3.down * 0.3f;
        controller.enabled = true;
        playerVelocity.y = -5f; // start falling
    }*/

    void StopHanging()
    {
        if (!isHanging)
        {
            return;
        }

        isHanging = false;
        currentHangPoint = null;

        controller.enabled = true;

        transform.position += Vector3.down * 0.3f;

        groundedCooldown = 0.2f;
        playerVelocity.y = -5f;
    }

    void Update()
    {
        spherePos = sphereTransform.position;

        if(!controller.enabled)
             return;
        

        if (groundedCooldown > 0f)
            groundedCooldown -= Time.deltaTime;

        // only apply gravity when not hanging
        if (!isHanging)
        {
            // simple gravity and ground checking
            bool canGroundCheck = groundedCooldown <= 0f;
            if (canGroundCheck && controller.isGrounded && playerVelocity.y < 0)
                playerVelocity.y = -2f;

            playerVelocity.y += gravity * Time.deltaTime;
            controller.Move(playerVelocity * Time.deltaTime);
        }
        else
        {
            // stop gravity while hanging
            playerVelocity = Vector3.zero;
        }
    }

    // draw gizmo so we can see the overlap sphere
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(spherePos, hangCheckRadius);
    }
}