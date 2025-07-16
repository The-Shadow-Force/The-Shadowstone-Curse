using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SimpleCardFlip : MonoBehaviour, IPointerClickHandler
{
    [Header("Card Sides")]
    public GameObject backSide;
    public GameObject frontSide;
    
    [Header("Animation")]
    public float flipDuration = 0.5f;
    public bool canFlip = true;
    
    private bool isFlipped = false;
    private bool isFlipping = false;
    private RectTransform cardTransform;
    
    void Start()
    {
        cardTransform = GetComponent<RectTransform>();
        
        // Đảm bảo trạng thái ban đầu
        if (backSide != null) backSide.SetActive(true);
        if (frontSide != null) frontSide.SetActive(false);
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (canFlip && !isFlipping)
        {
            StartCoroutine(FlipCard());
        }
    }
    
    IEnumerator FlipCard()
    {
        isFlipping = true;
        
        // Phase 1: Rotate to 90 degrees (hide current side)
        float elapsedTime = 0;
        float halfDuration = flipDuration / 2f;
        
        while (elapsedTime < halfDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / halfDuration;
            
            float yRotation = Mathf.Lerp(0, 90, progress);
            cardTransform.rotation = Quaternion.Euler(0, yRotation, 0);
            
            yield return null;
        }
        
        // Switch sides at 90 degrees
        if (isFlipped)
        {
            // Showing back side
            frontSide.SetActive(false);
            backSide.SetActive(true);
        }
        else
        {
            // Showing front side
            backSide.SetActive(false);
            frontSide.SetActive(true);
        }
        
        isFlipped = !isFlipped;
        
        // Phase 2: Rotate from 90 to 0 degrees (show new side)
        elapsedTime = 0;
        
        while (elapsedTime < halfDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / halfDuration;
            
            float yRotation = Mathf.Lerp(90, 0, progress);
            cardTransform.rotation = Quaternion.Euler(0, yRotation, 0);
            
            yield return null;
        }
        
        // Ensure final rotation
        cardTransform.rotation = Quaternion.identity;
        isFlipping = false;
    }
    
    [ContextMenu("Flip Card")]
    public void FlipManually()
    {
        if (!isFlipping)
        {
            StartCoroutine(FlipCard());
        }
    }
    
    [ContextMenu("Reset Card")]
    public void ResetCard()
    {
        isFlipped = false;
        isFlipping = false;
        cardTransform.rotation = Quaternion.identity;
        
        if (backSide != null) backSide.SetActive(true);
        if (frontSide != null) frontSide.SetActive(false);
    }
}