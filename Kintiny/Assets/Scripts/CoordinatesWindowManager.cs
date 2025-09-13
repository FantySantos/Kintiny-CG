using UnityEngine;
using TMPro;

public class CoordinatesWindowManager : MonoBehaviour
{
    [Header("Inputs da Coordenada 1")]
    public TMP_InputField x1_Input;
    public TMP_InputField y1_Input;

    [Header("Inputs da Coordenada 2")]
    public TMP_InputField x2_Input;
    public TMP_InputField y2_Input;

    private Vector2Int[] coordenadas = new Vector2Int[2];

    public Vector2Int[] GetVector2Ints()
    {
        if (ProcessarCoordenadas())
            return coordenadas;
        return null;
    }

    public bool ProcessarCoordenadas()
    {
        if (!ValidarTodosInputs())
        {
            Debug.LogWarning("Todas as coordenadas devem estar preenchidos!");
            return false;
        }

        coordenadas[0] = LerCoordenada(x1_Input, y1_Input);
        coordenadas[1] = LerCoordenada(x2_Input, y2_Input);

        for (int i = 0; i < coordenadas.Length; i++)
        {
            Debug.Log($"Coordenada {i + 1}: ({coordenadas[i].x}, {coordenadas[i].y})");
        }

        return true;
    }

    private bool ValidarTodosInputs()
    {
        return ValidarInput(x1_Input) && ValidarInput(y1_Input) &&
               ValidarInput(x2_Input) && ValidarInput(y2_Input);
    }

    private bool ValidarInput(TMP_InputField input)
    {
        return input != null && !string.IsNullOrEmpty(input.text.Trim());
    }

    private Vector2Int LerCoordenada(TMP_InputField inputX, TMP_InputField inputY)
    {
        int.TryParse(inputX.text, out int x);
        int.TryParse(inputY.text, out int y);

        return new Vector2Int(x, y);
    }
}