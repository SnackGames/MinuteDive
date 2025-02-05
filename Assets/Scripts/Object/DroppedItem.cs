using Data;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using Unit;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class DroppedItem : MonoBehaviour
{
  public float CollisionActivationDelay = 1.0f;
  public float launchForce = 300.0f;
  public float launchTorque = 10.0f;
  public Vector2 spawnedPosition = Vector2.zero;
  public Vector2 dropTargetPosition = Vector2.zero;
  [ReadOnly] public int droppedItemUID = -1;
  [ReadOnly] public bool droppedOnGround = false;
  [ReadOnly] protected Rigidbody2D body;
  [ReadOnly] protected Collider2D col;
  private bool reservedPickupItem = false;

  private void Awake()
  {
    body = GetComponent<Rigidbody2D>();
    col = GetComponent<Collider2D>();
    col.isTrigger = true;

    gameObject.layer = LayerMask.NameToLayer("DroppedItem");

    // 최초 스폰 시 충돌 비활성화, 일정 시간 후 충돌 활성화
    col.enabled = false;
    StartCoroutine(ActivateCollider());

    // TODO: dropTargetPosition 위치에 도달할 수 있도록 발사
    body.gravityScale = 1.0f;
    Vector2 launchDirection = new Vector2(0, 1).normalized;
    body.AddForce(launchDirection * launchForce);

    // TODO: dropTargetPosition 위치에 도달했을 때 기준으로 0도가 될 수 있도록 계산
    body.AddTorque(launchTorque, ForceMode2D.Impulse);
  }

  private IEnumerator ActivateCollider()
  {
    yield return new WaitForSeconds(CollisionActivationDelay);

    if (col != null)
    {
      col.enabled = true;
    }
  }

  private void OnTriggerEnter2D(Collider2D collision)
  {
    if (collision.GetComponent<Player>() != null)
    {
      // 이미 바닥에 떨어진 아이템은 즉시 습득
      if (droppedOnGround)
      {
        Player.Get.PickupItem(this);
      }
      // 아직 바닥에 안 떨어진 아이템은 충돌 유지한 채로 바닥에 떨어지면 습득할 수 있도록 예약
      else
      {
        ReservePickupItem(true);
      }
      return;
    }

    // 바닥: 아이템 멈춤
    if(LayerMask.LayerToName(collision.gameObject.layer) == "Wall" && collision.gameObject.transform.position.y < spawnedPosition.y)
    {
      body.velocity = Vector2.zero;
      body.angularVelocity = 0;
      body.bodyType = RigidbodyType2D.Kinematic;
      droppedOnGround = true;
      // 습득 예약된 경우 바로 습득
      if(reservedPickupItem)
      {
        Player.Get.PickupItem(this);
      }
      return;
    }
  }

  private void OnTriggerExit2D(Collider2D collision)
  {
    if (collision.GetComponent<Player>() != null && !droppedOnGround)
    {
      ReservePickupItem(false);
    }
  }

  private void ReservePickupItem(bool reserved)
  {
    reservedPickupItem = reserved;
  }
}