using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Cabinet : MonoBehaviour
{
    [SerializeField]
    private DrawingRenderer drawingRenderer;

    [SerializeField]
    private LogManager logManager;

    [SerializeField]
    private Bresenham bresenham;

    public void Projetar(List<Vector3> pontos3D, float anguloEmGraus)
    {
        if (bresenham == null || pontos3D == null || pontos3D.Count == 0)
        {
            logManager.AddLogMessage("Erro: Dependências não atribuídas ou lista de pontos 3D inválida.");
            return;
        }
        StartCoroutine(ProjetarCoroutine(pontos3D, anguloEmGraus));
    }

    private IEnumerator ProjetarCoroutine(List<Vector3> pontos3D, float anguloEmGraus)
    {
        logManager.AddLogMessage($"Iniciando Projeção Cabinet com ângulo de {anguloEmGraus} graus.");
        drawingRenderer.ClearCanvas();

        const float delta = 0.5f;

        float anguloRad = anguloEmGraus * Mathf.Deg2Rad;

        Matrix4x4 matrizProjecao = new(
            new Vector4(1, 0, 0, 0),
            new Vector4(0, 1, 0, 0),
            new Vector4(delta * Mathf.Cos(anguloRad), delta * Mathf.Sin(anguloRad), 1, 0),
            new Vector4(0, 0, 0, 1)
        );

        matrizProjecao = new Matrix4x4(
            new Vector4(1, 0, 0, 0),
            new Vector4(0, 1, 0, 0),
            new Vector4(0, 0, 0, 0),
            new Vector4(0, 0, 0, 1)
        )
        {
            m02 = delta * Mathf.Cos(anguloRad),
            m12 = delta * Mathf.Sin(anguloRad)
        };

        List<Vector2Int> pontosProjetados = new();
        logManager.AddLogMessage("Projetando pontos...");

        foreach (var ponto3D in pontos3D)
        {
            var pontoHomogeneo = new Vector4(ponto3D.x, ponto3D.y, ponto3D.z, 1);
            var pontoProjetado = matrizProjecao * pontoHomogeneo;

            var ponto2D = new Vector2Int(
                Mathf.RoundToInt(pontoProjetado.x),
                Mathf.RoundToInt(pontoProjetado.y)
            );

            pontosProjetados.Add(ponto2D);
        }

        logManager.AddLogMessage("Desenhando o resultado da projeção com Bresenham...");

        if (pontosProjetados.Count < 4 || pontosProjetados.Count % 2 != 0)
        {
            logManager.AddLogMessage("Erro: Número de pontos inválido para desenhar um objeto 3D com faces frontal e traseira.");
            yield return null;
        }
        else
        {
            int numPontosFace = pontosProjetados.Count / 2;

            for (int i = 0; i < numPontosFace - 1; i++)
            {
                bresenham.Desenhar(pontosProjetados[i].x, pontosProjetados[i].y, pontosProjetados[i + 1].x, pontosProjetados[i + 1].y, 0, false);
            }

            bresenham.Desenhar(pontosProjetados[numPontosFace - 1].x, pontosProjetados[numPontosFace - 1].y, pontosProjetados[0].x, pontosProjetados[0].y, 0, false);


            for (int i = numPontosFace; i < pontosProjetados.Count - 1; i++)
            {
                bresenham.Desenhar(pontosProjetados[i].x, pontosProjetados[i].y, pontosProjetados[i + 1].x, pontosProjetados[i + 1].y, 0, false);
            }

            bresenham.Desenhar(pontosProjetados[pontosProjetados.Count - 1].x, pontosProjetados[pontosProjetados.Count - 1].y, pontosProjetados[numPontosFace].x, pontosProjetados[numPontosFace].y, 0, false);


            for (int i = 0; i < numPontosFace; i++)
            {
                bresenham.Desenhar(pontosProjetados[i].x, pontosProjetados[i].y, pontosProjetados[i + numPontosFace].x, pontosProjetados[i + numPontosFace].y, 0, false);
            }
        }


        yield return null;
    }
}