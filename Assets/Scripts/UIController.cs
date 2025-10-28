using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UIController : MonoBehaviour
{
    [Header("Screens")]
    [SerializeField] private GameObject gameScreen;
    [SerializeField] private GameObject helpScreen;

    [Header("UI References")]
    public TMP_Text creditText;
    public TMP_Text winText;
    public Button spinButton;

    [Header("Bet Controls")]
    public TMP_Text betText;
    public Button addValue;
    public Button subValue;

    [Header("Helper Screen Container")]
    [SerializeField] private RectTransform parentContainer;
    [SerializeField] private GameObject containerPrefab;

    private int credits = 1000;
    private int bet = 50;
    private int win = 0;

    // Events
    public static event Action OnSpinStarted;
    public static event Action<int> OnBetChanged;

    private void OnEnable()
    {
        OnSpinStarted += DisableButtons;
        GameController.OnSpinResult += UpdateWin;
    }

    private void OnDisable()
    {
        OnSpinStarted += DisableButtons;
        GameController.OnSpinResult -= UpdateWin;
    }

    void Start()
    {
        SetUpHelperScreen();

        spinButton.onClick.AddListener(HandleSpinPressed);
        UpdateUI();
    }

    private void SetUpHelperScreen()
    {
        SymbolDatabase.instance.symbols.
            ForEach((x) => 
        { 
            GameObject temp = Instantiate(containerPrefab, parentContainer);
            temp.GetComponent<DataContainerController>().SetData(x);
        });
    }

    void HandleSpinPressed() //On Spin Button pressed, this will start changing credit value and start Spin
    {
        if (credits >= bet)
        {
            credits -= bet;
            UpdateUI();
            OnSpinStarted?.Invoke();  // Notify GameController
        }
        else
        {
            Debug.Log("Not enough credits!");
        }
    }

    public void UpdateWin(SpinResultData spd) //Changes values based on SpinResultData spd 
    {
        win = spd.win;
        credits += spd.win + bet;
        UpdateUI();

        EnableButtons();

    }

    private void UpdateUI()
    {
        creditText.text = $"Credits: {credits}";
        betText.text = $"Bet: {bet}";
        winText.text = $"Win: {win}";
    }


    public void AddBetValue()
    {
        if(bet + 10 <= credits)
        {
            bet += 10;
            UpdateUI();
            OnBetChanged?.Invoke(bet);
        }
    }

    public void SubBetValue()
    {
        if(bet > 50)
        {
            bet -= 10;
            UpdateUI();
            OnBetChanged?.Invoke(bet);

        }
    }

    public void DisableButtons()
    {
        spinButton.gameObject.SetActive(false);
        addValue.gameObject.SetActive(false);
        subValue.gameObject.SetActive(false);

    }

    public void EnableButtons()
    {
        spinButton.gameObject.SetActive(true);
        addValue.gameObject.SetActive(true);
        subValue.gameObject.SetActive(true);

    }

    public int GetBetAmount() => bet;

    public void OpenHelpScreen()
    {
        helpScreen.SetActive(true);
        gameScreen.SetActive(false);
    }

    public void CloseHelpScreen()
    {
        gameScreen.SetActive(true);
        helpScreen.SetActive(false);
    }
}
