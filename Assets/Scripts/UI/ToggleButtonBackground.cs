using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HoverButtonBackground : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Sprite spriteA; // Mặc định
    public Sprite spriteB; // Khi hover

    private Image buttonImage;

    void Start()
    {
        buttonImage = GetComponent<Image>();

        if (buttonImage != null && spriteA != null)
        {
            buttonImage.sprite = spriteA;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (buttonImage != null && spriteB != null)
        {
            buttonImage.sprite = spriteB;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (buttonImage != null && spriteA != null)
        {
            buttonImage.sprite = spriteA;
        }
    }
}
