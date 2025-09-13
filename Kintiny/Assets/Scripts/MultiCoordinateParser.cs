using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System;

public class MultiCoordinateParser : MonoBehaviour
{
    public TMP_InputField coordinatesInput;

    public List<Vector3> GetCoordinates()
    {
        if (coordinatesInput == null || string.IsNullOrWhiteSpace(coordinatesInput.text))
        {
            Debug.LogWarning("O campo de coordenadas está vazio.");
            return null;
        }

        List<Vector3> parsedCoordinates = new List<Vector3>();

        string[] pointStrings = coordinatesInput.text.Split(';', StringSplitOptions.RemoveEmptyEntries);

        foreach (string pointStr in pointStrings)
        {
            string[] componentStrings = pointStr.Split(',');

            if (componentStrings.Length != 3)
            {
                Debug.LogError($"Erro de formato: O ponto '{pointStr.Trim()}' não tem 3 componentes (X,Y,Z).");
                return null;
            }

            if (float.TryParse(componentStrings[0].Trim(), out float x) &&
                float.TryParse(componentStrings[1].Trim(), out float y) &&
                float.TryParse(componentStrings[2].Trim(), out float z))
            {
                parsedCoordinates.Add(new Vector3(x, y, z));
            }
            else
            {
                Debug.LogError($"Erro de formato: Não foi possível ler os números no ponto '{pointStr.Trim()}'.");
                return null;
            }
        }

        Debug.Log($"{parsedCoordinates.Count} coordenadas lidas com sucesso.");
        return parsedCoordinates;
    }
}