using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using CardSystem;

public class CardSystemDebugger : MonoBehaviour
{
    [Header("Debug Settings")]
    public bool enableDebugLogs = true;
    public bool showClickableAreas = true;
    public Color debugColor = Color.red;
    public bool autoFixIssues = true;

    [Header("Runtime Info")]
    [SerializeField] private int foundCards = 0;
    [SerializeField] private int activeCards = 0;
    [SerializeField] private bool canvasValid = false;
    [SerializeField] private bool eventSystemValid = false;

    void Start()
    {
        if (enableDebugLogs)
        {
            DebugSystemSetup();
        }
    }

    void Update()
    {
        // Update runtime info
        UpdateRuntimeInfo();
    }

    void UpdateRuntimeInfo()
    {
        var cardButtons = FindObjectsOfType<CardFlipButton>();
        foundCards = cardButtons.Length;
        activeCards = 0;

        foreach (var card in cardButtons)
        {
            if (card.gameObject.activeSelf)
                activeCards++;
        }

        var canvas = FindObjectOfType<Canvas>();
        canvasValid = canvas != null && canvas.gameObject.activeSelf;

        var eventSystem = FindObjectOfType<EventSystem>();
        eventSystemValid = eventSystem != null && eventSystem.enabled;
    }

    void DebugSystemSetup()
    {
        Debug.Log("=== Card System Debug Info ===");

        // Check Canvas
        var canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            Debug.Log($"✅ Canvas found: {canvas.name}");
            Debug.Log($"  - Active: {canvas.gameObject.activeSelf}");
            Debug.Log($"  - Scale: {canvas.transform.localScale}");
            Debug.Log($"  - Render Mode: {canvas.renderMode}");

            // Fix canvas scale if needed
            if (autoFixIssues && canvas.transform.localScale == Vector3.zero)
            {
                canvas.transform.localScale = Vector3.one;
                Debug.Log("  ✅ Fixed Canvas scale from 0 to 1");
            }

            var raycaster = canvas.GetComponent<GraphicRaycaster>();
            Debug.Log($"  - GraphicRaycaster: {(raycaster != null ? "Found" : "Missing")}");

            if (raycaster != null)
            {
                Debug.Log($"  - Raycaster enabled: {raycaster.enabled}");
            }
        }
        else
        {
            Debug.LogError("❌ No Canvas found!");
        }

        // Check EventSystem
        var eventSystem = FindObjectOfType<EventSystem>();
        if (eventSystem != null)
        {
            Debug.Log($"✅ EventSystem found: {eventSystem.name}");
            Debug.Log($"  - Enabled: {eventSystem.enabled}");
        }
        else
        {
            Debug.LogError("❌ No EventSystem found!");
        }

        // Check Cards
        var cardButtons = FindObjectsOfType<CardFlipButton>();
        Debug.Log($"🎴 Found {cardButtons.Length} CardFlipButton components");

        foreach (var card in cardButtons)
        {
            Debug.Log($"  Card: {card.name}");
            Debug.Log($"    - GameObject active: {card.gameObject.activeSelf}");
            Debug.Log($"    - Component enabled: {card.enabled}");
            Debug.Log($"    - Properly configured: {card.IsProperlyConfigured()}");

            // Fix card visibility if needed
            if (autoFixIssues && !card.gameObject.activeSelf)
            {
                card.gameObject.SetActive(true);
                Debug.Log($"    ✅ Fixed: Made {card.name} visible");
            }
        }

        // Check SkillCardPopup
        var popup = FindObjectOfType<SkillCardPopup>();
        if (popup != null)
        {
            Debug.Log($"🃏 SkillCardPopup found: {popup.name}");
            Debug.Log($"  - Active: {popup.gameObject.activeSelf}");
            Debug.Log($"  - Cards assigned: {(popup.cardButtons != null ? popup.cardButtons.Length : 0)}");
            Debug.Log($"  - Skills assigned: {(popup.allSkillData != null ? popup.allSkillData.Length : 0)}");
        }
        else
        {
            Debug.LogError("❌ No SkillCardPopup found!");
        }

