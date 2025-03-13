using UnityEngine;

[RequireComponent(typeof(Health))]
public class FragileObject : MonoBehaviour
{
  private void OnTriggerEnter2D(Collider2D collision)
  {
    GetComponent<Health>()?.OnHit(1);
  }

  public void OnHealthChanged(bool isHit, int prevHealth, int health)
  { 
    if (health <= 0)
    {
      GameObject itemUIObject = InventoryManager.GetInventory().CreateDropMoney(1, transform.position, transform.position);

      Destroy(gameObject);
    }
  }
}