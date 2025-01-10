using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillSlot : MonoBehaviour
{
    public Image image;
    public TMP_Text levelText;

    public void SetSlot(Sprite sprite, int level, int maxLevel)
    {
        image.sprite = sprite;
        levelText.text = $"{level}/{maxLevel}";
    }
}
