using UnityEngine;
using UnityEngine.UI;

public class SlotInformation : MonoBehaviour
{
    public string SlotName;
    public Sprite SloteSprite;
    public bool FreeSlot = true;

    public void UpdateIcon(string Name, Sprite sprite)
    {
        FreeSlot = true;
        SlotName = Name;
        SloteSprite = sprite;

        transform.GetChild(0).GetComponent<Image>().sprite = SloteSprite;

        Debug.Log("1111");
    }
}
