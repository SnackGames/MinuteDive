using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Monster : MonoBehaviour
{
  public MonsterData monsterData;

  public GameObject hitParticlePrefab;

  public void OnHit()
  {
    // 임시로 사라지며 파티클 소환
    Instantiate(hitParticlePrefab, transform.position, Quaternion.identity);
    Destroy(gameObject);
  }
}