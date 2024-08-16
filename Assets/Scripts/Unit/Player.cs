using PlayerState;
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
  public enum PlayerStateType
  {
    Move,
    Attack,
    FallAttack,
    Dash
  }

  [RequireComponent(typeof(Rigidbody2D),typeof(Animator),typeof(SpriteRenderer))]
  public class Player : MonoBehaviour
  {
    [Header("Input")]
    public float reservePressedInputDuration = 0.3f;
    [ReadOnly] public float moveInput = 0.0f;
    [ReadOnly] public bool isLookingRight = true;

    [Header("State")]
    [ReadOnly] public PlayerStateType playerState = PlayerStateType.Move;
    [ReadOnly] public PlayerStateBase playerStateBehaviour;

    [Header("Movement")]
    public float moveSpeed = 5.0f;
    public float aerialMoveSpeed = 1.0f;
    public float attackMoveSpeed = 1.5f;
    public float fallAttackSpeed = 3.0f;
    public float dashSpeed = 12.0f;
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

      // �̵�
      moveInput = 0.0f;
      if (IsHoldingInput(ButtonInputType.Left)) moveInput = -1.0f;
      else if (IsHoldingInput(ButtonInputType.Right)) moveInput = 1.0f;
    }
    #endregion

    #region Animation
    [ReadOnly] public bool isReservedDashDirectionRight = false;

    public void SetLookingDirection(bool right)
    {
      sprite.flipX = !right;
      isLookingRight = right;
    }

    public void AnimTrigger_EnableMoveInput(int enable) => playerStateBehaviour.AnimTrigger_EnableMoveInput(enable > 0);
    public void AnimTrigger_EnableAttackInput(int enable) => playerStateBehaviour.AnimTrigger_EnableAttackInput(enable > 0);

    public void AnimTrigger_Vibrate()
    {
      // #TODO ���� ����, �ð� �� Ŀ���� �Ǵ� plugin ã����
      Handheld.Vibrate();
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
      float maxSpeed = isOnGround ? moveSpeed : aerialMoveSpeed;
      switch (playerState)
      {
        case PlayerStateType.Move:
          {
            // ����
            if (Math.Abs(moveInput) > 0.0f)
            {
              float acceleration = moveAcceleration;
              // ������ȯ�� ���ӵ��� ��� �ش�
              if (moveInput * velocity.x < 0.0f) acceleration *= 3.0f;

              float newSpeed = velocity.x + moveInput * acceleration * Time.deltaTime;
              velocity.x = moveInput > 0.0f ? Math.Max(Math.Min(maxSpeed, newSpeed), velocity.x) : Math.Min(Math.Max(-maxSpeed, newSpeed), velocity.x);
            }
            // ����
            else
            {
              velocity.x = velocity.x > 0.0f ?
                Math.Max(0.0f, velocity.x - moveAcceleration * Time.deltaTime) :
                Math.Min(0.0f, velocity.x + moveAcceleration * Time.deltaTime);
            }
          } break;

        case PlayerStateType.Attack:
          {
            // ����
            velocity.x = velocity.x > 0.0f ?
                Math.Max(0.0f, velocity.x - moveAcceleration * Time.deltaTime) :
                Math.Min(0.0f, velocity.x + moveAcceleration * Time.deltaTime);
          } break;

        case PlayerStateType.FallAttack:
          {
            velocity = new Vector2(0.0f, -fallAttackSpeed);
          } break;

        case PlayerStateType.Dash:
          {
            // �ӵ� 3��� ����
            velocity.x = velocity.x > 0.0f ?
                Math.Max(0.0f, velocity.x - 3.0f * moveAcceleration * Time.deltaTime) :
                Math.Min(0.0f, velocity.x + 3.0f * moveAcceleration * Time.deltaTime);
          } break;
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

        RaycastHit2D? hit = CheckMoveCollision(body.position, move + move.normalized * epsilon);
        if (hit == null)
        {
          body.position += move;
          break;
        }

        float newDistance = hit.Value.distance - epsilon;

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