using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject boxPrefab;
    [SerializeField] private GameObject lineRendererPrefab;
    [SerializeField] private GameObject boxPoolPositionPosition;
    [SerializeField] private GameObject lineRendererPoolPosition;
    [SerializeField] private GameObject loadingPanel;
   


    [SerializeField] [Range(0, 1000)] private int boxPoolSize = 100;

    // Пул Box-ов
    private static Queue<BoxController> boxPool = new Queue<BoxController>();
    // Пул LineRenderer-ов
    private static Queue<LineRendererController> lineRendererPool = new Queue<LineRendererController>();
    // Спиской выбранных box-ов
    private static List<BoxController> selectedBoxes = new List<BoxController>();
    // Список активных (на экране) лайн рендереров
    private static List<LineRendererController> activeLineRenderers = new List<LineRendererController>();

  
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }


    void Start()
    {
        Application.targetFrameRate = 70;
        FillBoxPool();
        FillLineRenererPool();
    }
    
    // Создаем пул коробок
    private void FillBoxPool()
    {
        for (int i = 0; i < boxPoolSize; i++)
        {
            var box = Instantiate(boxPrefab, boxPoolPositionPosition.transform.position, Quaternion.identity, boxPoolPositionPosition.transform);
            BoxController boxC = box.GetComponent<BoxController>();
            boxC.gameObject.SetActive(false);
            boxPool.Enqueue(boxC);
        }
    }

    // создаем пул лайнРендереров
    private void FillLineRenererPool()
    {
        int maxPoolSize = (int)(0.5 * boxPoolSize * boxPoolSize - 0.5 * boxPoolSize);       // максимально возможное количество связей (линий) для конкретного значения boxPoolSize
        maxPoolSize = maxPoolSize > 5000 ? 5000 : maxPoolSize;                              // ограничиваем 5000 ЛРок иначе долго создаются.

        for (int i = 0; i < maxPoolSize; i++)
        {
            var lr = Instantiate(lineRendererPrefab, lineRendererPoolPosition.transform.position, Quaternion.identity, lineRendererPoolPosition.transform);
            LineRendererController lrC = lr.GetComponent<LineRendererController>();
            lrC.gameObject.SetActive(false);
            lineRendererPool.Enqueue(lrC);
        }
    }

    // Достаем коробку из пула или создаем новую, если пул пустой
    public static void SpawnBox(Vector3 position)
    {
        BoxController boxC;
        if (boxPool.Count > 0)
        {
            boxC = boxPool.Dequeue();
            boxC.transform.position = position;
            boxC.gameObject.SetActive(true);
            return;
        }

        var box = Instantiate(Instance.boxPrefab, position, Quaternion.identity, Instance.boxPoolPositionPosition.transform);
        boxC = box.GetComponent<BoxController>();
        boxC.gameObject.SetActive(true);
    }

    // Достаем лайн рендерер из пула или создаем новую, если пул пустой
    public static LineRendererController SpawnLineRenderer()
    {
        LineRendererController lrC;

        if (lineRendererPool.Count > 0)
        {
            lrC = lineRendererPool.Dequeue();
            lrC.gameObject.SetActive(true);
            return lrC;
        }

        var lr = Instantiate(Instance.lineRendererPrefab, Instance.lineRendererPoolPosition.transform.position, Quaternion.identity, Instance.lineRendererPoolPosition.transform);
        lrC = lr.GetComponent<LineRendererController>();
        lrC.gameObject.SetActive(true);
        return lrC;
    }

    // Уничтожаем (убираем в пул)
    public static void DestroyBox(BoxController box)
    {
        // Возвращаем объект в пул
        box.gameObject.SetActive(false);
        box.transform.position = GameManager.Instance.boxPoolPositionPosition.transform.position;
        boxPool.Enqueue(box);
    }

    // Уничтожаем (убираем в пул)
    public static void DestroyLR(LineRendererController lr)
    {
        // Возвращаем объект в пул
        lr.gameObject.SetActive(false);
        lineRendererPool.Enqueue(lr);
    }

    // Обрабатываем выбранные коробки (создаем линии между ними)
    public static void SetSelectedBoxed(Collider[] hitColliders)
    {
        foreach (Collider collider in hitColliders)
        {
            selectedBoxes.Add(collider.gameObject.GetComponent<BoxController>());
        }

        for (int i = 0; i < selectedBoxes.Count; i++)       // Проходим один раз по каждой уникальной паре элементов.
        {
            for (int j = i+1; j < selectedBoxes.Count; j++)
            {
                // "создаем" LineRenderer из пула
                LineRendererController lineRendererController = SpawnLineRenderer();
                activeLineRenderers.Add(lineRendererController);
                lineRendererController.SetConnectedBoxes(selectedBoxes[i], selectedBoxes[j]);               
            }
        }
    }


    // Сбрасываем выделение объектов (удаляем связи)
    public static void DropSelectedBoxes()
    {        
        foreach (LineRendererController lr in activeLineRenderers)
        {
            DestroyLR(lr);
        }
        activeLineRenderers.Clear();
        selectedBoxes.Clear();
    }

    public void Quit()
    {
        Application.Quit();
    }
}
