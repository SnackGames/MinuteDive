using UI;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Monster : MonoBehaviour
{
  [SerializeField]
  [ReadOnly]
  private int hp = 0;

  public MonsterData monsterData;
  public GameObject hitParticlePrefab;
  public UI_HP hpUI;

  private void Awake()
  {
    hp = monsterData.monsterHP;
    if (hpUI != null) { hpUI.SetHP(hp); }
  }

  public void OnHit()
  {
    hpUI.SetHP(--hp);

    // �ӽ÷� ������� ��ƼŬ ��ȯ
    Instantiate(hitParticlePrefab, transform.position, Quaternion.identity);

    if (hp <= 0)
    {
      Destroy(gameObject);
    }
  }
}