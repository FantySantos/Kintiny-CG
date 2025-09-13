using UnityEngine;
using TMPro;

public class TransformInputManager : MonoBehaviour
{
    [Header("Pivô (Ponto de Referência)")]
    public TMP_InputField pivotX_Input;
    public TMP_InputField pivotY_Input;

    [Header("Escala (Deixar em branco para 1,1)")]
    public TMP_InputField scaleX_Input;
    public TMP_InputField scaleY_Input;

    [Header("Rotação (Deixar em branco para 0)")]
    public TMP_InputField rotation_Input;

    [Header("Translação (Deixar em branco para 0,0)")]
    public TMP_InputField translationX_Input;
    public TMP_InputField translationY_Input;

    private Vector2Int? pivot;
    private Vector2 scale;
    private float rotation;
    private Vector2Int translation;

    public bool ProcessarInputs()
    {
        pivot = LerVector2IntNullable(pivotX_Input, pivotY_Input);
        scale = LerVector2(scaleX_Input, scaleY_Input, Vector2.one);
        rotation = LerFloat(rotation_Input, 0f);
        translation = LerVector2Int(translationX_Input, translationY_Input, Vector2Int.zero);

        Debug.Log($"Dados processados: Pivô({pivot}), Escala({scale}), Rotação({rotation}°), Translação({translation})");

        return true;
    }

    public Vector2Int? GetPivot() { return pivot; }
    public Vector2 GetScale() { return scale; }
    public float GetRotation() { return rotation; }
    public Vector2Int GetTranslation() { return translation; }

    private bool ValidarInput(TMP_InputField input)
    {
        return input != null && !string.IsNullOrEmpty(input.text.Trim());
    }

    private Vector2Int? LerVector2IntNullable(TMP_InputField inputX, TMP_InputField inputY)
    {
        if (!ValidarInput(inputX) && !ValidarInput(inputY))
            return null;

        int.TryParse(inputX.text, out int x);
        int.TryParse(inputY.text, out int y);
        return new Vector2Int(x, y);
    }

    private Vector2Int LerVector2Int(TMP_InputField inputX, TMP_InputField inputY, Vector2Int valorPadrao)
    {
        if (!ValidarInput(inputX) && !ValidarInput(inputY))
            return valorPadrao;

        int.TryParse(inputX.text, out int x);
        int.TryParse(inputY.text, out int y);
        return new Vector2Int(x, y);
    }

    private Vector2 LerVector2(TMP_InputField inputX, TMP_InputField inputY, Vector2 valorPadrao)
    {
        if (!ValidarInput(inputX) && !ValidarInput(inputY))
            return valorPadrao;

        float x = ValidarInput(inputX) ? float.Parse(inputX.text) : 1f;
        float y = ValidarInput(inputY) ? float.Parse(inputY.text) : 1f;
        return new Vector2(x, y);
    }

    private float LerFloat(TMP_InputField input, float valorPadrao)
    {
        if (!ValidarInput(input))
            return valorPadrao;

        float.TryParse(input.text, out float value);
        return value;
    }
}