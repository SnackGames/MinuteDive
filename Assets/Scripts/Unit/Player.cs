using UnityEngine;

namespace Unit
{
  [RequireComponent(typeof(Rigidbody2D))]
  public class Player : MonoBehaviour
  {
    [Header("Movement")]
    public float moveSpeed = 5.0f;
    [ReadOnly] public Vector2 moveInput;
    [ReadOnly] public Vector2 velocity;

    protected Rigidbody2D body;

    private void Awake()
    {
      body = GetComponent<Rigidbody2D>();
      body.isKinematic = true;
    }

    protected virtual void Update()
    {
      ComputeInput();
    }

    protected virtual void FixedUpdate()
    {
      velocity = moveInput * moveSpeed;
      body.position += velocity * Time.deltaTime;
    }

    private void ComputeInput()
    {
      moveInput = Input.GetAxisRaw("Horizontal") * Vector2.right;
    }
  }
}