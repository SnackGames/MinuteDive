using Data;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class Monster : KinematicObject
{
  public MonsterData monsterData;
  public GameObject hitParticlePrefab;

  protected Health health;

  protected override void Awake()
  {
    base.Awake();

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