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
      ComputeVelocity();
    }

    protected virtual void FixedUpdate()
    {
      velocity = moveInput * moveSpeed;
      body.position += velocity * Time.deltaTime;
    }

    private void ComputeVelocity()
    {
      float move = Input.GetAxisRaw("Horizontal");
      if (move > 0) moveInput = Vector2.right;
      else if (move < 0) moveInput = Vector2.left;
      else moveInput = Vector2.zero;
    }
  }
}