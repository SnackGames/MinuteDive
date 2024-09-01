using UI;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Health))]
public class Monster : MonoBehaviour
{
  public MonsterData monsterData;
  public GameObject hitParticlePrefab;

  protected Health health;

  private void Awake()
  {
    health = GetComponent<Health>();
    health.SetHP(monsterData.monsterHP);
  }

  public void OnHPChanged(bool isHit, int prevHp, int hp)
  {
    // 임시로 사라지며 파티클 소환
    if (isHit)
      Instantiate(hitParticlePrefab, transform.position, Quaternion.identity);

    if (hp <= 0)
      Destroy(gameObject);
  }
}