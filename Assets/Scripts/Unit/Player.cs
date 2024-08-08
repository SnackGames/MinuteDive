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
    public float gravityScale = 1.0f;
    [ReadOnly] public float moveInput = 0.0f;
    [ReadOnly] public Vector2 velocity = Vector2.zero;
    [ReadOnly] public bool isOnGround = false;

    [Header("State")]
    [ReadOnly] public PlayerState playerState = PlayerState.Move;

    protected Rigidbody2D body;
    protected Animator anim;
    protected SpriteRenderer sprite;
    protected ContactFilter2D contactFilter;

    private bool isLookingRight = true;

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

    protected virtual void FixedUpdate()
    {
      const float epsilon = 0.01f;
      const float groundAngle = 0.85f;

      isOnGround = false;

      // 속도 설정
      velocity += Physics2D.gravity * gravityScale * Time.deltaTime;
      velocity.x = moveInput * moveSpeed;

      float distance = velocity.magnitude * Time.deltaTime;

      // 부딫히면 그 자리에서 즉시 멈추도록 작업
      if (distance > 0.0)
      {
        RaycastHit2D[] hitBuffer = new RaycastHit2D[16];
        int count = body.Cast(velocity, contactFilter, hitBuffer, distance + epsilon);
        for (int i = 0; i < count; ++i)
        {
          Vector2 hitNormal = hitBuffer[i].normal;
          if (hitNormal.y > groundAngle)
          {
            isOnGround = true;
          }

          distance = Math.Min(distance, hitBuffer[i].distance - epsilon);
        }

        body.position += velocity.normalized * distance;
      }

      // 땅일 시, 좌/우로 움직이도록 추가 작업
      if (isOnGround)
      {
        velocity.y = 0.0f;
        float newDistance = velocity.magnitude * Time.deltaTime;

        RaycastHit2D[] hitBuffer = new RaycastHit2D[16];
        int count = body.Cast(velocity, contactFilter, hitBuffer, newDistance + epsilon);
        for (int i = 0; i < count; ++i)
        {
          Vector2 hitNormal = hitBuffer[i].normal;
          if (hitNormal.y <= groundAngle)
            newDistance = Math.Min(newDistance, hitBuffer[i].distance - epsilon);
        }

        body.position += velocity.normalized * newDistance;
      }
    }
  }
}