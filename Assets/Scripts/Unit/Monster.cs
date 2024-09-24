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
    Attack
  }

  [RequireComponent(typeof(Health))]
  public class Monster : KinematicObject
  {
    public MonsterData monsterData;
    public GameObject hitParticlePrefab;

    [Header("Animations")]
    public bool isLookingRight = true;

    [Header("Actions")]
    [ReadOnly] public bool isMonsterActive = false;
    [ReadOnly] public MonsterBehaviourType behaviourType = MonsterBehaviourType.Idle;

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

    protected override void FixedUpdate()
    {
      ProcessBehaviour();
      base.FixedUpdate();
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
    protected virtual void ProcessBehaviour()
    {
      behaviourType = MonsterBehaviourType.Idle;

      Player player = Player.Get;
      if(player)
      {
        // 임시로, 유저가 근처에 없으면 움직이지 않게 한다
        // 추후에 같은 층 (또는 윗층)에 있을 때 발동하게 할 것
        const float epsilon = 1.0f;
        if (player.transform.position.y < transform.position.y + epsilon
          && player.transform.position.y > transform.position.y - epsilon)
        {
          behaviourType = MonsterBehaviourType.Pursue;
        }
      }
    }

    protected override void ProcessVelocity()
    {
      base.ProcessVelocity();

      // 임시 이동
      switch (behaviourType)
      {
        case MonsterBehaviourType.Idle:
          velocity = new Vector2(0.0f, velocity.y);
          break;

        case MonsterBehaviourType.Pursue:
          float moveDirection = isLookingRight ? 1.0f : -1.0f;
          velocity = new Vector2(monsterData.monsterMoveSpeed * moveDirection * Time.deltaTime, velocity.y);
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