using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;


// Symbol Types used in Game
[System.Serializable]
public enum SymbolType
{
    Fire, Water, Earth, Air,
    Lightning, Ice, Metal, Nature,
    Shadow, Light,
    Wild, Scatter, Bonus, None
}


//Wrapper Class of SymbolTypes with its Image and Weight Defined
[System.Serializable]
public class SymbolData
{
    public SymbolType type;
    public Sprite sprite;
    public int weight;
}


//Symbol Database with few functionalities and complete SymbolDatas
public class SymbolDatabase : MonoBehaviour
{
    public static SymbolDatabase instance;
    public List<SymbolData> symbols;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }


    public List<SymbolType> GetSymbolTypes() //Returns List of SymbolTypes
    {
        List<SymbolType> st = new List<SymbolType>();

        foreach (SymbolData data in symbols) { st.Add(data.type); }
        return st;
    }

    public List<string> GetSymbolNames() //Returns List of SymbolTypes in String
    {
        List<string> st = new List<string>();

        foreach (SymbolData data in symbols) { st.Add(data.type.ToString()); }
        return st;
    }

    public SymbolData GetSymbol(SymbolType symbolType) //Returns SymbolData of particular Symbol Type
    {
        return symbols.Find((x) => x.type == symbolType);
    }

}