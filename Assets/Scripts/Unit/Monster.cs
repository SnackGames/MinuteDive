using Data;
using Unit;
using UnityEngine;
using UnityEngine.U2D;

[RequireComponent(typeof(Health))]
public class Monster : KinematicObject
{
  public MonsterData monsterData;
  public GameObject hitParticlePrefab;

  [Header("Animations")]
  public bool isLookingRight = true;

  [Header("Actions")]
  [ReadOnly] protected bool isMonsterActive = false;

  [Header("Component Links")]
  public Rigidbody2D attackRigidbody;

  protected Health health;
  protected SpriteRenderer sprite;

  protected virtual void OnValidate()
  {
    sprite = GetComponent<SpriteRenderer>();
    SetLookingDirection(isLookingRight);
  }

  protected override void Awake()
  {
    base.Awake();

    health = GetComponent<Health>();
    health.SetHP(monsterData.monsterHP);
    sprite = GetComponent<SpriteRenderer>();

    SetLookingDirection(isLookingRight);
  }

  protected virtual void Update()
  {
    ProcessAnimation();
  }

  #region Animation
  protected void ProcessAnimation()
  {
    // 몬스터가 바라보는 방향
    bool isRight = false;
    if (isLookingRight) isRight = velocity.x >= 0.0f;
    else isRight = velocity.x > 0.0f;
    SetLookingDirection(isRight);
  }

  public void SetLookingDirection(bool right)
  {
    sprite.flipX = right;
    isLookingRight = right;

    if (attackRigidbody)
    {
      Vector3 newPosition = attackRigidbody.transform.localPosition;
      newPosition.x *= (newPosition.x > 0.0f) == right ? 1.0f : -1.0f;
      attackRigidbody.transform.localPosition = newPosition;
    }
  }
#endregion

  #region Movement
  protected override void ProcessVelocity()
  {
    base.ProcessVelocity();

    // 이동 테스트
    float moveDirection = isLookingRight ? 1.0f : -1.0f;
    velocity = new Vector2(monsterData.monsterMoveSpeed * moveDirection * Time.deltaTime, velocity.y);
  }

  #endregion

  public void OnHPChanged(bool isHit, int prevHp, int hp)
  {
    // 임시로 사라지며 파티클 소환
    if (isHit)
      Instantiate(hitParticlePrefab, transform.position, Quaternion.identity);

    if (hp <= 0)
      Destroy(gameObject);
  }
}