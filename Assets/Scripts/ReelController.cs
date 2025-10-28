using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ReelController : MonoBehaviour
{

    [Header("Symbol Slots (Top to Bottom)")]
    public Image[] symbolSlots; // 4 image slots on reel

    [Header("Database & Weights")]
    public SymbolDatabase database;
    public List<SymbolData> symbols = new();

    [Header("Center Reel Settings")]
    [SerializeField] private bool isCenterReel = false; 

    public List<SymbolType> currentSymbols = new();
    private List<SymbolType> weightedPool = new();


    void Start()
    {
        RandomizeSymbols();
        BuildWeightedPool();
    }

    public void RandomizeSymbols() //Initializes Symbols in Each Placeholders of Reel randomly.
    { 
        currentSymbols.Clear(); 
        for (int i = 0; i < symbolSlots.Length; i++) 
        { 
            SymbolData symbol = database.symbols[Random.Range(0, database.symbols.Count)]; 
            currentSymbols.Add(symbol.type); 
            symbolSlots[i].sprite = symbol.sprite; 
        } 
    }

    public IEnumerator Spin(float duration = 1f) //Starts the spin process
    {
        float timer = 0f;
        while (timer < duration)
        {
            GenerateSymbols();
            yield return new WaitForSeconds(0.1f);
            timer += 0.1f;
        }

        if (isCenterReel)
        {
            if (currentSymbols.Contains(SymbolType.Wild))
            {
                for (int i = 0; i < currentSymbols.Count; i++)
                {
                    currentSymbols[i] = SymbolType.Wild;
                    SetSlotToSpecificSymbol(SymbolType.Wild, i);
                }
            }
        }
    }


    public void GenerateSymbols() //Initializes Symbols in Each Placeholders of Reel based on each elements weight.
    {
        currentSymbols.Clear();

        SymbolType? previousSymbol = null;

        for (int i = 0; i < symbolSlots.Length; i++)
        {
            SymbolType randomSymbol;

            // Small bias: 25% chance to repeat previous symbol
            if (previousSymbol != null && Random.value < 0.25f)
                randomSymbol = previousSymbol.Value;
            else
                randomSymbol = weightedPool[Random.Range(0, weightedPool.Count)];

            currentSymbols.Add(randomSymbol);
            previousSymbol = randomSymbol;

            // Update UI Image
            SetSlotToSpecificSymbol(randomSymbol, i);
            
        }
    }


    private void BuildWeightedPool() //Builds a list of symbols based on its weight
    {
        weightedPool.Clear();

        symbols = SymbolDatabase.instance.symbols;

        foreach (var entry in symbols)
        {
            for (int i = 0; i < entry.weight; i++)
                weightedPool.Add(entry.type);
        }

        Debug.Log($"[{gameObject.name}] Weighted pool built with {weightedPool.Count} entries");
    }

    private void SetSlotToSpecificSymbol(SymbolType st, int slotIndex)
    {
        SymbolData symbolData = database.GetSymbol(st);
        if (symbolData != null)
            symbolSlots[slotIndex].sprite = symbolData.sprite;
    }

    public List<SymbolType> GetAllSymbolsInReel() //Returns List of reel in this particular reel
    {
        return currentSymbols;
    }
}
