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

  // move 만큼 이동 시 부딫히는 충돌 정보
  protected RaycastHit2D? CheckMoveCollision(Vector2 position, Vector2 move)
  {
    RaycastHit2D[] hitBuffer = new RaycastHit2D[4];
    int count = body.Cast(move.normalized, contactFilter, hitBuffer, move.magnitude);
    if (count <= 0) return null;

    int closestIndex = 0;
    // 가장 가까운 충돌점 검색
    for (int i = 0; i < count; ++i)
      if (hitBuffer[i].distance < hitBuffer[closestIndex].distance)
        closestIndex = i;

    return hitBuffer[closestIndex];
  }

  protected virtual void ProcessVelocity()
  {
    // 중력
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

      // TODO_MMJ KinematicObject끼리 충돌 예상되는 경우에 대한 처리(좌우 이동 불가 등)
      if (hit.Value.collider.gameObject.layer == LayerMask.NameToLayer("Monster"))
      {
        Debug.Log("Collided with Monster " + hit.Value.collider.gameObject.name);
      }
      else if (hit.Value.collider.gameObject.layer == LayerMask.NameToLayer("Destructible"))
      {
        Debug.Log("Collided with Destructible " + hit.Value.collider.gameObject.name);
      }

      float newDistance = hit.Value.distance - epsilon;

      // 충돌하기 직전만큼 이동
      body.position += move.normalized * newDistance;

      // velocity 갱신
      Vector3 surfaceTangent = Vector2.Perpendicular(hit.Value.normal);
      velocity = Vector3.Project(velocity, surfaceTangent);

      // move 갱신
      move = Vector3.Project(move.normalized * (move.magnitude - newDistance), surfaceTangent);
    }
  }
}