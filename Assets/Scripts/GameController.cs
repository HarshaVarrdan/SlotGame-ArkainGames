using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Linq;
using System.Globalization;
using System.CodeDom.Compiler;

public class GameController : MonoBehaviour
{
    public ReelController[] reels;
    public UIController uiController;

    private float totalBets = 0f;
    private float totalWins = 0f;
    private int currentWin = 0;
    private int occurenceOfScatter = 0;
    SymbolGridData[,] symbolsGrid = new SymbolGridData[4, 5];
    SpinResultData spinResultData = new SpinResultData();

    public static event Action<SpinResultData> OnSpinResult;
    
    [SerializeField] List<WinData> windata = new List<WinData>();

    void OnEnable()
    {
        UIController.OnSpinStarted += HandleSpinStart;
    }

    void OnDisable()
    {
        UIController.OnSpinStarted -= HandleSpinStart;
    }

    void HandleSpinStart()
    {
        StartCoroutine(SpinRoutine());
    }

    IEnumerator SpinRoutine() //Initializes values, Starts spin, and calculates win.
    {
        Debug.Log("Spin Started");

        int bet = uiController.GetBetAmount();
        totalBets += bet;

        windata.Clear();
        occurenceOfScatter = 0;

        foreach (var reel in reels)
            StartCoroutine(reel.Spin(1.2f));

        yield return new WaitForSeconds(1.3f);

        EvaluateResult(bet);

        spinResultData.eventName = "Spin_Result";
        spinResultData.win = currentWin;
        spinResultData.symbols = SymbolDatabase.instance.GetSymbolNames();
        spinResultData.windata = windata;

        OnSpinResult?.Invoke(spinResultData);

        float rtp = (totalWins / totalBets) * 100f;
        Debug.Log($" Total Wins : {totalWins} / Total Bets : {totalBets}\nCurrent RTP: {rtp:F2}%");
    }

    void EvaluateResult(int bet) //Gets result by checking each elements and its neighbours from the grid created
    {

        for (int i = 0; i < reels.Length; i++)
        {
            ReelController reel = reels[i];

            List<SymbolType> symbols = reel.GetAllSymbolsInReel();

            for (int j = 0; j < symbols.Count; j++)
            {
                SymbolType symbol = symbols[j];

                if (symbol == SymbolType.Scatter) occurenceOfScatter++;

                symbolsGrid[j, i] = new SymbolGridData()
                {
                    symbol = symbol,
                };
            }
        }

        PrintGrid(symbolsGrid);

        CheckConsecutiveMatches();

        windata.ForEach((x) => Debug.Log($"{x.symbol} is in for {x.count}"));

        currentWin = (windata.Count > 0) ? (int)(((95 * bet) / 100) * (windata.Count/1.5f)) : 0;
        totalWins += currentWin;
    }

    //Fire        Wild        Earth       Earth       Wild        
    
    //Air         Fire        Wild        Fire        Fire        
    
    //Fire        Fire        Wild        Air         Fire        
    
    //Wild        Air         Wild        Earth       Fire        


    void CheckConsecutiveMatches() //Checks the neighbours
    {
        int row = symbolsGrid.GetLength(0);
        int col = symbolsGrid.GetLength(1);
   
        WinData win;

        for (int i = 0; i < row; i++) 
        { 
            for(int j=0; j < col; j++)
            {
                
                if(col - j > 2 && !symbolsGrid[i,j].isHorizonalChecked && (symbolsGrid[i, j].symbol != SymbolType.Wild || symbolsGrid[i, j].symbol != SymbolType.Scatter))
                {
                    win = CheckHorizontalMatches(i, j, row, col);
                    Debug.Log($"Symbol : {win.symbol}, occurence : {win.count}");

                    if (win.count >= 3)
                    {
                        windata.Add(win);
                        Debug.Log($"Symbol : {win.symbol}, occurence : {win.count} is added to WinData{windata.Count}");
                    }
                }
                if (row - i > 2 && !symbolsGrid[i, j].isVerticalChecked && (symbolsGrid[i, j].symbol != SymbolType.Wild || symbolsGrid[i, j].symbol != SymbolType.Scatter))
                {
                    win = CheckVerticalMatches(i, j, row, col);
                        Debug.Log($"Symbol : {win.symbol}, occurence : {win.count}");
                    if (win.count >= 3)
                    {
                        windata.Add(win);
                        Debug.Log($"Symbol : {win.symbol}, occurence : {win.count} is added to WinData{windata.Count}");
                    }
                }
            }
        }
    }

