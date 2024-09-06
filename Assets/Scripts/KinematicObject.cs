using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class KinematicObject : MonoBehaviour
{
  [Header("Kinematic")]
  public float gravityScale = 1.0f;
  [ReadOnly] public Vector2 velocity = Vector2.zero;

  protected Rigidbody2D body;
  protected ContactFilter2D contactFilter;

  protected virtual void Awake()
  {
    body = GetComponent<Rigidbody2D>();
    body.isKinematic = true;

    contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
    contactFilter.useLayerMask = true;
    contactFilter.useTriggers = false;
  }

  protected virtual void FixedUpdate()
  {
    ProcessVelocity();
    ProcessMovement();
  }

  // move ��ŭ �̵� �� �΋H���� �浹 ����
  protected RaycastHit2D? CheckMoveCollision(Vector2 position, Vector2 move)
  {
    RaycastHit2D[] hitBuffer = new RaycastHit2D[4];
    int count = body.Cast(move.normalized, contactFilter, hitBuffer, move.magnitude);
    if (count <= 0) return null;

    int closestIndex = 0;
    // ���� ����� �浹�� �˻�
    for (int i = 0; i < count; ++i)
      if (hitBuffer[i].distance < hitBuffer[closestIndex].distance)
        closestIndex = i;

    return hitBuffer[closestIndex];
  }

  protected virtual void ProcessVelocity()
  {
    // �߷�
    velocity += Physics2D.gravity * gravityScale * Time.deltaTime;
  }

  protected virtual void ProcessMovement()
  {
    const float epsilon = 0.1f;
    const int maxMovementIteration = 5;

    Vector2 move = velocity * Time.deltaTime;
    for (int i = 0; i < maxMovementIteration; ++i)
    {
      if (move.magnitude <= 0.0f || Time.deltaTime <= 0.0f) break;

      RaycastHit2D? hit = CheckMoveCollision(body.position, move + move.normalized * epsilon);
      if (hit == null)
      {
        body.position += move;
        break;
      }

      float newDistance = hit.Value.distance - epsilon;

      // �浹�ϱ� ������ŭ �̵�
      body.position += move.normalized * newDistance;

      // velocity ����
      Vector3 surfaceTangent = Vector2.Perpendicular(hit.Value.normal);
      velocity = Vector3.Project(velocity, surfaceTangent);

      // move ����
      move = Vector3.Project(move.normalized * (move.magnitude - newDistance), surfaceTangent);
    }
  }
}