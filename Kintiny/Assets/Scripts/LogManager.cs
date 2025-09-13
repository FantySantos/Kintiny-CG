using TMPro;
using UnityEngine;

public class LogManager : MonoBehaviour
{
    public GameObject logEntryPrefab;
    public Transform contentParent;

    void Start()
    {
        AddLogMessage("Sistema iniciado.");
    }

    public void AddLogMessage(string message)
    {
        Debug.Log(message);
        GameObject newLogEntry = Instantiate(logEntryPrefab, contentParent);
        TextMeshProUGUI textComponent = newLogEntry.GetComponentInChildren<TextMeshProUGUI>();

        if (textComponent != null)
            textComponent.text = message;
    }

    public void ClearLogMessages()
    {
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);
    }
}
