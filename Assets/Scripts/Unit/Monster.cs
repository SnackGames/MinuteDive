using Data;
using System;
using System.Collections.Generic;
using UnityEngine;

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
    public SpriteRenderer sprite;

    protected Health health;
    protected Animator anim;

    private ContactFilter2D attackFilter;
    private Collider2D[] hitColliders = new Collider2D[1];

    protected virtual void OnValidate()
    {
      SetLookingDirection(isLookingRight, true);
    }

    protected override void Awake()
    {
      base.Awake();

      health = GetComponent<Health>();
      anim = GetComponent<Animator>();
      health.SetHP(monsterData.monsterHP);

      if (attackRigidbody)
      {
        attackFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(attackRigidbody.gameObject.layer));
        attackFilter.useLayerMask = true;
        attackFilter.useTriggers = false;
      }

      SetLookingDirection(isLookingRight);
    }

    protected override void FixedUpdate()
    {
      MonsterBehaviourType prevBehaviourType = behaviourType;
      ProcessBehaviour();
      if (prevBehaviourType != behaviourType)
      {
        switch (behaviourType)
        {
          case MonsterBehaviourType.Idle:
          case MonsterBehaviourType.Wait: anim?.SetTrigger("Idle"); break;
          case MonsterBehaviourType.Pursue: anim?.SetTrigger("Pursue"); break;
          case MonsterBehaviourType.Attack: anim?.SetTrigger("Attack"); break;
        }
      }

      ProcessAnimation();
      base.FixedUpdate();
    }

    protected virtual void Update()
    {
      ProcessAttack();
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

    public void SetLookingDirection(bool right, bool forceSet = false)
    {
      if (!forceSet && right == isLookingRight) return;

      isLookingRight = right;
      sprite.flipX = right;

      if (attackRigidbody)
      {
        Vector3 newPosition = attackRigidbody.transform.localPosition;
        newPosition.x *= (newPosition.x > 0.0f) == right ? 1.0f : -1.0f;
        attackRigidbody.transform.localPosition = newPosition;
      }
    }

    public void AnimTrigger_Attack_Warning() { anim?.SetTrigger("AttackWarning"); }
    public void AnimTrigger_Attack(int enable) => isAttacking = enable > 0;
    public void AnimTrigger_StopAttack() => isInAttackState = false;
    #endregion

    #region Movement
    protected virtual void ProcessBehaviour()
    {
      behaviourType = MonsterBehaviourType.Idle;

      Player player = Player.Get;
      if (!player || player.playerState == PlayerStateType.Dying) return;

      if (isInAttackState)
      {
        currentWaitingTime = 0.0f;
        behaviourType = MonsterBehaviourType.Attack;
        return;
      }

      // 임시로, 유저가 근처에 없으면 움직이지 않게 한다
      // 추후에 같은 층 (또는 윗층)에 있을 때 발동하게 할 것
      const float epsilon = 1.0f;
      if (player.transform.position.y >= transform.position.y + epsilon
        || player.transform.position.y <= transform.position.y - epsilon
        || Math.Abs(player.transform.position.x - transform.position.x) > monsterData.monsterPursueDistance)
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
        return;
      }

      isInAttackState = true;
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

          // 가속하는 방향으로만 최대 속도에 영향을 받음. 가속하는 반대 방향으로는 최대 속도 이상으로 속도를 낼 수 있음. (impulse로 인해 날아가는 것 고려)
          if (Mathf.Sign(accDirection) == Mathf.Sign(newSpeed))
            velocity.x = newSpeed > 0.0f ? Math.Min(monsterData.monsterMoveSpeed, newSpeed) : Math.Max(-monsterData.monsterMoveSpeed, newSpeed);
          else
            velocity.x = newSpeed;

          break;
      }
    }
    #endregion

    private HashSet<GameObject> hitObjects = new HashSet<GameObject>();
    protected void ProcessAttack()
    {
      if (!isAttacking) hitObjects.Clear();
      else
      {
        Collider2D[] hitColliders = new Collider2D[4];
        int count = attackRigidbody.OverlapCollider(attackFilter, hitColliders);
        for (int i = 0; i < count; ++i)
        {
          Player player = hitColliders[i].gameObject.GetComponent<Player>();
          if (player != null && !hitObjects.Contains(player.gameObject))
          {
            FindObjectOfType<GameModeManager>()?.OnPlayerHit(monsterData.monsterDamage);
            hitObjects.Add(player.gameObject);
          }
        }
      }
    }

    public void OnHPChanged(bool isHit, int prevHp, int hp)
    {
      // 임시로 사라지며 파티클 소환
      if (isHit)
      {
        Instantiate(hitParticlePrefab, transform.position, Quaternion.identity);
        if (anim.runtimeAnimatorController)
          anim.SetTrigger("Hit");
      }

      if (hp <= 0)
      {
        DropItem(InventoryManager.DrawDropItem(monsterData.monsterDropData));
        Destroy(gameObject);
      }
    }

    public void DropItem(int itemID)
    {
      if (itemID == 0) return;

      GameObject itemUIObject = InventoryManager.GetInventory().CreateDropItem(itemID, transform.position, transform.position);
      if (itemUIObject == null)
      {
        Debug.LogError("Failed to Create Drop Item! itemId: " + itemID);
        return;
      }
    }
  }
}