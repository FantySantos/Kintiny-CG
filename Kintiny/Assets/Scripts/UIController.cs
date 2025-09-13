using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    [SerializeField]
    private DrawingRenderer drawingRenderer;
    [SerializeField]
    private LogManager logManager;
    [SerializeField]
    private GameObject painelConfiguracoes;
    [SerializeField]
    private GameObject desenharJanelaButton;
    [SerializeField]
    private GameObject transformacaoConfiguracoes;
    [SerializeField]
    private GameObject coordinates3D;

    [SerializeField]
    private Bresenham bresenham;
    [SerializeField]
    private PontoMedioCirculo pontoMedioCirculo;
    [SerializeField]
    private PontoMedioElipse pontoMedioElipse;
    [SerializeField]
    private Bezier bezier;
    [SerializeField]
    private Polilinha polilinha;
    [SerializeField]
    private CohenSutherland cohenSutherland;
    [SerializeField]
    private SutherlandHodgman sutherlandHodgman;
    [SerializeField]
    private FloodFill floodFill;
    [SerializeField]
    private ScanlineFill scanline;
    [SerializeField]
    private Transformer transformer;
    [SerializeField]
    private Ortogonal ortogonal;
    [SerializeField]
    private Perspectiva perspectiva;
    [SerializeField]
    private Cabinet cabinet;
    [SerializeField]
    private Cavalier cavalie;

    public CoordinatesWindowManager coordinatesWindowManager;
    public MultiCoordinateParser multiCoordinateParser;


    private int clickButtonResize = 0;
    public static int selectedAlgorithmIndex = 0;
    private float delayPassos = 0.5f;
    private bool desenharJanela = false;

    public void OnDropdownValueChanged(int newValue)
    {
        desenharJanela = false;
        logManager.ClearLogMessages();
        selectedAlgorithmIndex = newValue;

        if (selectedAlgorithmIndex == 5 || selectedAlgorithmIndex == 6)
        {
            painelConfiguracoes.SetActive(true);
            desenharJanelaButton.SetActive(true);
            transformacaoConfiguracoes.SetActive(false);
            coordinates3D.SetActive(false);
        }
        else if (selectedAlgorithmIndex == 7)
        {
            painelConfiguracoes.SetActive(false);
            desenharJanelaButton.SetActive(false);
            transformacaoConfiguracoes.SetActive(false);
            coordinates3D.SetActive(false);
            drawingRenderer.ClearPaintedPixels();
        }
        else if (selectedAlgorithmIndex == 8)
        {
            painelConfiguracoes.SetActive(false);
            desenharJanelaButton.SetActive(false);
            transformacaoConfiguracoes.SetActive(false);
            coordinates3D.SetActive(false);
        }
        else if (selectedAlgorithmIndex == 9)
        {
            painelConfiguracoes.SetActive(false);
            desenharJanelaButton.SetActive(false);
            coordinates3D.SetActive(false);
            transformacaoConfiguracoes.SetActive(true);
        }
        else if (selectedAlgorithmIndex >= 10)
        {
            painelConfiguracoes.SetActive(false);
            desenharJanelaButton.SetActive(false);
            transformacaoConfiguracoes.SetActive(false);
            coordinates3D.SetActive(true);
        }
        else
        {
            painelConfiguracoes.SetActive(false);
            desenharJanelaButton.SetActive(false);
            transformacaoConfiguracoes.SetActive(false);
            coordinates3D.SetActive(false);
            drawingRenderer.ClearCanvas();
        }
    }

    public void OnSliderValueChange(float newValue)
    {
        delayPassos = newValue;
    }

    public void OnResize()
    {
        if (drawingRenderer == null) return;

        switch (clickButtonResize)
        {
            case 0:
                drawingRenderer.ResizeCanvas(new Vector2Int(64, 64));
                break;
            case 1:
                drawingRenderer.ResizeCanvas(new Vector2Int(16, 16));
                break;
            case 2:
                drawingRenderer.ResizeCanvas(new Vector2Int(32, 32));
                break;
        }

        clickButtonResize++;

        if (clickButtonResize > 2)
        {
            clickButtonResize = 0;
        }
    }

    public void OnClearButtonClicked()
    {
        desenharJanela = false;
        if (drawingRenderer != null)
            drawingRenderer.ClearCanvas();
    }

    public void OnZoomInButtonClicked()
    {
        if (drawingRenderer != null)
            drawingRenderer.ZoomIn();
    }

    public void OnZoomOutButtonClicked()
    {
        if (drawingRenderer != null)
            drawingRenderer.ZoomOut();
    }

    public void OnResetZoomButtonClicked()
    {
        if (drawingRenderer != null)
            drawingRenderer.ResetZoom();
    }

    public void OnClickDesenharJanela()
    {
        if (drawingRenderer == null || cohenSutherland == null)
            return;

        var coordinates = coordinatesWindowManager.GetVector2Ints();
        if (selectedAlgorithmIndex == 5)
            cohenSutherland.DesenharJanelaDeRecorte(coordinates[1], coordinates[0]);
        else if ( selectedAlgorithmIndex == 6)
            sutherlandHodgman.DesenharJanelaDeRecorte(coordinates[1], coordinates[0]);
        desenharJanela = true;
    }

    public void OnExecuteButtonClicked()
    {
        logManager.ClearLogMessages();

        switch (selectedAlgorithmIndex)
        {
            case 0:
                ExecuteBresenham();
                break;
            case 1:
                ExecuteCirculo();
                break;
            case 2:
                ExecuteElipse();
                break;
            case 3:
                ExecuteBezier();
                break;
            case 4:
                ExecutePolilinha();
                break;
            case 5:
                ExecuteCohenSutherland();
                break;
            case 6:
                ExecuteSutherlandHodgman();
                break;
            case 7:
                ExecuteFloodFill();
                break;
            case 8:
                ExecuteScanline();
                break;
            case 9:
                ExecuteTransformer();
                break;
            case 10:
                ExecuteOrtogonal();
                break;
            case 11:
                ExecutePerspectiva();
                break;
            case 12:
                ExecuteCabinet();
                break;
            case 13:
                ExecuteCavalier();
                break;
            default:
                break;
        }
    }

    private void ExecuteBresenham()
    {
        if (drawingRenderer == null || bresenham == null)
            return;

        var paintedPixels = drawingRenderer.GetAllPaintedPixelsCartesian();

        if (paintedPixels.Count == 2)
        {
            Vector2Int point1 = paintedPixels[0];
            Vector2Int point2 = paintedPixels[1];

            bresenham.Desenhar(point1.x, point1.y, point2.x, point2.y, delayPassos);
        }
        else
            logManager.AddLogMessage("É necessário pintar exatamente dois pixels para executar o algoritmo de Bresenham.");
    }

    private void ExecuteCirculo()
    {
        if (drawingRenderer == null || pontoMedioCirculo == null)
            return;

        var paintedPixels = drawingRenderer.GetAllPaintedPixelsCartesian();

        if (paintedPixels.Count == 2)
        {
            Vector2Int center = paintedPixels[0];
            Vector2Int edge = paintedPixels[1];
            int radius = Mathf.RoundToInt(Vector2Int.Distance(center, edge));
            pontoMedioCirculo.Desenhar(center.x, center.y, radius, delayPassos);
        }
        else
            logManager.AddLogMessage("É necessário pintar exatamente dois pixels para executar o algoritmo do Ponto Médio do Círculo.");
    }

    private void ExecuteElipse()
    {
        if (drawingRenderer == null || pontoMedioCirculo == null)
            return;

        var paintedPixels = drawingRenderer.GetAllPaintedPixelsCartesian();
        if (paintedPixels.Count == 3)
        {
            Vector2Int center = paintedPixels[0];
            Vector2Int edgeA = paintedPixels[1];
            Vector2Int edgeB = paintedPixels[2];
            int a = Mathf.RoundToInt(Vector2Int.Distance(center, edgeA));
            int b = Mathf.RoundToInt(Vector2Int.Distance(center, edgeB));
            pontoMedioElipse.Desenhar(center.x, center.y, a, b, delayPassos);
        }
        else
            logManager.AddLogMessage("É necessário pintar exatamente três pixels para executar o algoritmo do Ponto Médio da Elipse.");
    }

    private void ExecuteBezier()
    {
        if (drawingRenderer == null || bezier == null)
            return;

        var paintedPixels = drawingRenderer.GetAllPaintedPixelsCartesian();
        if (paintedPixels.Count >= 2)
            bezier.Desenhar(paintedPixels, 100, delayPassos);
        else
            logManager.AddLogMessage("É necessário pintar pelo menos dois pixels para executar o algoritmo de Bézier.");
    }

    private void ExecutePolilinha()
    {
        if (drawingRenderer == null || polilinha == null)
            return;

        var paintedPixels = drawingRenderer.GetAllPaintedPixelsCartesian();
        if (paintedPixels.Count >= 3)
            polilinha.Desenhar(paintedPixels, delayPassos);
        else
            logManager.AddLogMessage("É necessário pintar pelo menos três pixels para executar o algoritmo de Polilinha.");
    }

    private void ExecuteCohenSutherland()
    {
        if (drawingRenderer == null || cohenSutherland == null || bresenham == null)
            return;

        var paintedPixels = drawingRenderer.GetAllPaintedPixelsCartesian();

        if (!desenharJanela)
        {
            logManager.AddLogMessage("Primeiro desenhe a janela de recorte.");
            return;
        }

        if (paintedPixels.Count >= 2)
        {
            Vector2Int point1 = paintedPixels[0];
            Vector2Int point2 = paintedPixels[1];

            Vector2Int p1f = new(point1.x, point1.y);
            Vector2Int p2f = new(point2.x, point2.y);

            if (cohenSutherland.Clip(ref p1f, ref p2f))
            {
                drawingRenderer.ClearCanvas(false);
                bresenham.Desenhar(p1f.x, p1f.y, p2f.x, p2f.y, 0, false);
                OnClickDesenharJanela();
            }
        }
        else
            logManager.AddLogMessage("É necessário desenhar uma linha para executar o algoritmo de Bresenham.");
    }

    private void ExecuteSutherlandHodgman()
    {
        if (drawingRenderer == null || sutherlandHodgman == null)
            return;

        var paintedPixels = drawingRenderer.GetAllPaintedPixelsCartesian();

        if (!desenharJanela)
        {
            logManager.AddLogMessage("Primeiro desenhe a janela de recorte.");
            return;
        }

        if (paintedPixels.Count >= 3)
        {
            List<Vector2Int> paintedPixelsRef = new(paintedPixels);

            foreach (var p in paintedPixels)
                drawingRenderer.ErasePixelCartesian(p);

            if (sutherlandHodgman.Clip(ref paintedPixelsRef))
                polilinha.Desenhar(paintedPixelsRef, 0);
        }
        else
            logManager.AddLogMessage("É necessário desenhar um polígono para executar o algoritmo de Sutherland-Hodgman.");
    }

    private void ExecuteFloodFill()
    {
        if (drawingRenderer == null || floodFill == null)
            return;

        var paintedPixels = drawingRenderer.GetAllPaintedPixelsCartesian();
        if (paintedPixels.Count == 1)
        {
            Vector2Int startPoint = paintedPixels[0];
            floodFill.IniciarPreenchimento(startPoint, delayPassos);
        }
        else
            logManager.AddLogMessage("É necessário pintar exatamente um pixel para executar o algoritmo de preenchimento recursivo.");
    }

    private void ExecuteScanline()
    {
        if (drawingRenderer == null || scanline == null)
            return;

        var paintedPixels = drawingRenderer.GetAllPaintedPixelsCartesian();
        if (paintedPixels.Count >= 3)
            scanline.IniciarPreenchimento(paintedPixels, delayPassos);
        else
            logManager.AddLogMessage("É necessário um polígono para executar o algoritmo de Scanline.");
    }

    private void ExecuteTransformer()
    {
        if (drawingRenderer == null || transformer == null)
            return;

        var paintedPixels = drawingRenderer.GetAllPaintedPixelsCartesian();

        if (paintedPixels.Count >= 3)
            transformer.AplicarTransformacoes(paintedPixels, delayPassos);
        else
            logManager.AddLogMessage("É necessário um polígono para executar as transformações geométricas.");
    }

    private void ExecuteOrtogonal()
    {
        if (drawingRenderer == null || ortogonal == null || multiCoordinateParser == null)
            return;

        var pontos3D = multiCoordinateParser.GetCoordinates();
        if (pontos3D != null && pontos3D.Count >= 2)
            ortogonal.Projetar(pontos3D);
        else
            logManager.AddLogMessage("É necessário pelo menos dois pontos 3D para executar a projeção ortográfica.");
    }

    private void ExecutePerspectiva()
    {
        if (drawingRenderer == null || perspectiva == null || multiCoordinateParser == null)
            return;

        var pontos3D = multiCoordinateParser.GetCoordinates();
        if (pontos3D != null && pontos3D.Count >= 2)
            perspectiva.Projetar(pontos3D, -10f);
        else
            logManager.AddLogMessage("É necessário pelo menos dois pontos 3D para executar a projeção em perspectiva.");
    }

    private void ExecuteCabinet()
    {
        if (drawingRenderer == null || cabinet == null || multiCoordinateParser == null)
            return;

        var pontos3D = multiCoordinateParser.GetCoordinates();
        if (pontos3D != null && pontos3D.Count >= 2)
            cabinet.Projetar(pontos3D, 45f);
        else
            logManager.AddLogMessage("É necessário pelo menos dois pontos 3D para executar a projeção Cabinet.");
    }

    private void ExecuteCavalier()
    {
        if (drawingRenderer == null || cavalie == null || multiCoordinateParser == null)
            return;

        var pontos3D = multiCoordinateParser.GetCoordinates();
        if (pontos3D != null && pontos3D.Count >= 2)
            cavalie.Projetar(pontos3D, 45f);
        else
            logManager.AddLogMessage("É necessário pelo menos dois pontos 3D para executar a projeção Cavalier.");
    }
}