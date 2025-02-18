using PlayerState;
using System;
using System.Collections.Generic;
using UnityEngine;
using GameMode;

using AYellowpaper.SerializedCollections;
using Data;
using UI;

namespace Unit
{
  [Serializable]
  public enum ButtonInputType
  {
    Left,
    Right,
    Attack
  }

  [Serializable]
  public enum PlayerStateType
  {
    Move,
    Attack,
    FallAttack,
    Dash,
    Hit,
    Dying,
    Win
  }

  [RequireComponent(typeof(Animator))]
  [RequireComponent(typeof(SpriteRenderer))]
  [RequireComponent(typeof(AudioSource))]
  public class Player : KinematicObject
  {
    [Header("Input")]
    public float reservePressedInputDuration = 0.3f;
    [ReadOnly] public float moveInput = 0.0f;
    [ReadOnly] public bool isLookingRight = true;

    [Header("State")]
    [ReadOnly] public PlayerStateType prevPlayerState = PlayerStateType.Move;
    [ReadOnly] public PlayerStateType playerState = PlayerStateType.Move;
    [ReadOnly] public PlayerStateBase playerStateBehaviour;
    [ReadOnly] public UserStateChangeData userStateChangeData;

    [Header("Movement")]
    public float moveSpeed = 5.0f;
    public float aerialMoveSpeed = 1.0f;
    public float attackMoveSpeed = 1.5f;
    public float fallAttackSpeed = 3.0f;
    public float dashSpeed = 12.0f;
    public float moveAcceleration = 50.0f;
    public float fallAttackThreshold = 1.0f;
    public float fallAttackImpulseMultiplier = 0.2f;
    [ReadOnly] public bool canFallAttack = false;
    [ReadOnly] public bool receivedImpulseDuringFallAttack = false;

    [Header("Attack")]
    [ReadOnly] public bool isAttacking = false;
    [ReadOnly] public bool isFallAttacking = false;
    private ContactFilter2D attackFilter;

    [Header("Sound")]
    [SerializedDictionary("Sound Name", "AudioClip")]
    public SerializedDictionary<string, AudioClip> playerSounds;

    [Header("Component Links")]
    public PlayerCamera playerCamera;
    public Rigidbody2D attackRigidbody;
    public Rigidbody2D fallAttackRigidbody;
    public ParticleSystem dashEffect;
    public SpriteRenderer glowSprite;

    protected Animator anim;
    protected SpriteRenderer sprite;
    protected AudioSource sound;

    // #TODO_ITEM 장착중인 아이템 정보 추가
    // #TODO_ITEM 스탯 개념 추가(공격력, 방어력)

    static private Player player;
    static public Player Get
    {
      get => player;
    }

    protected override void Awake()
    {
      base.Awake();

      player = this;

      anim = GetComponent<Animator>();
      sprite = GetComponent<SpriteRenderer>();
      sound = GetComponent<AudioSource>();

      attackFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(attackRigidbody.gameObject.layer));
      attackFilter.useLayerMask = true;
      attackFilter.useTriggers = false;

      userStateChangeData = UserStateChangeData.CreateInstance<UserStateChangeData>();
    }

    protected override void FixedUpdate()
    {
      base.FixedUpdate();

      // 낙하 공격 가능 여부
      canFallAttack = CheckMoveCollision(body.position, Vector2.down * fallAttackThreshold) == null;
    }

    protected virtual void Update()
    {
      ProcessInput();
      ProcessAttack();
    }

    #region Input
    private bool[] holdingInputs = new bool[Enum.GetNames(typeof(ButtonInputType)).Length];
    private Queue<(ButtonInputType, float)> pressedInputs = new Queue<(ButtonInputType, float)>();
    private float previousAxisInput = 0.0f;

