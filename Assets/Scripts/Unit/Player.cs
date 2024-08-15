using System;
using System.Collections.Generic;
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
      ComputeInput();
      ComputeAnimation();
    }

    #region Input
    private Dictionary<ButtonInputType, bool> buttonInputs = new Dictionary<ButtonInputType, bool>();

    [VisibleEnum(typeof(ButtonInputType))]
    public void OnButtonDown(int buttonInputType)
    {
      buttonInputs[(ButtonInputType)buttonInputType] = true;
    }

    [VisibleEnum(typeof(ButtonInputType))]
    public void OnButtonUp(int buttonInputType)
    {
      buttonInputs[(ButtonInputType)buttonInputType] = false;
    }

    private void ComputeInput()
    {
      // 공격
      {
        // 임시로 ground 위에 있을때만 입력을 받음
        if (isOnGround && playerState == PlayerState.Move)
        {
          // 임시로 공격 1밖에 진행하지 않음
          if (buttonInputs.ContainsKey(ButtonInputType.Attack) && buttonInputs[ButtonInputType.Attack]) playerState = PlayerState.Attack_1;
          else if (Input.GetKeyDown("z") || Input.GetKeyDown("space")) playerState = PlayerState.Attack_1;
        }
      }

      // 이동
      {
        moveInput = 0.0f;

        if (playerState == PlayerState.Move)
        {
          // 터치 입력
          if (buttonInputs.ContainsKey(ButtonInputType.Left) && buttonInputs[ButtonInputType.Left]) moveInput = -1.0f;
          else if (buttonInputs.ContainsKey(ButtonInputType.Right) && buttonInputs[ButtonInputType.Right]) moveInput = 1.0f;
          // 기타 입력
          else moveInput = Input.GetAxisRaw("Horizontal");
        }
      }
    }
    #endregion

    #region Animation
    void ComputeAnimation()
    {
      anim.SetBool("isRunning", Math.Abs(velocity.x) > 0.0f);
      anim.SetBool("isFalling", !isOnGround);
      anim.SetBool("isAttacking", playerState == PlayerState.Attack_1);

      // 캐릭터가 바라보는 방향
      if (isLookingRight) isLookingRight = velocity.x >= 0.0f;
      else isLookingRight = velocity.x > 0.0f;
      sprite.flipX = !isLookingRight;
    }

    private void OnAnimationFinished()
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

    // position에서 move만큼 이동한다
    // 충돌 여부를 반환한다
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

      ProcessMovement();

      // 땅 위인지 여부
      isOnGround = CheckMoveCollision(body.position, Vector2.down * 0.1f) != null;
    }
  }
}