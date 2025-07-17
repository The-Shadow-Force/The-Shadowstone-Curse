using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CardSystem
{
    public class CardFlipButton : MonoBehaviour, IPointerClickHandler
    {
        [Header("Card Components")]
        public GameObject backSide;
        public GameObject frontSide;
        public Image skillIconImg;

        [Header("Audio")]
        public AudioSource audioSource;
        public AudioClip flipCardSound;
        public AudioClip clickSound;
        public AudioClip skillSelectedSound;
        [Range(0f, 1f)]
        public float audioVolume = 0.5f;

        [Header("Animation Settings")]
        [Range(0.1f, 2f)]
        public float flipDuration = 0.6f;
        [Range(0.1f, 3f)]
        public float pauseAfterFlip = 2f;
        [Range(0.1f, 2f)]
        public float fadeOutDuration = 0.5f;
        public bool useGameTimePause = true;

        [Header("Visual Effects")]
        public bool fadeOutAfterDelay = true;
        public AnimationCurve fadeOutCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

        [Header("Debug")]
        public bool debugMode = true;

        private SkillCardPopup popup;
        private SkillData skillData;
        private bool flipped;
        private bool isInitialized;
        private CanvasGroup canvasGroup;
        private bool isCurrentlyFlipping;

        // Static để track card nào đang flip
        private static CardFlipButton currentFlippingCard;

        void Start()
        {
            ValidateComponents();
            SetupAudio();
            SetupCanvasGroup();
            EnsureInitialState();
        }

        void EnsureInitialState()
        {
            flipped = false;
            isCurrentlyFlipping = false;

            if (backSide != null) backSide.SetActive(true);
            if (frontSide != null) frontSide.SetActive(false);

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }

            if (debugMode) Debug.Log($"[CardFlipButton] Initial state ensured for {name}");
        }

        void SetupCanvasGroup()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        void SetupAudio()
        {
            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
                if (audioSource == null)
                {
                    audioSource = gameObject.AddComponent<AudioSource>();
                }
            }

            audioSource.playOnAwake = false;
            audioSource.volume = audioVolume;
            audioSource.spatialBlend = 0f;

            if (debugMode) Debug.Log($"[CardFlipButton] Audio setup completed for {name}");
        }

        public void Init(SkillCardPopup parent, SkillData _data)
        {
            if (debugMode) Debug.Log($"[CardFlipButton] Init called for {name}");

            popup = parent;
            skillData = _data;
            isInitialized = true;

            // Reset states
            flipped = false;
            isCurrentlyFlipping = false;

            // Ensure card is visible
            gameObject.SetActive(true);
            EnsureInitialState();

            // Set skill icon
            SetSkillIcon();

            if (debugMode) Debug.Log($"[CardFlipButton] Init completed for {name} with skill: {skillData?.skillName}");
        }

        void SetSkillIcon()
        {
            if (skillIconImg == null)
            {
                if (debugMode) Debug.LogWarning($"[CardFlipButton] skillIconImg is null for {name}");
                return;
            }

            if (skillData?.skillSprite != null)
            {
                skillIconImg.sprite = skillData.skillSprite;
                skillIconImg.gameObject.SetActive(true);
                if (debugMode) Debug.Log($"[CardFlipButton] Icon set for {skillData.skillName} (from Sprite)");
            }

            if (skillData?.effectPrefab != null)
            {
                var spriteRenderer = skillData.effectPrefab.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    skillIconImg.sprite = spriteRenderer.sprite;
                    skillIconImg.gameObject.SetActive(true);
                    if (debugMode) Debug.Log($"[CardFlipButton] Icon set for {skillData.skillName}");
                }
            }
            else
            {
                skillIconImg.sprite = null;
                if (debugMode) Debug.LogWarning($"[CardFlipButton] No skill data or effect prefab for {name}");
            }
        }

        void ValidateComponents()
        {
            bool hasErrors = false;

            if (backSide == null)
            {
                Debug.LogError($"[CardFlipButton] backSide is null for {name}");
                hasErrors = true;
            }

            if (frontSide == null)
            {
                Debug.LogError($"[CardFlipButton] frontSide is null for {name}");
                hasErrors = true;
            }

            if (skillIconImg == null)
            {
                Debug.LogWarning($"[CardFlipButton] skillIconImg is null for {name}");
            }

            if (debugMode && !hasErrors)
            {
                Debug.Log($"[CardFlipButton] Component validation passed for {name}");
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (debugMode) Debug.Log($"[CardFlipButton] OnPointerClick triggered for {name}");

            if (flipped || isCurrentlyFlipping)
            {
                if (debugMode) Debug.Log($"[CardFlipButton] Card already flipped or flipping, ignoring click");
                return;
            }

            if (currentFlippingCard != null && currentFlippingCard != this)
            {
                if (debugMode) Debug.Log($"[CardFlipButton] Another card is flipping, ignoring click");
                return;
            }

            if (!isInitialized)
            {
                if (debugMode) Debug.LogWarning($"[CardFlipButton] Card not initialized, ignoring click");
                return;
            }

            if (popup == null)
            {
                Debug.LogError($"[CardFlipButton] Popup reference is null for {name}");
                return;
            }

            if (skillData == null)
            {
                Debug.LogError($"[CardFlipButton] SkillData is null for {name}");
                return;
            }

            // Set this card as currently flipping
            currentFlippingCard = this;
            isCurrentlyFlipping = true;

            PlayClickSound();
            StartCoroutine(CompleteFlipSequence());
        }

        void PlayClickSound()
        {
            if (audioSource != null && clickSound != null)
            {
                audioSource.PlayOneShot(clickSound, audioVolume);
                if (debugMode) Debug.Log($"[CardFlipButton] Click sound played for {name}");
            }
        }

        void PlayFlipSound()
        {
            if (audioSource != null && flipCardSound != null)
            {
                audioSource.PlayOneShot(flipCardSound, audioVolume);
                if (debugMode) Debug.Log($"[CardFlipButton] Flip sound played for {name}");
            }
        }

        void PlaySkillSelectedSound()
        {
            if (audioSource != null && skillSelectedSound != null)
            {
                audioSource.PlayOneShot(skillSelectedSound, audioVolume);
                if (debugMode) Debug.Log($"[CardFlipButton] Skill selected sound played for {name}");
            }
        }

        IEnumerator CompleteFlipSequence()
        {
            if (debugMode) Debug.Log($"[CardFlipButton] Starting complete flip sequence for {name}");

            // Disable interaction với TẤT CẢ cards
            DisableAllCardsInteraction();

            // Ensure other cards are forced to backside
            EnsureOnlyThisCardFlips();

            // Step 1: Flip animation
            yield return StartCoroutine(FlipAnimation());

            // Step 2: Pause and display result
            if (debugMode) Debug.Log($"[CardFlipButton] Pausing for {pauseAfterFlip} seconds");

            if (useGameTimePause)
            {
                Time.timeScale = 0f;
                yield return new WaitForSecondsRealtime(pauseAfterFlip);
                Time.timeScale = 1f;
            }
            else
            {
                yield return new WaitForSeconds(pauseAfterFlip);
            }

            // Step 3: Fade out if enabled
            if (fadeOutAfterDelay)
            {
                yield return StartCoroutine(FadeOutAnimation());
            }

            // Step 4: Play selection sound và notify popup
            PlaySkillSelectedSound();

            if (debugMode) Debug.Log($"[CardFlipButton] Notifying popup of skill selection");

            // Clear static reference
            currentFlippingCard = null;

            popup.OnSkillChosen(skillData);
        }

        void EnsureOnlyThisCardFlips()
        {
            var allCards = FindObjectsOfType<CardFlipButton>();
            foreach (var card in allCards)
            {
                if (card != this)
                {
                    card.ForceBackSide();
                }
            }

            if (debugMode) Debug.Log($"[CardFlipButton] Ensured only {name} can flip");
        }

        public void ForceBackSide()
        {
            if (backSide != null) backSide.SetActive(true);
            if (frontSide != null) frontSide.SetActive(false);

            flipped = false;
            isCurrentlyFlipping = false;

            if (canvasGroup != null)
            {
                canvasGroup.interactable = false;
            }

            if (debugMode) Debug.Log($"[CardFlipButton] {name} forced to backside");
        }

        void DisableAllCardsInteraction()
        {
            var allCards = FindObjectsOfType<CardFlipButton>();
            foreach (var card in allCards)
            {
                if (card.canvasGroup != null)
                {
                    card.canvasGroup.interactable = false;
                }
            }
            if (debugMode) Debug.Log($"[CardFlipButton] All cards interaction disabled");
        }

        IEnumerator FlipAnimation()
        {
            if (debugMode) Debug.Log($"[CardFlipButton] Starting flip animation for {name}");

            PlayFlipSound();

            // Tạo hiệu ứng lật đơn giản và mượt mà
            float elapsedTime = 0f;

            while (elapsedTime < flipDuration)
            {
                elapsedTime += Time.unscaledDeltaTime;
                float progress = elapsedTime / flipDuration;

                // Tính toán rotation cho hiệu ứng lật
                float yRotation = Mathf.Lerp(0, 180, progress);
                transform.localRotation = Quaternion.Euler(0, yRotation, 0);

                // Ở giữa animation (90 độ), switch sides
                if (progress >= 0.5f && !flipped)
                {
                    if (debugMode) Debug.Log($"[CardFlipButton] Switching card sides for {name}...");

                    if (backSide != null)
                    {
                        backSide.SetActive(false);
                        if (debugMode) Debug.Log($"[CardFlipButton] BackSide deactivated for {name}");
                    }

                    if (frontSide != null)
                    {
                        frontSide.SetActive(true);
                        if (debugMode) Debug.Log($"[CardFlipButton] FrontSide activated for {name}");
                    }

                    // Đổi hình skillIconImg thành sprite mới của skill
                    if (skillIconImg != null && skillData != null && skillData.skillSprite != null)
                    {
                        skillIconImg.sprite = skillData.skillSprite;
                        skillIconImg.gameObject.SetActive(true);
                        if (debugMode) Debug.Log($"[CardFlipButton] Đã đổi icon skill cho {skillData.skillName}");
                    }

                    flipped = true;
                }

                yield return null;
            }

            // Ensure final state
            transform.localRotation = Quaternion.Euler(0, 180, 0);
            isCurrentlyFlipping = false;

            if (debugMode) Debug.Log($"[CardFlipButton] Flip animation completed for {name}");
        }

        IEnumerator FadeOutAnimation()
        {
            if (debugMode) Debug.Log($"[CardFlipButton] Starting fade out animation for {name}");

            float startAlpha = canvasGroup.alpha;

            for (float t = 0; t < fadeOutDuration; t += Time.unscaledDeltaTime)
            {
                float normalizedTime = t / fadeOutDuration;
                float curveValue = fadeOutCurve.Evaluate(normalizedTime);
                canvasGroup.alpha = startAlpha * curveValue;
                yield return null;
            }

            canvasGroup.alpha = 0f;

            if (debugMode) Debug.Log($"[CardFlipButton] Fade out completed for {name}");
        }

        // Public method để reset trạng thái
        public void ResetCard()
        {
            flipped = false;
            isCurrentlyFlipping = false;
            currentFlippingCard = null;

            transform.localRotation = Quaternion.identity;
            EnsureInitialState();

            if (debugMode) Debug.Log($"[CardFlipButton] {name} reset to initial state");
        }

        [ContextMenu("Debug Card States")]
        public void DebugCardStates()
        {
            Debug.Log($"=== CARD DEBUG INFO for {name} ===");
            Debug.Log($"GameObject Active: {gameObject.activeSelf}");
            Debug.Log($"Component Enabled: {enabled}");
            Debug.Log($"BackSide: {(backSide != null ? backSide.name : "NULL")} - Active: {(backSide != null ? backSide.activeSelf : false)}");
            Debug.Log($"FrontSide: {(frontSide != null ? frontSide.name : "NULL")} - Active: {(frontSide != null ? frontSide.activeSelf : false)}");
            Debug.Log($"SkillIconImg: {(skillIconImg != null ? skillIconImg.name : "NULL")} - Active: {(skillIconImg != null ? skillIconImg.gameObject.activeSelf : false)}");
            Debug.Log($"CanvasGroup Alpha: {(canvasGroup != null ? canvasGroup.alpha : 0f)}");
            Debug.Log($"CanvasGroup Interactable: {(canvasGroup != null ? canvasGroup.interactable : false)}");
            Debug.Log($"Flipped: {flipped}");
            Debug.Log($"IsCurrentlyFlipping: {isCurrentlyFlipping}");
            Debug.Log($"IsInitialized: {isInitialized}");
            Debug.Log($"Transform Rotation: {transform.localEulerAngles}");
        }

        [ContextMenu("Force Make Visible")]
        public void ForceMakeVisible()
        {
            gameObject.SetActive(true);
            EnsureInitialState();
        }

        public void SetFlipDuration(float duration)
        {
            flipDuration = Mathf.Clamp(duration, 0.1f, 2f);
            if (debugMode) Debug.Log($"[CardFlipButton] Flip duration set to {flipDuration} seconds");
        }

        public void SetPauseAfterFlip(float seconds)
        {
            pauseAfterFlip = Mathf.Clamp(seconds, 0.1f, 10f);
            if (debugMode) Debug.Log($"[CardFlipButton] Pause after flip set to {pauseAfterFlip} seconds");
        }

        public void SetFadeOutDuration(float seconds)
        {
            fadeOutDuration = Mathf.Clamp(seconds, 0.1f, 5f);
            if (debugMode) Debug.Log($"[CardFlipButton] Fade out duration set to {fadeOutDuration} seconds");
        }

        public void SetAudioVolume(float volume)
        {
            audioVolume = Mathf.Clamp01(volume);
            if (audioSource != null)
            {
                audioSource.volume = audioVolume;
            }
        }

        public bool IsProperlyConfigured()
        {
            return backSide != null && frontSide != null && isInitialized;
        }

        void OnValidate()
        {
            if (Application.isPlaying) return;

            if (backSide == null)
            {
                Transform backTransform = transform.Find("BackSide");
                if (backTransform == null) backTransform = transform.Find("Back");
                if (backTransform == null) backTransform = transform.Find("CardBack");

                if (backTransform != null)
                {
                    backSide = backTransform.gameObject;
                }
            }

            if (frontSide == null)
            {
                Transform frontTransform = transform.Find("FrontSide");
                if (frontTransform == null) frontTransform = transform.Find("Front");
                if (frontTransform == null) frontTransform = transform.Find("CardFront");

                if (frontTransform != null)
                {
                    frontSide = frontTransform.gameObject;
                }
            }

            if (skillIconImg == null)
            {
                skillIconImg = GetComponentInChildren<Image>();
            }

            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
            }

            flipDuration = Mathf.Clamp(flipDuration, 0.1f, 2f);
            pauseAfterFlip = Mathf.Clamp(pauseAfterFlip, 0.1f, 10f);
            fadeOutDuration = Mathf.Clamp(fadeOutDuration, 0.1f, 5f);
        }
    }
}