    [VisibleEnum(typeof(ButtonInputType))]
    public void OnButtonDown(int buttonInputType)
    {
      if (buttonInputType < 0 || buttonInputType >= holdingInputs.Length)
      {
        Debug.LogError($"버튼 누르기 실패: {buttonInputType}");
        return;
      }

      holdingInputs[buttonInputType] = true;
      PressInput((ButtonInputType)buttonInputType);
    }

    [VisibleEnum(typeof(ButtonInputType))]
    public void OnButtonUp(int buttonInputType)
    {
      if (buttonInputType < 0 || buttonInputType >= holdingInputs.Length)
      {
        Debug.LogError($"버튼 떼기 실패: {buttonInputType}");
        return;
      }

      holdingInputs[buttonInputType] = false;
    }

    public bool HasPressedInput() => pressedInputs.Count > 0;
    public ButtonInputType PeekPressedInput() => pressedInputs.Peek().Item1;
    public void DequePressedInput() => pressedInputs.Dequeue();

    public bool IsHoldingInput(ButtonInputType buttonInputType)
    {
      switch (buttonInputType)
      {
        case ButtonInputType.Left: return holdingInputs[(int)buttonInputType] || Input.GetAxisRaw("Horizontal") < 0.0f;
        case ButtonInputType.Right: return holdingInputs[(int)buttonInputType] || Input.GetAxisRaw("Horizontal") > 0.0f;
        case ButtonInputType.Attack: return holdingInputs[(int)buttonInputType] || Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.Space);
      }

