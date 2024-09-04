using UnityEngine;

public class FragileObject : MonoBehaviour
{
  public void OnHealthChanged(bool isHit, int prevHealth, int health)
  { 
    if (health <= 0)
    {
      InventoryManager.GetInventory()?.AddMoney(1);

      Destroy(gameObject);
    }
  }
}