    WinData CheckVerticalMatches(int a, int j, int row, int col) // Checks Vertical Neighbours
    {
        int wild_a = -1, wild_b = -1;

        SymbolType currentSymbol = symbolsGrid[a, j].symbol;
        SymbolType previousSymbol = (a > 0) ? ((symbolsGrid[a-1,j].symbol == SymbolType.Wild && !symbolsGrid[a - 1, j].isUsed) ? SymbolType.Wild : SymbolType.None) : SymbolType.None;

        
        int occurence = 0;

        if(previousSymbol == SymbolType.Wild)
        {
            previousSymbol = currentSymbol;
            occurence++;
        }


        Debug.Log($"Checking for {currentSymbol}");
        for(int i = a; i < row; i++)
        {
            if(previousSymbol == SymbolType.None)
            {
                previousSymbol = symbolsGrid[i, j].symbol;
                symbolsGrid[i, j].isVerticalChecked = true;

                occurence++;
                continue;
            }
            
            currentSymbol = symbolsGrid[i, j].symbol;

            if (currentSymbol == SymbolType.Wild)
            {
                currentSymbol = previousSymbol;
                wild_a = i;
                wild_b = j;
            }

            Debug.Log($"Vertical : {currentSymbol} [{i},{j}] " + ((currentSymbol == previousSymbol) ? "==" : "!=") + $" {previousSymbol} Occurence = {occurence}");
            
            if (currentSymbol == previousSymbol)
            {
                symbolsGrid[i, j].isVerticalChecked = true;
                occurence++;
            }
            else
            {
                break;
            }
        }
        
        if(occurence >= 3 && (wild_a >=0 && wild_b >= 0))
        {
            symbolsGrid[wild_a,wild_b].isUsed = true;
            Debug.Log($"Wild in [{wild_a},{wild_b} is used]");
        }

        return new WinData() {symbol = symbolsGrid[a, j].symbol, count = occurence}; 
    }

    WinData CheckHorizontalMatches(int i, int b, int row, int col) // Checks Horizontal Neighbours
    {

        int wild_a = -1, wild_b = -1;

        SymbolType currentSymbol = symbolsGrid[i, b].symbol;
        SymbolType previousSymbol = (b > 0) ? ((symbolsGrid[i, b-1].symbol == SymbolType.Wild && !symbolsGrid[i, b - 1].isUsed) ? SymbolType.Wild : SymbolType.None) : SymbolType.None;

        int occurence = 0;

        if (previousSymbol == SymbolType.Wild)
        {
            previousSymbol = currentSymbol;
            occurence++;
        }


        for (int j = b; j < col; j++)
        {
            if (previousSymbol == SymbolType.None)
            {
                previousSymbol = symbolsGrid[i, j].symbol;
                symbolsGrid[i, j].isHorizonalChecked = true;

                occurence++;
                continue;
            }

            currentSymbol = symbolsGrid[i, j].symbol;

            if (currentSymbol == SymbolType.Wild)
            {
                currentSymbol = previousSymbol;
                wild_a = i;
                wild_b = j;
            }


            Debug.Log($"Horizontal : {currentSymbol} [{i},{j}] " + ((currentSymbol == previousSymbol) ? "==" : "!=") + $" {previousSymbol} Occurence = {occurence}");
            
            if (currentSymbol == previousSymbol)
            {
                symbolsGrid[i, j].isHorizonalChecked = true;
                occurence++;
            }
            else
            {
                break;
            }
        }

        if (occurence >= 3 && (wild_a >= 0 && wild_b >= 0))
        {
            symbolsGrid[wild_a, wild_b].isUsed = true;
        }

        return new WinData() { symbol = symbolsGrid[i, b].symbol, count = occurence };
    }

    
    void PrintGrid(SymbolGridData[,] grid) //Prints the created grid to console
    {
        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);

        string gridString = "\n";

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                gridString += grid[row, col].symbol.ToString().PadRight(10) + $"  {row},{col}";
            }
            gridString += "\n";
        }

        Debug.Log(gridString);
    }

}

[System.Serializable]
public struct SpinResultData
{
    public string eventName;
    public int win;
    public List<string> symbols;
    public List<WinData> windata;
}

[System.Serializable]
public struct WinData
{
    public SymbolType symbol;
    public int count;
}

[System.Serializable]
public class SymbolGridData
{
    public SymbolType symbol;
    public bool isHorizonalChecked = false;
    public bool isVerticalChecked = false;
    public bool isUsed = false;
}