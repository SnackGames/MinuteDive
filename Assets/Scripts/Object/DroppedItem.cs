using Data;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using Unit;
using static UnityEngine.GraphicsBuffer;
using Unity.VisualScripting;
using System.Security.Cryptography;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class DroppedItem : MonoBehaviour
{
  public float collisionActivationDelay = 0.5f;
  public float launchSpeed = 5.0f;
  public float launchRevolutions = 2.0f;
  public float moveToPlayerDelay = 5.0f;
  [ReadOnly] public Vector2 spawnedPosition = Vector2.zero;
  [ReadOnly] public Vector2 dropTargetPosition = Vector2.zero;
  [ReadOnly] public int droppedItemUID = -1;
  [ReadOnly] public bool droppedOnGround = false;
  [ReadOnly] public bool moveToPlayer = false;
  protected Rigidbody2D body;
  protected Collider2D col;
  private bool reservedPickupItem = false;
  private float estimatedTimeToDst = 0f;
  private float elapsedTimeAfterLaunch = 0f;
  private float elapsedTimeAfterLand = 0f;

  private void Awake()
  {
    body = GetComponent<Rigidbody2D>();
    col = GetComponent<Collider2D>();
    col.isTrigger = true;

    gameObject.layer = LayerMask.NameToLayer("DroppedItem");
  }

  private void Start()
  {
    // 최초 스폰 시 충돌 비활성화, 일정 시간 후 충돌 활성화
    col.enabled = false;
    StartCoroutine(ActivateCollider());

    // dropTargetPosition 위치에 도달할 수 있도록 발사
    body.velocity = CalculateLaunchVelocity(spawnedPosition, dropTargetPosition, launchSpeed);

    // dropTargetPosition 위치에 도달했을 때 기준으로 0도가 될 수 있도록 계산
    body.angularVelocity = CalculateLaunchAngularVelocity();
  }

  private IEnumerator ActivateCollider()
  {
    yield return new WaitForSeconds(collisionActivationDelay);

    if (col != null)
    {
      col.enabled = true;
    }
  }

  private void FixedUpdate()
  {
    if (!droppedOnGround)
    {
      elapsedTimeAfterLaunch += Time.deltaTime;

      // 목표 지점 도착 예상 시간 도달: 아이템 멈춤
      if (elapsedTimeAfterLaunch > estimatedTimeToDst)
      {
        droppedOnGround = true;
        transform.position = dropTargetPosition;
        transform.rotation = Quaternion.identity;
        body.velocity = Vector2.zero;
        body.angularVelocity = 0;
        body.bodyType = RigidbodyType2D.Kinematic;
        // 습득 예약된 경우 바로 습득
        if (reservedPickupItem)
        {
          InventoryManager.GetInventory().PickupDropItem(this);
        }
        return;
      }
    }
    else if (!moveToPlayer)
    {
      // 드랍된 지 일정 시간이 지나면 플레이어에게 자동으로 날아감.
      elapsedTimeAfterLand += Time.deltaTime;
      if(elapsedTimeAfterLand > moveToPlayerDelay)
      {
        moveToPlayer = true;
      }
    }

    if (moveToPlayer)
    {
      float minSpeed = 1.5f;
      float maxSpeed = 30.0f;
      float distanceToPlayer = Vector2.Distance(transform.position, Player.Get.transform.position);
      float speed = Mathf.Lerp(minSpeed, maxSpeed, distanceToPlayer / 10.0f);
      transform.position = Vector2.MoveTowards(transform.position, Player.Get.transform.position, speed * Time.deltaTime);
    }
  }

  private void OnTriggerEnter2D(Collider2D collision)
  {
    if (collision.GetComponent<Player>() != null)
    {
      // 이미 바닥에 떨어진 아이템은 즉시 습득
      if (droppedOnGround)
      {
        InventoryManager.GetInventory().PickupDropItem(this);
      }
      // 아직 바닥에 안 떨어진 아이템은 충돌 유지한 채로 바닥에 떨어지면 습득할 수 있도록 예약
      else
      {
        ReservePickupItem(true);
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

  private Vector2 CalculateLaunchVelocity(Vector2 src, Vector2 dst, float launchSpeed)
  {
    if (src.y != dst.y)
      return Vector2.up * launchSpeed;

    float g = Mathf.Abs(Physics2D.gravity.y) * body.gravityScale;
    float dx = dst.x - src.x;
    float dy = dst.y - src.y;

    estimatedTimeToDst = 2 * launchSpeed / g;
    float vy = launchSpeed;
    float vx = dx / estimatedTimeToDst;

    Vector2 result = new Vector2(vx, vy);
    return result;
  }

  private float CalculateLaunchAngularVelocity()
  {
    // launchRevolutions 바퀴 회전
    return (launchRevolutions * 360f) / estimatedTimeToDst;
  }
}