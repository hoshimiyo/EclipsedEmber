using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    PlayerStat player;
    private GameObject[] heartContainers;
    private Image[] heartFills;
    public Transform heartsParent;
    public GameObject heartContainerPrefab;
    [SerializeField] Image manaStorage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        heartContainers = new GameObject[PlayerStat.healthCap];
        heartFills = new Image[PlayerStat.currentHealth];
        PlayerStat.instance.onHealthChangedCallback += UpdateHeartsHUD;
        manaStorage.fillAmount = PlayerStat.instance.Mana;

        InstantiateHeartContainers();
        UpdateHeartsHUD();
    }

    // Update is called once per frame
    void Update()
    {
        manaStorage.fillAmount = PlayerStat.instance.mana;
    }

    void SetHeartContainers()
    {
        for (int i = 0; i < heartContainers.Length; i++)
        {
            if (i < PlayerStat.healthCap)
            {
                heartContainers[i].SetActive(true);
            }
            else
            {
                heartContainers[i].SetActive(false);
            }
        }
    }
    void SetFilledHearts()
    {
        for (int i = 0; i < heartFills.Length; i++)
        {
            if (i < PlayerStat.currentHealth)
            {
                heartFills[i].fillAmount = 1;
            }
            else
            {
                heartFills[i].fillAmount = 0;
            }
        }
    }
    void InstantiateHeartContainers()
    {
        for (int i = 0; i < PlayerStat.healthCap; i++)
        {
            GameObject temp = Instantiate(heartContainerPrefab);
            temp.transform.SetParent(heartsParent, false);
            heartContainers[i] = temp;
            heartFills[i] = temp.transform.Find("00_69").GetComponent<Image>();
        }
    }

    void UpdateHeartsHUD()
    {
        SetHeartContainers();
        SetFilledHearts();
    }
}
