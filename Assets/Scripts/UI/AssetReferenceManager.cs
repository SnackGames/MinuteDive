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
        Image image = itemGrid.transform.GetChild(i).GetComponent<Image>();
        image.color = items[i] > 0 ? Color.red : Color.white;
      }
    }
  }
}