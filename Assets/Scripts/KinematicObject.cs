using Unity.Burst.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class KinematicObject : MonoBehaviour
{
  [Header("Kinematic")]
  public float gravityScale = 1.0f;
  [ReadOnly] public Vector2 velocity = Vector2.zero;
  [ReadOnly] public Vector2 reservedImpulse = Vector2.zero;
  public float mass = 1.0f;
  [ReadOnly] public bool isOnGround = false;

  protected Rigidbody2D body;
  protected ContactFilter2D contactFilter;

  protected const float collisionEpsilon = 0.1f;
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

    // 땅 위인지 여부
    RaycastHit2D? hit = CheckMoveCollision(body.position, Vector2.down * (collisionEpsilon * 1.1f));
    isOnGround = hit != null && LayerMask.LayerToName(hit.Value.collider.gameObject.layer) == "Wall";
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
    // 중력: 이미 땅 위에 있을 경우 y축 방향 속도를 주지 않음 (몬스터 움직임이 덜덜 떨리는 현상 발생 방지)
    if (isOnGround)
      velocity.y = 0f;
    else
      velocity += Physics2D.gravity * gravityScale * Time.deltaTime;

    // 임펄스
    ApplyImpulse();
  }

  protected virtual void ProcessMovement()
  {
    const int maxMovementIteration = 5;

    Vector2 move = velocity * Time.deltaTime;
    for (int i = 0; i < maxMovementIteration; ++i)
    {
      if (move.magnitude <= 0.0f || Time.deltaTime <= 0.0f) break;

      RaycastHit2D? hit = CheckMoveCollision(body.position, move + move.normalized * collisionEpsilon);
      if (hit == null)
      {
        body.position += move;
        break;
      }

      float newDistance = hit.Value.distance - collisionEpsilon;

      // 충돌하기 직전만큼 이동
      body.position += move.normalized * hit.Value.distance;

      // 충돌한 면에서 살짝 띄우기
      body.position += hit.Value.normal * collisionEpsilon;

      // velocity 갱신
      Vector3 surfaceTangent = Vector2.Perpendicular(hit.Value.normal);
      velocity = Vector3.Project(velocity, surfaceTangent);

      // move 갱신
      move = Vector3.Project(move.normalized * (move.magnitude - newDistance), surfaceTangent);
    }
  }

  public void ReserveImpulse(Vector2 impulse)
  {
    reservedImpulse = impulse;
  }

  public void ApplyImpulse()
  {
    velocity += reservedImpulse;
    reservedImpulse = Vector2.zero;
  }
}