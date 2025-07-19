using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardSelectionManager : MonoBehaviour
{
    [Header("Cards")]
    public List<SelectableCard> cards = new List<SelectableCard>();
    
    [Header("UI")]
    public GameObject selectionPanel;
    public Button confirmButton;
    public Text selectedCardText;
    
    [Header("Settings")]
    public bool allowMultipleSelection = false;
    public float cardSpacing = 250f;
    
    [Header("Animation")]
    public float showDelay = 0.2f;
    public float hideDelay = 0.1f;
    
    private SelectableCard selectedCard;
    private bool isSelectionActive = false;
    
    void Start()
    {
        // Setup UI
        if (confirmButton != null)
        {
            confirmButton.onClick.AddListener(ConfirmSelection);
            confirmButton.interactable = false;
        }
        
        // Setup cards
        SetupCards();
        
        // Hide selection panel initially
        if (selectionPanel != null)
        {
            selectionPanel.SetActive(false);
        }
    }
    
    void SetupCards()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i] != null)
            {
                cards[i].Initialize(this, i);
                cards[i].SetSelectable(false);
            }
        }
    }
    
    [ContextMenu("Show Card Selection")]
    public void ShowCardSelection()
    {
        if (isSelectionActive) return;
        
        isSelectionActive = true;
        
        // Show panel
        if (selectionPanel != null)
        {
            selectionPanel.SetActive(true);
        }
        
        // Reset selection
        selectedCard = null;
        UpdateUI();
        
        // Show cards with animation
        StartCoroutine(ShowCardsSequence());
    }
    
    IEnumerator ShowCardsSequence()
    {
        // Position cards
        PositionCards();
        
        // Show cards one by one
        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i] != null)
            {
                cards[i].gameObject.SetActive(true);
                cards[i].SetSelectable(true);
                cards[i].AnimateShow();
                
                yield return new WaitForSeconds(showDelay);
            }
        }
    }
    
    void PositionCards()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i] != null)
            {
                RectTransform cardRect = cards[i].GetComponent<RectTransform>();
                
                // Position cards horizontally
                float xOffset = (i - 1) * cardSpacing; // -1, 0, 1 for 3 cards
                cardRect.anchoredPosition = new Vector2(xOffset, 0);
            }
        }
    }
    
    public void OnCardSelected(SelectableCard card)
    {
        if (!isSelectionActive) return;
        
        // Deselect previous card
        if (selectedCard != null && selectedCard != card)
        {
            selectedCard.SetSelected(false);
        }
        
        // Select new card
        selectedCard = card;
        selectedCard.SetSelected(true);
        
        // Update UI
        UpdateUI();
    }
    
    void UpdateUI()
    {
        if (confirmButton != null)
        {
            confirmButton.interactable = selectedCard != null;
        }
        
        if (selectedCardText != null)
        {
            selectedCardText.text = selectedCard != null ? 
                $"Selected: {selectedCard.cardName}" : 
                "Choose a card";
        }
    }
    
    public void ConfirmSelection()
    {
        if (selectedCard == null || !isSelectionActive) return;
        
        Debug.Log($"🎉 Player selected: {selectedCard.cardName}");
        
        // Process selection
        ProcessCardSelection(selectedCard);
        
        // Hide selection
        StartCoroutine(HideCardsSequence());
    }
    
    void ProcessCardSelection(SelectableCard card)
    {
        // Add your card selection logic here
        // For example:
        // - Give player a reward
        // - Add skill to inventory
        // - Trigger next game phase
        
        Debug.Log($"Processing card: {card.cardName}");
    }
    
    IEnumerator HideCardsSequence()
    {
        isSelectionActive = false;
        
        // Hide non-selected cards first
        for (int i = cards.Count - 1; i >= 0; i--)
        {
            if (cards[i] != null && cards[i] != selectedCard)
            {
                cards[i].AnimateHide();
                yield return new WaitForSeconds(hideDelay);
            }
        }
        
        // Hide selected card last
        if (selectedCard != null)
        {
            selectedCard.AnimateHide();
            yield return new WaitForSeconds(0.5f);
        }
        
        // Hide panel
        if (selectionPanel != null)
        {
            selectionPanel.SetActive(false);
        }
        
        // Reset cards
        ResetCards();
    }
    
    void ResetCards()
    {
        foreach (var card in cards)
        {
            if (card != null)
            {
                card.gameObject.SetActive(false);
                card.SetSelected(false);
                card.SetSelectable(false);
                card.ResetCard();
            }
        }
        
        selectedCard = null;
    }
    
    [ContextMenu("Reset Selection")]
    public void ResetSelection()
    {
        StopAllCoroutines();
        ResetCards();
        isSelectionActive = false;
        
        if (selectionPanel != null)
        {
            selectionPanel.SetActive(false);
        }
    }
}