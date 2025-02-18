using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
  [DisallowMultipleComponent]
  public class AssetReferenceManager : MonoBehaviour
  {
    static private AssetReferenceManager assetReferenceSingleton;
    static public AssetReferenceManager GetAssetReferences() { return assetReferenceSingleton; }

    [Header("Asset References")]
    public AssetReferenceData assetReferences;

    [Header("UI Links")]
    public TextMeshProUGUI moneyText;
    public GameObject itemGrid;
    public UI_RemainTime remainTime;
    public UI_GameOver gameOver;

    private void Awake()
    {
      assetReferenceSingleton = this;
    }

    public void SetMoney(int money)
    {
      if (moneyText) moneyText.text = $"{money}";
    }

    public void SetItems(int[] items)
    {
      int count = items.Length;
      for (int i = 0; i < count; ++i)
      {
        // #TODO_ITEM 인벤토리 아이템용 프리팹 추가
        // #TODO_ITEM 인벤토리 아이템에 아이템 정보 적용
        // #TODO_ITEM 인벤토리 아이템 클릭 시 아이템 장착/해제
        Image image = itemGrid.transform.GetChild(i).GetComponent<Image>();
        image.color = items[i] != 0 ? Color.red : Color.white;
      }
    }
  }
}