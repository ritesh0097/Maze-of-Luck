using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float rotateSpeed = 100f;
    [SerializeField] float jumpForce = 5f;
    

    Rigidbody playerRigid;

    Vector2 moveInput;
    float rotateInput;

    void Awake()
    {
        playerRigid = GetComponent<Rigidbody>();
    }

    // W and S movement
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    // A and D rotation
    public void OnRotate(InputValue value)
    {
        rotateInput = value.Get<float>();
    }

    void FixedUpdate()
    {
        // Forward and backward movement
        Vector3 movement = transform.forward * moveInput.y * moveSpeed;

        playerRigid.linearVelocity = new Vector3(
            movement.x,
            playerRigid.linearVelocity.y,
            movement.z
        );

        // Y-axis rotation
        float rotationAmount =
            rotateInput * rotateSpeed * Time.fixedDeltaTime;

        Quaternion targetRotation =
            playerRigid.rotation * Quaternion.Euler(0f, rotationAmount, 0f);

        playerRigid.MoveRotation(targetRotation);
    }




    // Jump

    bool isGrounded = true;
    public void OnJump(InputValue value)
    {
        if (value.isPressed && isGrounded)
        {
            playerRigid.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
