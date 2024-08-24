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
    // �ӽ÷� ������� ��ƼŬ ��ȯ
    Instantiate(hitParticlePrefab, transform.position, Quaternion.identity);

    if(--hp <= 0)
    {
      Destroy(gameObject);
    }
  }
}