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
    [Header("Input")]
    public float reservePressedInputDuration = 0.3f;
    [ReadOnly] public float moveInput = 0.0f;
    private bool isLookingRight = true;

    [Header("State")]
    [ReadOnly] public PlayerState playerState = PlayerState.Move;

    [Header("Movement")]
    public float moveSpeed = 5.0f;
    public float aerialMoveSpeed = 1.0f;
    public float moveAcceleration = 50.0f;
    public float gravityScale = 1.0f;
    [ReadOnly] public Vector2 velocity = Vector2.zero;
    [ReadOnly] public bool isOnGround = false;
    private ContactFilter2D contactFilter;

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
    private Queue<(ButtonInputType,float)> pressedInputs = new Queue<(ButtonInputType, float)>();
    private float previousAxisInput = 0.0f;

    [VisibleEnum(typeof(ButtonInputType))]
    public void OnButtonDown(int buttonInputType)
    {
      if (buttonInputType < 0 || buttonInputType >= holdingInputs.Length)
      {
        Debug.LogError($"��ư ������ ����: {buttonInputType}");
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
        Debug.LogError($"��ư ���� ����: {buttonInputType}");
        return;
      }

      holdingInputs[buttonInputType] = false;
    }

    protected void PressInput(ButtonInputType buttonInputType)
    {
      const int maxPressedInputStack = 10;

      if (pressedInputs.Count < maxPressedInputStack)
        pressedInputs.Enqueue((buttonInputType, Time.time));
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
      // ������ ���Է� ����
      while (pressedInputs.Count > 0)
      {
        float pressedInputTime = pressedInputs.Peek().Item2;
        if (pressedInputTime >= Time.time - reservePressedInputDuration) break;
        pressedInputs.Dequeue();
      }

      // ���� Ű�� �Է�
      if(Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Space))
        PressInput(ButtonInputType.Attack);

      // �̵� Ű���� �Է�
      float currentAxisInput = Input.GetAxisRaw("Horizontal");
      if (previousAxisInput != currentAxisInput)
      {
        if (previousAxisInput <= 0.0f && currentAxisInput > 0.0f)
          PressInput(ButtonInputType.Right);
        else if (previousAxisInput >= 0.0f && currentAxisInput < 0.0f)
          PressInput(ButtonInputType.Left);
      }
      previousAxisInput = currentAxisInput;
    }
    #endregion

    #region PlayerState
    protected void ProcessPlayerState()
    {
      moveInput = 0.0f;

      switch (playerState)
      {
        case PlayerState.Move: ProcessPlayerState_Move(); break;
      }
    }

    protected void ProcessPlayerState_Move()
    {
      PlayerState nextPlayerState = PlayerState.Move;

      while (pressedInputs.Count > 0)
      {
        ButtonInputType pressedInput = pressedInputs.Peek().Item1;

        // �̵� Ű�� �����Ѵ�
        if (pressedInput == ButtonInputType.Left || pressedInput == ButtonInputType.Right)
        {
          pressedInputs.Dequeue();
          continue;
        }

        // ����
        if(pressedInput == ButtonInputType.Attack)
        {
          // �ӽ÷� �� ���� �������� �ߵ�
          if(isOnGround)
          {
            pressedInputs.Dequeue();
            nextPlayerState = PlayerState.Attack_1;
          }
        }

        break;
      }

      // �̵�
      if (nextPlayerState == PlayerState.Move)
      {
        if (IsHoldingInput(ButtonInputType.Left)) moveInput = -1.0f;
        else if (IsHoldingInput(ButtonInputType.Right)) moveInput = 1.0f;
      }

      playerState = nextPlayerState;
    }
    #endregion

    #region Animation
    void ProcessAnimation()
    {
      anim.SetBool("isRunning", Math.Abs(velocity.x) > 0.0f);
      anim.SetBool("isFalling", !isOnGround);
      anim.SetBool("isAttacking", playerState == PlayerState.Attack_1);

      // ĳ���Ͱ� �ٶ󺸴� ����
      if (isLookingRight) isLookingRight = velocity.x >= 0.0f;
      else isLookingRight = velocity.x > 0.0f;
      sprite.flipX = !isLookingRight;
    }

    private void AnimTrigger_Vibrate()
    {
      // #TODO ���� ����, �ð� �� Ŀ���� �Ǵ� plugin ã����
      Handheld.Vibrate();
    }

    private void AnimTrigger_AnimFinished()
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

    protected void ProcessVelocity()
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
      ProcessVelocity();
      ProcessMovement();

      // �� ������ ����
      isOnGround = CheckMoveCollision(body.position, Vector2.down * 0.1f) != null;
    }
  }
}