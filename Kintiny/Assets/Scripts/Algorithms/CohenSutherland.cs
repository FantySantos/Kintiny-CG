using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CohenSutherland : MonoBehaviour
{
    [SerializeField]
    private DrawingRenderer drawingRenderer;

    [SerializeField]
    private LogManager logManager;

    [SerializeField]
    private Polilinha polilinha;

    private int x_min, y_min, x_max, y_max;

    [System.Flags]
    private enum OutCode
    {
        Inside = 0, // 0000
        Left = 1,   // 0001
        Right = 2,  // 0010
        Bottom = 4, // 0100
        Top = 8     // 1000
    }

    public void DesenharJanelaDeRecorte(Vector2Int windowMin, Vector2Int windowMax)
    {
        this.x_min = windowMin.x;
        this.y_min = windowMin.y;
        this.x_max = windowMax.x;
        this.y_max = windowMax.y;

        logManager.AddLogMessage($"Janela de recorte definida de ({x_min},{y_min}) até ({x_max},{y_max}).");

        DesenharJanela();
    }

    private void DesenharJanela()
    {
        Vector2Int p1 = new(x_min, y_min);
        Vector2Int p2 = new(x_max, y_min);
        Vector2Int p3 = new(x_max, y_max);
        Vector2Int p4 = new(x_min, y_max);

        List<Vector2Int> pontosJanela = new() { p1, p2, p3, p4, p1 };
        
        polilinha.Desenhar(pontosJanela, 0);
        
        logManager.AddLogMessage($"Janela de recorte desenhada com os pontos: ({p1.x},{p1.y}), ({p2.x},{p2.y}), ({p3.x},{p3.y}), ({p4.x},{p4.y}).");
    }

    public bool Clip(ref Vector2Int p1, ref Vector2Int p2)
    {
        OutCode outcode1 = ComputeOutCode(p1.x, p1.y);
        OutCode outcode2 = ComputeOutCode(p2.x, p2.y);
        bool accept = false;

        while (true)
        {
            if (outcode1 == OutCode.Inside && outcode2 == OutCode.Inside)
            {
                logManager.AddLogMessage($"Linha totalmente dentro da janela de recorte.");
                accept = true;
                break;
            }
            else if ((outcode1 & outcode2) != 0)
            {
                logManager.AddLogMessage($"Linha totalmente fora da janela de recorte.");
                break;
            }
            else
            {
                Vector2Int intersectionPoint = new();
                OutCode outcodeOut = (outcode1 != OutCode.Inside) ? outcode1 : outcode2;
                logManager.AddLogMessage($"Calculando interseção para o ponto com OutCode {outcodeOut}.");

                if ((outcodeOut & OutCode.Top) != 0)
                {
                    logManager.AddLogMessage($"Ponto acima da janela. Calculando interseção com y = {y_max}.");
                    intersectionPoint.x = p1.x + (p2.x - p1.x) * (y_max - p1.y) / (p2.y - p1.y);
                    intersectionPoint.y = y_max;
                }
                else if ((outcodeOut & OutCode.Bottom) != 0)
                {
                    logManager.AddLogMessage($"Ponto abaixo da janela. Calculando interseção com y = {y_min}.");
                    intersectionPoint.x = p1.x + (p2.x - p1.x) * (y_min - p1.y) / (p2.y - p1.y);
                    intersectionPoint.y = y_min;
                }
                else if ((outcodeOut & OutCode.Right) != 0)
                {
                    logManager.AddLogMessage($"Ponto à direita da janela. Calculando interseção com x = {x_max}.");
                    intersectionPoint.y = p1.y + (p2.y - p1.y) * (x_max - p1.x) / (p2.x - p1.x);
                    intersectionPoint.x = x_max;
                }
                else if ((outcodeOut & OutCode.Left) != 0)
                {
                    logManager.AddLogMessage($"Ponto à esquerda da janela. Calculando interseção com x = {x_min}.");    
                    intersectionPoint.y = p1.y + (p2.y - p1.y) * (x_min - p1.x) / (p2.x - p1.x);
                    intersectionPoint.x = x_min;
                }

                if (outcodeOut == outcode1)
                {
                    logManager.AddLogMessage($"Atualizando ponto 1 para a interseção em ({intersectionPoint.x}, {intersectionPoint.y}).");
                    p1 = intersectionPoint;
                    outcode1 = ComputeOutCode(p1.x, p1.y);
                }
                else
                {
                    logManager.AddLogMessage($"Atualizando ponto 2 para a interseção em ({intersectionPoint.x}, {intersectionPoint.y}).");
                    p2 = intersectionPoint;
                    outcode2 = ComputeOutCode(p2.x, p2.y);
                }
            }
        }
        return accept;
    }

    private OutCode ComputeOutCode(float x, float y)
    {
        OutCode code = OutCode.Inside;
        if (x < x_min) code |= OutCode.Left;
        else if (x > x_max) code |= OutCode.Right;
        if (y < y_min) code |= OutCode.Bottom;
        else if (y > y_max) code |= OutCode.Top;
        return code;
    }
}