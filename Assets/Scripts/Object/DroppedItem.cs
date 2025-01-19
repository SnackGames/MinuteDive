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
  protected Rigidbody2D body;
  protected Collider2D col;

  private void Awake()
  {
    body = GetComponent<Rigidbody2D>();
    body.isKinematic = true;
    col = GetComponent<Collider2D>();
    col.isTrigger = true;

    gameObject.layer = LayerMask.NameToLayer("DroppedItem");
  }

  private void OnTriggerEnter2D(Collider2D collision)
  {
    if (collision.GetComponent<Player>() != null)
    {
      Player.Get.PickupItem(gameObject);
      Destroy(gameObject);
    }
  }

  private void OnTriggerExit2D(Collider2D collision)
  {
  }
}