      return holdingInputs[(int)buttonInputType];
    }

    protected void PressInput(ButtonInputType buttonInputType)
    {
      const int maxPressedInputStack = 10;

      if (pressedInputs.Count < maxPressedInputStack)
        pressedInputs.Enqueue((buttonInputType, Time.time));
    }

    protected void ProcessInput()
    {
      // 오래된 선입력 제거
      while (pressedInputs.Count > 0)
      {
        float pressedInputTime = pressedInputs.Peek().Item2;
        if (pressedInputTime >= Time.time - reservePressedInputDuration) break;
        pressedInputs.Dequeue();
      }

      // 공격 키마 입력
      if(Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Space))
        PressInput(ButtonInputType.Attack);

      // 이동 키보드 입력
      float currentAxisInput = Input.GetAxisRaw("Horizontal");
      if (previousAxisInput != currentAxisInput)
      {
        if (previousAxisInput <= 0.0f && currentAxisInput > 0.0f)
          PressInput(ButtonInputType.Right);
        else if (previousAxisInput >= 0.0f && currentAxisInput < 0.0f)
          PressInput(ButtonInputType.Left);
      }
      previousAxisInput = currentAxisInput;

      // 이동
      moveInput = 0.0f;
      if (IsHoldingInput(ButtonInputType.Left)) moveInput = -1.0f;
      else if (IsHoldingInput(ButtonInputType.Right)) moveInput = 1.0f;
    }
    #endregion

    #region Animation
    [HideInInspector] public bool isReservedDashDirectionRight = false;
    private bool isNoGravity = false;

    public void SetLookingDirection(bool right)
    {
      sprite.flipX = !right;
      isLookingRight = right;

      Vector3 newPosition = attackRigidbody.transform.localPosition;
      newPosition.x *= (newPosition.x > 0.0f) == right ? 1.0f : -1.0f;
      attackRigidbody.transform.localPosition = newPosition;
    }

    public void AnimTrigger_EnableMoveInput(int enable) => playerStateBehaviour.AnimTrigger_EnableMoveInput(enable > 0);
    public void AnimTrigger_EnableAttackInput(int enable) => playerStateBehaviour.AnimTrigger_EnableAttackInput(enable > 0);
    public void AnimTrigger_NoGravity(int enable) => isNoGravity = enable > 0;

    public void AnimTrigger_Attack(int enable) => isAttacking = enable > 0;
    public void AnimTrigger_FallAttack(int enable) => isFallAttacking = enable > 0;

    public void AnimTrigger_Vibrate()
    {
      // #TODO 진동 세기, 시간 등 커스텀 되는 plugin 찾을것
      if (GameSettingManager.GetGameSettings().gameSettingData.enableVibrate) Handheld.Vibrate();
    }

    public void AnimTrigger_CameraShake(float strength = 1.0f) => playerCamera.ShakeCamera(strength);
    public void AnimTrigger_PlaySound(string soundName) => sound.PlayOneShot(playerSounds[soundName]);
    #endregion

    #region Movement
    protected override void ProcessVelocity()
    {
      // 중력
      if (isNoGravity || isOnGround)
        velocity.y = 0.0f;
      else
        velocity += Physics2D.gravity * gravityScale * Time.deltaTime;

      // 좌/우 이동
      float maxSpeed = isOnGround ? moveSpeed : aerialMoveSpeed;
      switch (playerState)
      {
        case PlayerStateType.Move:
          {
            // 가속
            if (Math.Abs(moveInput) > 0.0f)
            {
              float acceleration = moveAcceleration;
              // 방향전환은 가속도를 쎄게 준다
              if (moveInput * velocity.x < 0.0f) acceleration *= 3.0f;

              float newSpeed = velocity.x + moveInput * acceleration * Time.deltaTime;
              velocity.x = moveInput > 0.0f ? Math.Max(Math.Min(maxSpeed, newSpeed), velocity.x) : Math.Min(Math.Max(-maxSpeed, newSpeed), velocity.x);
            }
            // 감속
            else
            {
              velocity.x = velocity.x > 0.0f ?
                Math.Max(0.0f, velocity.x - moveAcceleration * Time.deltaTime) :
                Math.Min(0.0f, velocity.x + moveAcceleration * Time.deltaTime);
            }
          } break;

        case PlayerStateType.Attack:
          {
            // 감속
            velocity.x = velocity.x > 0.0f ?
                Math.Max(0.0f, velocity.x - moveAcceleration * Time.deltaTime) :
                Math.Min(0.0f, velocity.x + moveAcceleration * Time.deltaTime);
          } break;

        case PlayerStateType.FallAttack:
          {
            // x축 감속
            velocity.x = velocity.x > 0.0f ?
                Math.Max(0.0f, velocity.x - moveAcceleration * Time.deltaTime) :
                Math.Min(0.0f, velocity.x + moveAcceleration * Time.deltaTime);

            // y축 낙하 공격 속도
            velocity.y = isNoGravity ? 0.0f : -fallAttackSpeed;
          } break;

        case PlayerStateType.Dash:
          {
            // 속도 3배로 감속
            velocity.x = velocity.x > 0.0f ?
                Math.Max(0.0f, velocity.x - 3.0f * moveAcceleration * Time.deltaTime) :
                Math.Min(0.0f, velocity.x + 3.0f * moveAcceleration * Time.deltaTime);
          } break;

        case PlayerStateType.Hit:
          {
            // x축 감속
            velocity.x = velocity.x > 0.0f ?
                Math.Max(0.0f, velocity.x - moveAcceleration * Time.deltaTime) :
                Math.Min(0.0f, velocity.x + moveAcceleration * Time.deltaTime);
          }
          break;

        case PlayerStateType.Dying:
          {
            // x축 속도 제거
            velocity.x = 0.0f;
          }
          break;

        case PlayerStateType.Win:
          {
            // x축 속도 제거
            velocity.x = 0.0f;
          }
          break;
      }

      // 임펄스
      ApplyImpulse();
    }

    protected override void ProcessMovement()
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

        // KinematicObject끼리 충돌 예상되는 경우에 대한 처리(좌우 이동 불가 등)
        // 충돌 결과에 따라 자기 자신 혹은 충돌 대상을 밀치는 기능은 오작동 방지를 위해 유저에서만 구현한다.
        if (hit.Value.collider.gameObject.GetComponent<KinematicObject>() != null)
        {
          switch (LayerMask.LayerToName(hit.Value.collider.gameObject.layer))
          {
            case "Monster":
              {
                Monster collidingMonster = hit.Value.collider.gameObject.GetComponent<Monster>();
                if (collidingMonster != null)
                {
                  // x좌표 기준 (몬스터 중심 -> 내 중심) 방향으로 밀침
                  Vector2 monsterToUser = new Vector2(transform.position.x - collidingMonster.transform.position.x, 0f).normalized;
                  if (monsterToUser == Vector2.zero) monsterToUser.x = -1;  // 몬스터와 x좌표가 일치하는 경우, 왼쪽으로 밀침.
                  Vector2 userToMonster = -monsterToUser;

                  switch (playerState)
                  {
                    case PlayerStateType.FallAttack:
                      {
                        // 낙하 공격 중 여러 마리 몬스터를 공격하여 여러 차례 튕겨나가는 현상 방지를 위해 한 번만 밀친다.
                        if (!receivedImpulseDuringFallAttack)
                        {
                          // 몬스터와 비교해 무거울수록 덜 움직임
                          float massRatio = Mathf.Clamp(collidingMonster.mass / mass, 0.01f, 100);
                          ReserveImpulse(monsterToUser * velocity.magnitude * massRatio * fallAttackImpulseMultiplier);
                          collidingMonster.ReserveImpulse(userToMonster * (velocity.magnitude / massRatio) * fallAttackImpulseMultiplier);
                          receivedImpulseDuringFallAttack = true;
                        }
                      }
                      break;
                    case PlayerStateType.Move:
                      {
                        // 몬스터 머리 위에 낙하공격 없이 떨어지는 경우 몬스터 머리 위에 서 있을 수 없도록 임펄스 가함
                        if (hit.Value.normal.y > 0)
                        {
                          ReserveImpulse(monsterToUser * velocity.magnitude * 0.7f);
                        }
                      }
                      break;
                  }
                }
              } break;
            default:
              //Debug.Log("Collided with Undefined Layer! Object Name: " + hit.Value.collider.gameObject.name);
              break;
          }
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
    #endregion

    private HashSet<GameObject> hitObjects = new HashSet<GameObject>();
    protected void ProcessAttack()
    {
      if (!isAttacking && !isFallAttacking) hitObjects.Clear();

      if (isAttacking)
      {
        Collider2D[] hitColliders = new Collider2D[4];
        int count = attackRigidbody.OverlapCollider(attackFilter, hitColliders);
        for (int i = 0; i < count; ++i)
        {
          Health health = hitColliders[i].gameObject.GetComponent<Health>();
          if (health != null && !hitObjects.Contains(health.gameObject))
          {
            health.OnHit(1);
            hitObjects.Add(health.gameObject);
          }
        }
      }

      if (isFallAttacking)
      {
        Collider2D[] hitColliders = new Collider2D[4];
        int count = fallAttackRigidbody.OverlapCollider(attackFilter, hitColliders);
        for (int i = 0; i < count; ++i)
        {
          Health health = hitColliders[i].gameObject.GetComponent<Health>();
          if (health != null && !hitObjects.Contains(health.gameObject))
          {
            health.OnHit(1);
            hitObjects.Add(health.gameObject);
          }
        }
      }
    }

    public void OnRemainTimeExpired()
    {
      glowSprite.enabled = true;

      userStateChangeData.reserveDying(true);
      InventoryManager.GetInventory()?.SaveInventory();
      InventoryManager.GetInventory()?.ClearDroppedItems();
    }

    public void ReturnToLobby()
    {
      glowSprite.enabled = false;

      GameModeManager ModeManager = FindObjectOfType<GameModeManager>();
      if(ModeManager != null && ModeManager.CurrentMode == GameModeType.Dungeon)
      {
        transform.position = ModeManager.UserInitialPosition;
      }
    }

    public void OnSetGameMode(GameModeType Type)
    {
      switch (Type)
      {
        case GameModeType.Lobby:
          if (playerState == PlayerStateType.Dying) userStateChangeData.reserveMove(true);
          break;
      }
    }
  }
}