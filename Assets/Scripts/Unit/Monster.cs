using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Monster : MonoBehaviour
{
  public MonsterData monsterData;
  public GameObject hitParticlePrefab;

  [SerializeField]
  [ReadOnly]
  private int hp = 0;

  private void Awake()
  {
    hp = monsterData.monsterHP;
  }

  public void OnHit()
  {
    // 임시로 사라지며 파티클 소환
    Instantiate(hitParticlePrefab, transform.position, Quaternion.identity);

    if(--hp <= 0)
    {
      Destroy(gameObject);
    }
  }
}