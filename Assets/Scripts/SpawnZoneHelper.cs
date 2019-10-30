using UnityEngine;
using UnityEngine.EventSystems;

public class SpawnZoneHelper : MonoBehaviour, IPointerClickHandler, IEndDragHandler, IBeginDragHandler, IDragHandler
{
    [SerializeField] private GameObject boxPrefab;
    [SerializeField] private LayerMask layerMask;


    [SerializeField] [Range(0, 200)] private float _minDistanceForDrag = 50;

    private RaycastResult firstSelectRaycast = new RaycastResult();
    private RaycastResult currentSelectRaycast = new RaycastResult();
    private RaycastResult endSelectRaycast = new RaycastResult();

    private bool _dragActive = false;


    public void OnPointerClick(PointerEventData eventData)
    {

        var clickPosition = eventData.pointerPressRaycast.worldPosition;

        if (!_dragActive)
        {
            GameManager.DropSelectedBoxes();

            if (ChekAvailableSpace(clickPosition))      // Проверяем есть ли место под создание объекта
            {
                GameManager.SpawnBox(clickPosition);
            }
        }

        
    }

    bool ChekAvailableSpace(Vector3 clickPosition)
    {
        // Кидаем OverlapBox размером с Box в точку клика и собираем коллайдеры. Если никто не попал, значит место есть
        Collider[] hitColliders = Physics.OverlapBox(clickPosition, boxPrefab.transform.localScale / 2, Quaternion.identity, layerMask);

        return hitColliders.Length == 0 ? true : false;
    }

    void SelectObjects()
    {
        // выбираем обведенные объекты
        Vector2 start = new Vector2(firstSelectRaycast.worldPosition.x, firstSelectRaycast.worldPosition.y);
        Vector2 end = new Vector2(endSelectRaycast.worldPosition.x, endSelectRaycast.worldPosition.y);
        Vector2 center = new Vector2((start.x + end.x) / 2, (start.y + end.y) / 2);

        Vector3 halfExtents = new Vector3(Mathf.Abs(start.x - center.x), Mathf.Abs(start.y - center.y), 0);

        // Кидаем OverlapBox размером с Box в точку клика и собираем коллайдеры. Если никто не попал, значит место есть
        Collider[] hitColliders = Physics.OverlapBox(center, halfExtents, Quaternion.identity, layerMask);
        
        GameManager.SetSelectedBoxed(hitColliders);        
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        firstSelectRaycast = eventData.pointerCurrentRaycast;
        GameManager.DropSelectedBoxes();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        endSelectRaycast = eventData.pointerCurrentRaycast;
        _dragActive = false;

        SelectObjects();
    }

    public void OnDrag(PointerEventData eventData)
    {
        currentSelectRaycast = eventData.pointerCurrentRaycast;

        // При быстром движении мыши и кликах они воспринимаются не как клик, а как перетаскивание, поэтому я сделал првоерку на этот случай 
        // Теперь окно выделения не будет рисоваться при маленьком перетаскивании, а будет обрабатываться как клик
        if (Vector3.Distance(firstSelectRaycast.screenPosition, currentSelectRaycast.screenPosition) > _minDistanceForDrag)
        {
            _dragActive = true;
        }
    }

    // Рисуем область выделения
    void OnGUI()
    {
        if (_dragActive)
        {
            GUI.Box(GetRectFromPoints(firstSelectRaycast.screenPosition, currentSelectRaycast.screenPosition), "");
        }
    }

    private Rect GetRectFromPoints(Vector3 one, Vector3 two)     // переводим координаты
    {
        float height = two.x - one.x;
        float width = (Screen.height - two.y) - (Screen.height - one.y);

        return new Rect(one.x, Screen.height - one.y, height, width);
    }
}
