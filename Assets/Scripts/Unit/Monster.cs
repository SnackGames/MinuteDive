using Data;
using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Unit
{
  [Serializable]
  public enum MonsterBehaviourType
  {
    Idle,
    Pursue,
    Wait,
    Attack
  }

  [RequireComponent(typeof(Health))]
  [RequireComponent(typeof(Animator))]
  [RequireComponent(typeof(SpriteRenderer))]
  public class Monster : KinematicObject
  {
    public MonsterData monsterData;
    public GameObject hitParticlePrefab;

    [Header("Animations")]
    public bool isLookingRight = true;

    [Header("Actions")]
    [ReadOnly] public bool isMonsterActive = false;
    [ReadOnly] public MonsterBehaviourType behaviourType = MonsterBehaviourType.Idle;
    [ReadOnly] private float currentWaitingTime = 0.0f;
    [ReadOnly] private bool isInAttackState = false;
    [ReadOnly] private bool isAttacking = false;

    [Header("Component Links")]
    public Rigidbody2D attackRigidbody;

    protected Health health;
    protected Animator anim;
    protected SpriteRenderer sprite;

    private ContactFilter2D attackFilter;
    private Collider2D[] hitColliders = new Collider2D[1];

    protected virtual void OnValidate()
    {
      sprite = GetComponent<SpriteRenderer>();
      SetLookingDirection(isLookingRight);
    }

    protected override void Awake()
    {
      base.Awake();

      health = GetComponent<Health>();
      anim = GetComponent<Animator>();
      health.SetHP(monsterData.monsterHP);
      sprite = GetComponent<SpriteRenderer>();

      if (attackRigidbody)
      {
        attackFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(attackRigidbody.gameObject.layer));
        attackFilter.useLayerMask = true;
        attackFilter.useTriggers = false;
      }

      SetLookingDirection(isLookingRight);
    }

    protected virtual void Update()
    {
      ProcessAnimation();
    }

    protected override void FixedUpdate()
    {
      ProcessBehaviour();
      base.FixedUpdate();
    }

    #region Animation
    protected void ProcessAnimation()
    {
      // 몬스터가 바라보는 방향
      switch (behaviourType)
      {
        case MonsterBehaviourType.Pursue:
          Player player = Player.Get;
          if (player) SetLookingDirection(player.transform.position.x > transform.position.x);
          break;

        default:
          bool isRight = false;
          if (isLookingRight) isRight = velocity.x >= 0.0f;
          else isRight = velocity.x > 0.0f;
          SetLookingDirection(isRight);
          break;
      }
    }

    public void SetLookingDirection(bool right)
    {
      if (right == isLookingRight) return;

      isLookingRight = right;
      sprite.flipX = right;

      if (attackRigidbody)
      {
        Vector3 newPosition = attackRigidbody.transform.localPosition;
        newPosition.x *= (newPosition.x > 0.0f) == right ? 1.0f : -1.0f;
        attackRigidbody.transform.localPosition = newPosition;
      }
    }

    public void AnimTrigger_Attack(int enable) => isAttacking = enable > 0;
    public void AnimTrigger_StopAttack() => isInAttackState = false;
    #endregion

    #region Movement
    protected virtual void ProcessBehaviour()
    {
      behaviourType = MonsterBehaviourType.Idle;

      Player player = Player.Get;
      if (!player) return;

      if (isInAttackState)
      {
        behaviourType = MonsterBehaviourType.Attack;
        return;
      }

      // 임시로, 유저가 근처에 없으면 움직이지 않게 한다
      // 추후에 같은 층 (또는 윗층)에 있을 때 발동하게 할 것
      const float epsilon = 1.0f;
      if (player.transform.position.y >= transform.position.y + epsilon
        || player.transform.position.y <= transform.position.y - epsilon)
      {
        currentWaitingTime = 0.0f;
        return;
      }

      behaviourType = MonsterBehaviourType.Pursue;
      if (!attackRigidbody) return;

      // 공격 범위 밖일 시 추격       
      int count = attackRigidbody.OverlapCollider(attackFilter, hitColliders);
      if (count <= 0)
      {
        currentWaitingTime = 0.0f;
        return;
      }

      // 범위 안에 들어오면 대기
      if (currentWaitingTime < monsterData.monsterWaitTime)
      {
        currentWaitingTime += Time.deltaTime;
        behaviourType = MonsterBehaviourType.Wait;
      }

      isInAttackState = true;
      anim?.SetTrigger("Attack");
      behaviourType = MonsterBehaviourType.Attack;
      velocity = new Vector2(0.0f, velocity.y); // 공격 시 몬스터 강제 멈춤
    }

    protected override void ProcessVelocity()
    {
      base.ProcessVelocity();

      switch (behaviourType)
      {
        case MonsterBehaviourType.Idle:
        case MonsterBehaviourType.Wait:
          // 감속
          velocity.x = velocity.x > 0.0f ?
            Math.Max(0.0f, velocity.x - monsterData.monsterMoveAcceleration * Time.deltaTime) :
            Math.Min(0.0f, velocity.x + monsterData.monsterMoveAcceleration * Time.deltaTime);
          break;

        case MonsterBehaviourType.Attack:
          // 공격 시 몬스터 강제 멈춤
          velocity.x = 0.0f;
          break;

        case MonsterBehaviourType.Pursue:
          // 유저 방향으로 가속
          float accDirection = isLookingRight ? 1.0f : -1.0f;
          float newSpeed = velocity.x + (accDirection * monsterData.monsterMoveAcceleration * Time.deltaTime);
          velocity.x = newSpeed > 0.0f ? Math.Min(monsterData.monsterMoveSpeed, newSpeed) : Math.Max(-monsterData.monsterMoveSpeed, newSpeed);
          break;
      }
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
}