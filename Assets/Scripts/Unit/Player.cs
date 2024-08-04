using System;
using System.Collections.Generic;
using Unity.VisualScripting;
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

  [RequireComponent(typeof(Rigidbody2D))]
  public class Player : MonoBehaviour
  {
    [Header("Movement")]
    public float moveSpeed = 5.0f;
    [ReadOnly] public Vector2 moveInput;

    protected Rigidbody2D body;
    protected ContactFilter2D contactFilter;
    protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];

    private void Awake()
    {
      body = GetComponent<Rigidbody2D>();
      body.isKinematic = true;

      contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
      contactFilter.useLayerMask = true;
      contactFilter.useTriggers = false;
    }

    protected virtual void Update()
    {
      ComputeInput();
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
      moveInput = Vector2.zero;

      // 터치 입력
      if (buttonInputs.ContainsKey(ButtonInputType.Left) && buttonInputs[ButtonInputType.Left]) moveInput = Vector2.left;
      else if (buttonInputs.ContainsKey(ButtonInputType.Right) && buttonInputs[ButtonInputType.Right]) moveInput = Vector2.right;
      // 기타 입력
      else moveInput = Input.GetAxisRaw("Horizontal") * Vector2.right;
    }
    #endregion

    protected virtual void FixedUpdate()
    {
      Vector2 velocity = moveInput * moveSpeed;
      float distance = velocity.magnitude * Time.deltaTime;

      int count = body.Cast(velocity, contactFilter, hitBuffer, distance);
      for (int i = 0; i < count; ++i)
      {
        distance = Math.Min(distance, hitBuffer[i].distance);
      }

      // Time.deltaTime이 epsilon이다
      body.position += velocity.normalized * (distance - Time.deltaTime);
    }
  }
}