        Debug.Log("=== End Debug Info ===");
    }

    [ContextMenu("Full System Debug")]
    public void FullSystemDebug()
    {
        DebugSystemSetup();

        // Additional deep debugging
        var popup = FindObjectOfType<SkillCardPopup>();
        if (popup != null)
        {
            popup.DebugAllCards();
        }
    }

    [ContextMenu("Fix All Issues")]
    public void FixAllIssues()
    {
        Debug.Log("🔧 Starting to fix all issues...");

        // Fix Canvas
        var canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            canvas.transform.localScale = Vector3.one;
            canvas.gameObject.SetActive(true);
            Debug.Log("✅ Canvas fixed");
        }

        // Fix Cards
        var cardButtons = FindObjectsOfType<CardFlipButton>();
        foreach (var card in cardButtons)
        {
            card.gameObject.SetActive(true);
            card.ForceMakeVisible();
        }
        Debug.Log($"✅ {cardButtons.Length} cards fixed");

        // Fix Popup
        var popup = FindObjectOfType<SkillCardPopup>();
        if (popup != null)
        {
            popup.ForceShowAllCards();
            Debug.Log("✅ Popup fixed");
        }

        Debug.Log("✅ All issues fixed!");
    }

    [ContextMenu("Test Card System")]
    public void TestCardSystem()
    {
        Debug.Log("🧪 Testing card system...");

        var popup = FindObjectOfType<SkillCardPopup>();
        if (popup != null)
        {
            popup.TestPopup();
            Debug.Log("✅ Test completed");
        }
        else
        {
            Debug.LogError("❌ No SkillCardPopup found for testing!");
        }
    }

    [ContextMenu("Reset Card System")]
    public void ResetCardSystem()
    {
        Debug.Log("🔄 Resetting card system...");

        var popup = FindObjectOfType<SkillCardPopup>();
        if (popup != null)
        {
            popup.ResetAllCards();
            Debug.Log("✅ All cards reset");
        }

        var cardButtons = FindObjectsOfType<CardFlipButton>();
        foreach (var card in cardButtons)
        {
            card.ResetCard();
        }

        Debug.Log("✅ Card system reset completed");
    }

    [ContextMenu("Performance Check")]
    public void PerformanceCheck()
    {
        Debug.Log("⚡ Performance Check:");
        Debug.Log($"  - Cards found: {foundCards}");
        Debug.Log($"  - Active cards: {activeCards}");
        Debug.Log($"  - Canvas valid: {canvasValid}");
        Debug.Log($"  - EventSystem valid: {eventSystemValid}");
        Debug.Log($"  - Frame rate: {1f / Time.unscaledDeltaTime:F1} FPS");
    }

    void OnDrawGizmos()
    {
        if (!showClickableAreas) return;

        var cardButtons = FindObjectsOfType<CardFlipButton>();
        Gizmos.color = debugColor;

        foreach (var card in cardButtons)
        {
            if (!card.gameObject.activeSelf) continue;

            var rectTransform = card.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                // Convert to world space
                Vector3[] corners = new Vector3[4];
                rectTransform.GetWorldCorners(corners);

                Vector3 center = (corners[0] + corners[2]) / 2f;
                Vector3 size = corners[2] - corners[0];

                Gizmos.DrawWireCube(center, size);

                // Draw card name
#if UNITY_EDITOR
                UnityEditor.Handles.Label(center, card.name);
#endif
            }
        }
    }

    void OnGUI()
    {
        if (!enableDebugLogs) return;

        // Runtime debug info
        GUILayout.BeginArea(new Rect(10, 10, 300, 150));
        GUILayout.Label("Card System Debug Info:");
        GUILayout.Label($"Cards Found: {foundCards}");
        GUILayout.Label($"Active Cards: {activeCards}");
        GUILayout.Label($"Canvas Valid: {canvasValid}");
        GUILayout.Label($"EventSystem Valid: {eventSystemValid}");

        if (GUILayout.Button("Fix All Issues"))
        {
            FixAllIssues();
        }

        if (GUILayout.Button("Test System"))
        {
            TestCardSystem();
        }

        GUILayout.EndArea();
    }
}