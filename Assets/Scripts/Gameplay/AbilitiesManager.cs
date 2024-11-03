using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilitiesManager : MonoBehaviour
{
    [SerializeField] Image ability1;
    [SerializeField] Image abilityImg1;
    [SerializeField] Image cooldown1;
    [SerializeField] TextMeshProUGUI countdownText1;

    [SerializeField] Image ability2;
    [SerializeField] Image abilityImg2;
    [SerializeField] Image cooldown2;
    [SerializeField] TextMeshProUGUI countdownText2;

    [SerializeField] Image ability3;
    [SerializeField] Image abilityImg3;
    [SerializeField] Image cooldown3;
    [SerializeField] TextMeshProUGUI countdownText3;

    [SerializeField] Image ability4;
    [SerializeField] Image abilityImg4;
    [SerializeField] Image cooldown4;
    [SerializeField] TextMeshProUGUI countdownText4;

    [SerializeField] Image ability5;
    [SerializeField] Image abilityImg5;
    [SerializeField] Image cooldown5;
    [SerializeField] TextMeshProUGUI countdownText5;

    //[SerializeField] GameObject ability6;
    //[SerializeField] Image abilityImg6;
    //[SerializeField] Image cooldown6;

    public static AbilitiesManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            if (Instance != this)
            {
                Destroy(Instance);
            }
        }
    }

    public void HighlightAbility1()
    {
        ability1.gameObject.SetActive(true);
        countdownText1.gameObject.SetActive(true);
    }

    public void UnhighlightAbility1()
    {
        ability1.gameObject.SetActive(false);
        countdownText1.gameObject.SetActive(false);
    }
    public void SetCoolDownAbility1(float value, float time)
    {
        cooldown1.fillAmount = value;
        float flooredValue = Mathf.Floor(time * 10) / 10;
        countdownText1.text = flooredValue.ToString("F1");
    }
    public void SetImageAbility1(Sprite sprite, Sprite spriteCooldown)
    {
        abilityImg1.sprite = sprite;
        ability1.sprite = spriteCooldown;
    }


    public void HighlightAbility2()
    {
        ability2.gameObject.SetActive(true);
        countdownText2.gameObject.SetActive(true);
    }
    public void UnhighlightAbility2()
    {
        ability2.gameObject.SetActive(false);
        countdownText2.gameObject.SetActive(false);
    }
    public void SetCoolDownAbility2(float value, float time)
    {
        cooldown2.fillAmount = value;
        float flooredValue = Mathf.Floor(time * 10) / 10;
        countdownText2.text = flooredValue.ToString("F1");
    }
    public void SetImageAbility2(Sprite sprite, Sprite spriteCooldown)
    {
        abilityImg2.sprite = sprite;
        ability2.sprite = spriteCooldown;
    }


    public void HighlightAbility3()
    {
        ability3.gameObject.SetActive(true);
        countdownText3.gameObject.SetActive(true);
    }
    public void UnhighlightAbility3()
    {
        ability3.gameObject.SetActive(false);
        countdownText3.gameObject.SetActive(false);
    }
    public void SetCoolDownAbility3(float value, float time)
    {
        cooldown3.fillAmount = value;
        float flooredValue = Mathf.Floor(time * 10) / 10;
        countdownText3.text = flooredValue.ToString("F1");
    }
    public void SetImageAbility3(Sprite sprite, Sprite spriteCooldown)
    {
        abilityImg3.sprite = sprite;
        ability3.sprite = spriteCooldown;
    }


    public void HighlightAbility4()
    {
        ability4.gameObject.SetActive(true);
        countdownText4.gameObject.SetActive(true);
    }
    public void UnhighlightAbility4()
    {
        ability4.gameObject.SetActive(false);
        countdownText4.gameObject.SetActive(false);
    }
    public void SetCoolDownAbility4(float value, float time)
    {
        cooldown4.fillAmount = value;
        float flooredValue = Mathf.Floor(time * 10) / 10;
        countdownText4.text = flooredValue.ToString("F1");
    }
    public void SetImageAbility4(Sprite sprite, Sprite spriteCooldown)
    {
        abilityImg4.sprite = sprite;
        ability4.sprite = spriteCooldown;
    }


    public void HighlightAbility5()
    {
        ability5.gameObject.SetActive(true);
        countdownText5.gameObject.SetActive(true);
    }
    public void UnhighlightAbility5()
    {
        ability5.gameObject.SetActive(false);
        countdownText5.gameObject.SetActive(false);
    }
    public void SetCoolDownAbility5(float value, float time)
    {
        cooldown5.fillAmount = value;
        float flooredValue = Mathf.Floor(time * 10) / 10;
        countdownText5.text = flooredValue.ToString("F1");
    }
    public void SetImageAbility5(Sprite sprite, Sprite spriteCooldown)
    {
        abilityImg5.sprite = sprite;
        ability5.sprite = spriteCooldown;
    }


    //public void HighlightAbility6()
    //{
    //    ability6.SetActive(true);
    //}
    //public void UnhighlightAbility6()
    //{
    //    ability6.SetActive(false);
    //}
    //public void SetCoolDownAbility6(float value)
    //{
    //    cooldown6.fillAmount = value;
    //}
    //public void SetImageAbility6(Sprite sprite)
    //{
    //    abilityImg6.sprite = sprite;
    //}
}