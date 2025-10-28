using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class AnalyticsManager : MonoBehaviour
{
    public static AnalyticsManager instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }    
        else
            Destroy(gameObject);

    }
    private void OnEnable()
    {
        GameController.OnSpinResult += LogEvent;
    }

    private void OnDisable()
    {
        GameController.OnSpinResult -= LogEvent;
    }

    public void LogEvent(SpinResultData spinResult) //Logs details to console
    {
        //var data = new { eventName, win, time = System.DateTime.Now.ToString("HH:mm:ss") };
        string json = JsonUtility.ToJson(spinResult);
        Debug.Log(json);
    }
}
