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
    // �ӽ÷� ������� ��ƼŬ ��ȯ
    if (isHit)
      Instantiate(hitParticlePrefab, transform.position, Quaternion.identity);

    if (hp <= 0)
      Destroy(gameObject);
  }
}