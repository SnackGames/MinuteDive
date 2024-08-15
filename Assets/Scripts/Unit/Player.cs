using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
  public enum PlayerState
  {
    Move,
    Attack_1
  }

  [RequireComponent(typeof(Rigidbody2D),typeof(Animator),typeof(SpriteRenderer))]
  public class Player : MonoBehaviour
  {
    [Header("Movement")]
    public float moveSpeed = 5.0f;
    public float aerialMoveSpeed = 1.0f;
    public float moveAcceleration = 50.0f;
    public float gravityScale = 1.0f;
    [ReadOnly] public float moveInput = 0.0f;
    [ReadOnly] public Vector2 velocity = Vector2.zero;
    [ReadOnly] public bool isOnGround = false;
    private bool isLookingRight = true;
    private ContactFilter2D contactFilter;

    [Header("State")]
    [ReadOnly] public PlayerState playerState = PlayerState.Move;

    protected Rigidbody2D body;
    protected Animator anim;
    protected SpriteRenderer sprite;

    private void Awake()
    {
      body = GetComponent<Rigidbody2D>();
      body.isKinematic = true;

      anim = GetComponent<Animator>();

      sprite = GetComponent<SpriteRenderer>();

      contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
      contactFilter.useLayerMask = true;
      contactFilter.useTriggers = false;
    }

    protected virtual void Update()
    {
      ProcessInput();
      ProcessPlayerState();
      ProcessAnimation();
    }

    #region Input
    private bool[] holdingInputs = new bool[Enum.GetNames(typeof(ButtonInputType)).Length];
    private List<ButtonInputType> pressedInputs = new List<ButtonInputType>();
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
      pressedInputs.Add((ButtonInputType)buttonInputType);
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

    protected bool IsHoldingInput(ButtonInputType buttonInputType)
    {
      switch (buttonInputType)
      {
        case ButtonInputType.Left:   return holdingInputs[(int)buttonInputType] || Input.GetAxisRaw("Horizontal") < 0.0f;
        case ButtonInputType.Right:  return holdingInputs[(int)buttonInputType] || Input.GetAxisRaw("Horizontal") > 0.0f;
        case ButtonInputType.Attack: return holdingInputs[(int)buttonInputType] || Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.Space);
      }

      return holdingInputs[(int)buttonInputType];
    }

    protected void ProcessInput()
    {
      // 공격 키보드 입력
      if(Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Space))
        pressedInputs.Add(ButtonInputType.Attack);

      // 이동 키보드 입력
      float currentAxisInput = Input.GetAxisRaw("Horizontal");
      if (previousAxisInput != currentAxisInput)
      {
        if (previousAxisInput <= 0.0f && currentAxisInput > 0.0f)
          pressedInputs.Add(ButtonInputType.Right);
        else if (previousAxisInput >= 0.0f && currentAxisInput < 0.0f)
          pressedInputs.Add(ButtonInputType.Left);
      }
      previousAxisInput = currentAxisInput;
    }
    #endregion

    protected void ProcessPlayerState()
    {
      // 공격
      {
        // 임시로 ground 위에 있을때만 입력을 받음
        if (isOnGround && playerState == PlayerState.Move)
        {
          // 임시로 공격 1밖에 진행하지 않음
          if (IsHoldingInput(ButtonInputType.Attack)) playerState = PlayerState.Attack_1;
        }
      }

      // 이동
      {
        moveInput = 0.0f;

        if (playerState == PlayerState.Move)
        {
          // 터치 입력
          if (IsHoldingInput(ButtonInputType.Left)) moveInput = -1.0f;
          else if (IsHoldingInput(ButtonInputType.Right)) moveInput = 1.0f;
        }
      }
    }

    #region Animation
    void ProcessAnimation()
    {
      anim.SetBool("isRunning", Math.Abs(velocity.x) > 0.0f);
      anim.SetBool("isFalling", !isOnGround);
      anim.SetBool("isAttacking", playerState == PlayerState.Attack_1);

      // 캐릭터가 바라보는 방향
      if (isLookingRight) isLookingRight = velocity.x >= 0.0f;
      else isLookingRight = velocity.x > 0.0f;
      sprite.flipX = !isLookingRight;
    }

    private void AnimTrigger_Vibrate()
    {
      // #TODO 진동 세기, 시간 등 커스텀 되는 plugin 찾을것
      Handheld.Vibrate();
    }

    private void AnimTrigger_AnimFinished()
    {
      playerState = PlayerState.Move;
    }
    #endregion

    #region Movement
    // 플레이어가 move 만큼 이동 시 부딫히는 충돌 정보
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

    protected void ProcessVelocity()
    {
      // 중력
      velocity += Physics2D.gravity * gravityScale * Time.deltaTime;

      // 좌/우 이동
      {
        float maxSpeed = isOnGround ? moveSpeed : aerialMoveSpeed;

        // 가속
        if (Math.Abs(moveInput) > 0.0f)
        {
          float newSpeed = velocity.x + moveInput * moveAcceleration * Time.deltaTime;

          velocity.x = moveInput > 0.0f ? Math.Max(Math.Min(maxSpeed, newSpeed), velocity.x) : Math.Min(Math.Max(-maxSpeed, newSpeed), velocity.x);
        }
        // 감속
        else
        {
          velocity.x = velocity.x > 0.0f ?
            Math.Max(0.0f, velocity.x - moveAcceleration * Time.deltaTime) :
            Math.Min(0.0f, velocity.x + moveAcceleration * Time.deltaTime);
        }
      }
    }

    protected void ProcessMovement()
    {
      const float epsilon = 0.1f;
      const int maxMovementIteration = 5;

      Vector2 move = velocity * Time.deltaTime;
      for (int i = 0; i < maxMovementIteration; ++i)
      {
        if (move.magnitude <= 0.0f || Time.deltaTime <= 0.0f) break;

        RaycastHit2D? hit = CheckMoveCollision(body.position, move);
        if (hit == null)
        {
          body.position += move;
          break;
        }

        // #TODO_MOVEMENT
        // 현재 epsilon때문에 경사있는 면에서는 이동이 불가능하다 (또는 낙하 코너에서)
        // 따라서, 내적으로 경사면의 Tangent에 따라 epsilon의 강도가 바뀌도록 수정할 것 (또는 surfaceNormal 기준으로 살짝 띄우거나)
        float newDistance = Math.Max(0.0f, hit.Value.distance - epsilon);

        // 충돌하기 직전만큼 이동
        body.position += move.normalized * newDistance;

        // velocity 갱신
        Vector3 surfaceTangent = Vector2.Perpendicular(hit.Value.normal);
        velocity = Vector3.Project(velocity, surfaceTangent);

        // move 갱신
        move = Vector3.Project(move.normalized * (move.magnitude - newDistance), surfaceTangent);
      }
    }
    #endregion

    protected virtual void FixedUpdate()
    {
      ProcessVelocity();
      ProcessMovement();

      // 땅 위인지 여부
      isOnGround = CheckMoveCollision(body.position, Vector2.down * 0.1f) != null;
    }
  }
}