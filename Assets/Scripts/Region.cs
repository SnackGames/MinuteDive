using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class RegionEvent : UnityEvent<string> { }

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Region : MonoBehaviour
{
  public string regionName;

  public RegionEvent OnRegionEnter;
  public RegionEvent OnRegionExit;

  protected Rigidbody2D body;
  protected Collider2D regionCollider;

  private void Awake()
  {
    body = GetComponent<Rigidbody2D>();
    body.isKinematic = true;
    regionCollider = GetComponent<Collider2D>();
    regionCollider.isTrigger = true;

    gameObject.layer = LayerMask.NameToLayer("Region");
  }

  private void OnTriggerEnter2D(Collider2D collision)
  {
    OnRegionEnter.Invoke(regionName);
  }

  private void OnTriggerExit2D(Collider2D collision)
  {
    OnRegionExit.Invoke(regionName);
  }
}