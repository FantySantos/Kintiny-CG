using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Transformer : MonoBehaviour
{
    [SerializeField]
    private DrawingRenderer drawingRenderer;

    [SerializeField]
    private LogManager logManager;

    [SerializeField]
    private Polilinha polilinha;

    [SerializeField]
    private TransformInputManager transformManager;

    public void AplicarTransformacoes(List<Vector2Int> vertices, float delay = 1.5f)
    {
        if (polilinha == null || transformManager == null || drawingRenderer == null || vertices == null || vertices.Count == 0)
        {
            logManager.AddLogMessage("Erro: Dependências não atribuídas no Inspector ou polígono inválido.");
            return;
        }
        if (!transformManager.ProcessarInputs())
            return;

        StartCoroutine(TransformarCoroutine(vertices, delay));
    }

    private IEnumerator TransformarCoroutine(List<Vector2Int> vertices, float delay)
    {
        logManager.AddLogMessage("Iniciando transformações geométricas...");

        drawingRenderer.ClearCanvas(false);
        polilinha.Desenhar(vertices, 0);
        logManager.AddLogMessage("Polígono original desenhado.");

        yield return new WaitForSeconds(delay);

        Vector2Int? pivot = transformManager.GetPivot();
        Vector2 scale = transformManager.GetScale();
        float rotationDegrees = transformManager.GetRotation();
        Vector2Int translation = transformManager.GetTranslation();

        List<Vector2Int> verticesAtuais = new(vertices);

        if (scale.x != 1.0f || scale.y != 1.0f)
        {
            logManager.AddLogMessage($"Aplicando escala: ({scale.x}, {scale.y})...");
            verticesAtuais = AplicarTransformacao(verticesAtuais, pivot, CreateScaleMatrix(scale.x, scale.y));
            
            drawingRenderer.ClearCanvas(false);
            polilinha.Desenhar(verticesAtuais, 0);
            logManager.AddLogMessage("Escala aplicada!");
            
            yield return new WaitForSeconds(delay);
        }

        if (rotationDegrees != 0.0f)
        {
            float anguloRad = rotationDegrees * Mathf.Deg2Rad;
            logManager.AddLogMessage($"Aplicando rotação: {rotationDegrees}°...");
            verticesAtuais = AplicarTransformacao(verticesAtuais, pivot, CreateRotationMatrix(anguloRad));
            
            drawingRenderer.ClearCanvas(false);
            polilinha.Desenhar(verticesAtuais, 0);
            logManager.AddLogMessage("Rotação aplicada!");
            
            yield return new WaitForSeconds(delay);
        }

        if (translation.x != 0 || translation.y != 0)
        {
            logManager.AddLogMessage($"Aplicando translação: ({translation.x}, {translation.y})...");
            verticesAtuais = AplicarTransformacao(verticesAtuais, null, CreateTranslationMatrix(translation.x, translation.y));
            
            drawingRenderer.ClearCanvas(false);
            polilinha.Desenhar(verticesAtuais, 0);
            logManager.AddLogMessage("Translação aplicada!");
        }

        drawingRenderer.UpdateFirstPaintedPixels(verticesAtuais);
        logManager.AddLogMessage("Todas as transformações concluídas!");
    }

    private List<Vector2Int> AplicarTransformacao(List<Vector2Int> vertices, Vector2Int? pivot, Matrix4x4 transformacao)
    {
        Matrix4x4 matrizFinal;

        if (pivot.HasValue)
        {
            Matrix4x4 matTranslacaoPivoOrigem = CreateTranslationMatrix(-pivot.Value.x, -pivot.Value.y);
            Matrix4x4 matTranslacaoPivoVolta = CreateTranslationMatrix(pivot.Value.x, pivot.Value.y);
            matrizFinal = matTranslacaoPivoVolta * transformacao * matTranslacaoPivoOrigem;
        }
        else
        {
            matrizFinal = transformacao;
        }

        List<Vector2Int> verticesTransformados = new();
        foreach (var vertice in vertices)
        {
            var pontoHomogeneo = new Vector3(vertice.x, vertice.y, 1);
            var pontoTransformado = matrizFinal.MultiplyPoint3x4(pontoHomogeneo);
            verticesTransformados.Add(new Vector2Int(Mathf.RoundToInt(pontoTransformado.x), Mathf.RoundToInt(pontoTransformado.y)));
        }

        return verticesTransformados;
    }

    private Matrix4x4 CreateTranslationMatrix(float tx, float ty)
    {
        Matrix4x4 m = Matrix4x4.identity;
        m[0, 3] = tx;
        m[1, 3] = ty;
        return m;
    }

    private Matrix4x4 CreateScaleMatrix(float sx, float sy)
    {
        Matrix4x4 m = Matrix4x4.identity;
        m[0, 0] = sx;
        m[1, 1] = sy;
        return m;
    }

    private Matrix4x4 CreateRotationMatrix(float radianos)
    {
        Matrix4x4 m = Matrix4x4.identity;
        m[0, 0] = Mathf.Cos(radianos);
        m[0, 1] = -Mathf.Sin(radianos);
        m[1, 0] = Mathf.Sin(radianos);
        m[1, 1] = Mathf.Cos(radianos);
        return m;
    }
}