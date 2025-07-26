using UnityEngine;
using UnityEngine.UI;

public class SlotInformation : MonoBehaviour
{
    [Header("Визуальные элементы")]
    [SerializeField] private Image iconImage;

    public GameObject ItemPrefab { get; private set; }
    public string ItemName { get; private set; }
    public Sprite ItemSprite { get; private set; }
    public bool IsFree { get; private set; } = true;

    public void InitializeSlot()
    {
        ClearSlot();
    }

    public void FillSlot(string itemName, Sprite itemSprite, GameObject itemPrefab)
    {
        ItemPrefab = itemPrefab;
        ItemName = itemName;
        ItemSprite = itemSprite;
        IsFree = false;

        if (iconImage != null)
        {
            iconImage.sprite = itemSprite;
            iconImage.enabled = true;
        }
    }
    public void ClearSlot()
    {
        ItemPrefab = null;
        ItemName = string.Empty;
        ItemSprite = null;
        IsFree = true;

        if (iconImage != null)
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
        }
    }
}