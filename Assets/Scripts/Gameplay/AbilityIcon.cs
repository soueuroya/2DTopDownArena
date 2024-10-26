using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityIcon : MonoBehaviour
{
    [SerializeField] Image abilityImg;
    [SerializeField] Image cooldownImg;
    [SerializeField] TextMeshProUGUI countdownText;

    public void HighlightAbility()
    {
        gameObject.SetActive(true);
    }

    public void UnhighlightAbility1()
    {
        gameObject.SetActive(false);
    }
    public void SetCoolDownAbility1(float value)
    {
        cooldownImg.fillAmount = value;
        countdownText.text = Mathf.Floor(value).ToString();
    }
    public void SetImageAbility1(Sprite sprite)
    {
        abilityImg.sprite = sprite;
    }
}
