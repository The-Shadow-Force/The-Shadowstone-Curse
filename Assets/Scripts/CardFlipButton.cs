using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardFlipButton : MonoBehaviour, IPointerClickHandler
{
    public GameObject backSide;
    public GameObject frontSide;
    public Image skillIconImg;

    private SkillCardPopup popup;
    private SkillData skillData;
    private bool flipped;

    public void Init(SkillCardPopup parent, SkillData _data)
    {
        popup = parent;
        skillData = _data;
        flipped = false;

        backSide.SetActive(true);
        frontSide.SetActive(false);
        transform.localRotation = Quaternion.identity;

        skillIconImg.sprite = skillData.effectPrefab != null
            ? skillData.effectPrefab.GetComponent<SpriteRenderer>()?.sprite
            : null; // Gán icon từ prefab hoặc khác
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (flipped) return;
        StartCoroutine(FlipRoutine());
    }

    IEnumerator FlipRoutine()
    {
        flipped = true;
        for (float t = 0; t < 0.3f; t += Time.unscaledDeltaTime)
        {
            float y = Mathf.Lerp(0, 90, t / 0.3f);
            transform.localRotation = Quaternion.Euler(0, y, 0);
            yield return null;
        }

        backSide.SetActive(false);
        frontSide.SetActive(true);

        for (float t = 0; t < 0.3f; t += Time.unscaledDeltaTime)
        {
            float y = Mathf.Lerp(90, 0, t / 0.3f);
            transform.localRotation = Quaternion.Euler(0, y, 0);
            yield return null;
        }

        popup.OnSkillChosen(skillData);
    }
}

