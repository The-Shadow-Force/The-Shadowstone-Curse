using System.Linq;
using UnityEngine;

namespace CardSystem
{
    public class SkillCardPopup : MonoBehaviour
    {
        [Header("Cards")]
        public CardFlipButton[] cardButtons;

        [Header("Skill Pool")]
        public SkillData[] allSkillData;

        [Header("Timing Settings")]
        [Range(0.1f, 3f)]
        public float globalPauseAfterFlip = 2f;
        [Range(0.1f, 2f)]
        public float globalFadeOutDuration = 0.5f;
        public bool syncTimingWithCards = true;

        [Header("Debug")]
        public bool debugMode = true;

        void Awake()
        {
            // Ensure all cards are found
            if (cardButtons == null || cardButtons.Length == 0)
            {
                cardButtons = GetComponentsInChildren<CardFlipButton>();
                if (debugMode) Debug.Log($"[SkillCardPopup] Auto-found {cardButtons.Length} card buttons");
            }
        }

        void Start()
        {
            // Ensure popup is initially hidden
            gameObject.SetActive(true);
        }

        void OnEnable()
        {
            if (debugMode) Debug.Log("[SkillCardPopup] OnEnable called");

            // Initialize cards properly
            InitializeCards();
            SyncTimingSettings();
        }

        void SyncTimingSettings()
        {
            if (!syncTimingWithCards) return;

            foreach (var card in cardButtons)
            {
                if (card != null)
                {
                    card.SetPauseAfterFlip(globalPauseAfterFlip);
                    card.SetFadeOutDuration(globalFadeOutDuration);
                }
            }

            if (debugMode) Debug.Log("[SkillCardPopup] Timing settings synced with all cards");
        }

        void InitializeCards()
        {
            if (cardButtons == null || cardButtons.Length == 0)
            {
                Debug.LogError("[SkillCardPopup] No card buttons assigned!");
                return;
            }

            if (allSkillData == null || allSkillData.Length == 0)
            {
                Debug.LogWarning("[SkillCardPopup] No skill data available, using fallback");
                InitializeWithFallback();
                return;
            }

            var availableSkills = allSkillData.Where(skill => skill != null).ToArray();

            if (availableSkills.Length == 0)
            {
                Debug.LogError("[SkillCardPopup] No valid skills found!");
                InitializeWithFallback();
                return;
            }

            // Shuffle skills randomly
            var shuffledSkills = availableSkills.OrderBy(x => UnityEngine.Random.value).ToArray();

            // Initialize each card with a random skill
            for (int i = 0; i < cardButtons.Length; i++)
            {
                if (cardButtons[i] == null)
                {
                    Debug.LogError($"[SkillCardPopup] Card button {i} is null!");
                    continue;
                }

                // Use shuffled skill or repeat if not enough skills
                SkillData skillToUse = shuffledSkills[i % shuffledSkills.Length];
                cardButtons[i].Init(this, skillToUse);

                if (debugMode) Debug.Log($"[SkillCardPopup] Card {i} ({cardButtons[i].name}) initialized with skill: {skillToUse.skillName}");
            }

            if (debugMode) Debug.Log("[SkillCardPopup] All cards initialized successfully");
        }

        void InitializeWithFallback()
        {
            var fallbackSkill = ScriptableObject.CreateInstance<SkillData>();
            fallbackSkill.skillName = "Default Skill";

            for (int i = 0; i < cardButtons.Length; i++)
            {
                if (cardButtons[i] != null)
                {
                    cardButtons[i].Init(this, fallbackSkill);
                }
            }

            if (debugMode) Debug.Log("[SkillCardPopup] Initialized with fallback skills");
        }

        public void OnSkillChosen(SkillData data)
        {
            if (debugMode) Debug.Log($"[SkillCardPopup] OnSkillChosen called with: {data?.skillName ?? "null"}");

            if (data == null)
            {
                Debug.LogError("[SkillCardPopup] Skill data is null!");
                return;
            }

            // Level up the skill
            data.LevelUp();

            Debug.Log($"🎉 Bạn đã chọn skill: {data.skillName}");

            // Hide popup
            gameObject.SetActive(false);

            // Ensure game is resumed
            Time.timeScale = 1f;

            if (debugMode) Debug.Log("[SkillCardPopup] Skill selection completed");
        }

        public void ShowPopup()
        {
            if (debugMode) Debug.Log("[SkillCardPopup] Showing popup");
            gameObject.SetActive(true);
        }

        public void HidePopup()
        {
            if (debugMode) Debug.Log("[SkillCardPopup] Hiding popup");
            gameObject.SetActive(false);
        }

        public void UpdateTimingSettings()
        {
            SyncTimingSettings();
        }

        [ContextMenu("Test Popup")]
        public void TestPopup()
        {
            if (debugMode) Debug.Log("[SkillCardPopup] Testing popup initialization");

            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }

            InitializeCards();
        }

        [ContextMenu("Force Show All Cards")]
        public void ForceShowAllCards()
        {
            if (cardButtons == null) return;

            foreach (var card in cardButtons)
            {
                if (card != null)
                {
                    card.gameObject.SetActive(true);
                    card.ForceMakeVisible();
                }
            }

            if (debugMode) Debug.Log("[SkillCardPopup] All cards forced to show");
        }

        [ContextMenu("Debug All Cards")]
        public void DebugAllCards()
        {
            Debug.Log("=== POPUP DEBUG INFO ===");
            Debug.Log($"Popup Active: {gameObject.activeSelf}");
            Debug.Log($"Cards Count: {(cardButtons != null ? cardButtons.Length : 0)}");
            Debug.Log($"Skills Count: {(allSkillData != null ? allSkillData.Length : 0)}");

            if (cardButtons != null)
            {
                for (int i = 0; i < cardButtons.Length; i++)
                {
                    if (cardButtons[i] != null)
                    {
                        Debug.Log($"Card {i}: {cardButtons[i].name} - Active: {cardButtons[i].gameObject.activeSelf}");
                        cardButtons[i].DebugCardStates();
                    }
                    else
                    {
                        Debug.Log($"Card {i}: NULL");
                    }
                }
            }
        }

        [ContextMenu("Reset All Cards")]
        public void ResetAllCards()
        {
            if (cardButtons == null) return;

            foreach (var card in cardButtons)
            {
                if (card != null)
                {
                    card.ResetCard();
                }
            }

            if (debugMode) Debug.Log("[SkillCardPopup] All cards reset");
        }

        void OnValidate()
        {
            if (Application.isPlaying) return;

            if (cardButtons == null || cardButtons.Length == 0)
            {
                cardButtons = GetComponentsInChildren<CardFlipButton>();
            }

            globalPauseAfterFlip = Mathf.Clamp(globalPauseAfterFlip, 0.1f, 10f);
            globalFadeOutDuration = Mathf.Clamp(globalFadeOutDuration, 0.1f, 5f);
        }
    }
}