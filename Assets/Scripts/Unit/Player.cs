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
      // ����
      {
        // �ӽ÷� ground ���� �������� �Է��� ����
        if (isOnGround && playerState == PlayerState.Move)
        {
          // �ӽ÷� ���� 1�ۿ� �������� ����
          if (buttonInputs.ContainsKey(ButtonInputType.Attack) && buttonInputs[ButtonInputType.Attack]) playerState = PlayerState.Attack_1;
          else if (Input.GetKeyDown("z") || Input.GetKeyDown("space")) playerState = PlayerState.Attack_1;
        }
      }

      // �̵�
      {
        moveInput = 0.0f;

        if (playerState == PlayerState.Move)
        {
          // ��ġ �Է�
          if (buttonInputs.ContainsKey(ButtonInputType.Left) && buttonInputs[ButtonInputType.Left]) moveInput = -1.0f;
          else if (buttonInputs.ContainsKey(ButtonInputType.Right) && buttonInputs[ButtonInputType.Right]) moveInput = 1.0f;
          // ��Ÿ �Է�
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

      // ĳ���Ͱ� �ٶ󺸴� ����
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
    // �÷��̾ move ��ŭ �̵� �� �΋H���� �浹 ����
    protected RaycastHit2D? CheckMoveCollision(Vector2 position, Vector2 move)
    {
      RaycastHit2D[] hitBuffer = new RaycastHit2D[4];
      int count = body.Cast(move.normalized, contactFilter, hitBuffer, move.magnitude);
      if (count <= 0) return null;

      int closestIndex = 0;
      // ���� ����� �浹�� �˻�
      for (int i = 0; i < count; ++i)
        if (hitBuffer[i].distance < hitBuffer[closestIndex].distance)
          closestIndex = i;

      return hitBuffer[closestIndex];
    }

    // position���� move��ŭ �̵��Ѵ�
    // �浹 ���θ� ��ȯ�Ѵ�
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
        // ���� epsilon������ ����ִ� �鿡���� �̵��� �Ұ����ϴ� (�Ǵ� ���� �ڳʿ���)
        // ����, �������� ������ Tangent�� ���� epsilon�� ������ �ٲ�� ������ �� (�Ǵ� surfaceNormal �������� ��¦ ���ų�)
        float newDistance = Math.Max(0.0f, hit.Value.distance - epsilon);

        // �浹�ϱ� ������ŭ �̵�
        body.position += move.normalized * newDistance;

        // velocity ����
        Vector3 surfaceTangent = Vector2.Perpendicular(hit.Value.normal);
        velocity = Vector3.Project(velocity, surfaceTangent);

        // move ����
        move = Vector3.Project(move.normalized * (move.magnitude - newDistance), surfaceTangent);
      }
    }
    #endregion

    protected virtual void FixedUpdate()
    {
      // �߷�
      velocity += Physics2D.gravity * gravityScale * Time.deltaTime;

      // ��/�� �̵�
      {
        float maxSpeed = isOnGround ? moveSpeed : aerialMoveSpeed;

        // ����
        if (Math.Abs(moveInput) > 0.0f)
        {
          float newSpeed = velocity.x + moveInput * moveAcceleration * Time.deltaTime;

          velocity.x = moveInput > 0.0f ? Math.Max(Math.Min(maxSpeed, newSpeed), velocity.x) : Math.Min(Math.Max(-maxSpeed, newSpeed), velocity.x);
        }
        // ����
        else
        {
          velocity.x = velocity.x > 0.0f ? 
            Math.Max(0.0f, velocity.x - moveAcceleration * Time.deltaTime) :
            Math.Min(0.0f, velocity.x + moveAcceleration * Time.deltaTime);
        }
      }

      ProcessMovement();

      // �� ������ ����
      isOnGround = CheckMoveCollision(body.position, Vector2.down * 0.1f) != null;
    }
  }
}