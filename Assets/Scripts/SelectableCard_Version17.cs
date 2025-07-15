using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectableCard : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Card Info")]
    public string cardName = "Card";
    public Sprite cardIcon;
    public string cardDescription = "This is a card";
    
    [Header("Card Sides")]
    public GameObject backSide;
    public GameObject frontSide;
    public Image iconImage;
    public Text nameText;
    public Text descriptionText;
    
    [Header("Selection Visual")]
    public GameObject selectionBorder;
    public Color selectedColor = Color.yellow;
    public Color normalColor = Color.white;
    public Color hoverColor = Color.gray;
    
    [Header("Animation")]
    public float flipDuration = 0.5f;
    public float scaleDuration = 0.3f;
    public Vector3 hoverScale = new Vector3(1.1f, 1.1f, 1f);
    public Vector3 selectedScale = new Vector3(1.05f, 1.05f, 1f);
    
    private CardSelectionManager manager;
    private bool isSelected = false;
    private bool isSelectable = false;
    private bool isFlipped = false;
    private RectTransform cardTransform;
    private Image cardImage;
    private Vector3 originalScale;
    
    void Awake()
    {
        cardTransform = GetComponent<RectTransform>();
        cardImage = GetComponent<Image>();
        originalScale = cardTransform.localScale;
        
        // Setup initial state
        if (backSide != null) backSide.SetActive(true);
        if (frontSide != null) frontSide.SetActive(false);
        if (selectionBorder != null) selectionBorder.SetActive(false);
        
        // Setup card info
        UpdateCardInfo();
    }
    
    public void Initialize(CardSelectionManager manager, int index)
    {
        this.manager = manager;
        
        // You can assign different card data based on index
        // For example, load from ScriptableObject array
    }
    
    void UpdateCardInfo()
    {
        if (iconImage != null && cardIcon != null)
        {
            iconImage.sprite = cardIcon;
        }
        
        if (nameText != null)
        {
            nameText.text = cardName;
        }
        
        if (descriptionText != null)
        {
            descriptionText.text = cardDescription;
        }
    }
    
    public void SetSelectable(bool selectable)
    {
        isSelectable = selectable;
        
        if (cardImage != null)
        {
            cardImage.color = selectable ? normalColor : Color.gray;
        }
    }
    
    public void SetSelected(bool selected)
    {
        isSelected = selected;
        
        if (selectionBorder != null)
        {
            selectionBorder.SetActive(selected);
        }
        
        if (selected)
        {
            StartCoroutine(ScaleAnimation(selectedScale));
            if (cardImage != null) cardImage.color = selectedColor;
        }
        else
        {
            StartCoroutine(ScaleAnimation(originalScale));
            if (cardImage != null) cardImage.color = normalColor;
        }
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isSelectable) return;
        
        if (!isFlipped)
        {
            // Flip card first
            StartCoroutine(FlipCard());
        }
        else
        {
            // Select card
            if (manager != null)
            {
                manager.OnCardSelected(this);
            }
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isSelectable || isSelected) return;
        
        StartCoroutine(ScaleAnimation(hoverScale));
        if (cardImage != null) cardImage.color = hoverColor;
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isSelectable || isSelected) return;
        
        StartCoroutine(ScaleAnimation(originalScale));
        if (cardImage != null) cardImage.color = normalColor;
    }
    
    IEnumerator FlipCard()
    {
        float elapsedTime = 0;
        float halfDuration = flipDuration / 2f;
        
        // Flip to 90 degrees
        while (elapsedTime < halfDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / halfDuration;
            
            float yRotation = Mathf.Lerp(0, 90, progress);
            cardTransform.rotation = Quaternion.Euler(0, yRotation, 0);
            
            yield return null;
        }
        
        // Switch sides
        if (backSide != null) backSide.SetActive(false);
        if (frontSide != null) frontSide.SetActive(true);
        isFlipped = true;
        
        // Flip back to 0 degrees
        elapsedTime = 0;
        while (elapsedTime < halfDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / halfDuration;
            
            float yRotation = Mathf.Lerp(90, 0, progress);
            cardTransform.rotation = Quaternion.Euler(0, yRotation, 0);
            
            yield return null;
        }
        
        cardTransform.rotation = Quaternion.identity;
    }
    
    IEnumerator ScaleAnimation(Vector3 targetScale)
    {
        Vector3 startScale = cardTransform.localScale;
        float elapsedTime = 0;
        
        while (elapsedTime < scaleDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / scaleDuration;
            
            cardTransform.localScale = Vector3.Lerp(startScale, targetScale, progress);
            yield return null;
        }
        
        cardTransform.localScale = targetScale;
    }
    
    public void AnimateShow()
    {
        // Animate card entrance
        cardTransform.localScale = Vector3.zero;
        StartCoroutine(ScaleAnimation(originalScale));
    }
    
    public void AnimateHide()
    {
        // Animate card exit
        StartCoroutine(ScaleAnimation(Vector3.zero));
    }
    
    public void ResetCard()
    {
        isFlipped = false;
        isSelected = false;
        
        if (backSide != null) backSide.SetActive(true);
        if (frontSide != null) frontSide.SetActive(false);
        if (selectionBorder != null) selectionBorder.SetActive(false);
        
        cardTransform.rotation = Quaternion.identity;
        cardTransform.localScale = originalScale;
        
        if (cardImage != null) cardImage.color = normalColor;
    }
}