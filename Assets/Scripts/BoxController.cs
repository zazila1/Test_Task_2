using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoxController : MonoBehaviour, IPointerClickHandler, IEndDragHandler, IBeginDragHandler, IDragHandler
{
    private Transform transform;
    private Rigidbody rigidbody;


    public delegate void BoxDestroyEventHandler();
    public event BoxDestroyEventHandler BoxDestroyEvent;
    
    [SerializeField] [Range(0, 2000)] private int _dragSpeedForce = 1500;
    [SerializeField] [Range(0, 50)] private int _dragSpeedVelocity = 5;
    [SerializeField] [Range(0, 20)] private float _stopDistance = 1.5f;            // Область вокруг точки назначения, в которой объект будет тормозиться
    
    private bool _isSetup = false;
    private Vector3 dragEventPosition = new Vector3();

    private enum MovementType
    {
        Force, Velocity
    }

    [SerializeField] private MovementType movementType; // это видно в инспекторе

    public bool DragActive { get; private set; }

    void Awake()
    {
        transform = gameObject.GetComponent<Transform>();
        rigidbody = gameObject.GetComponent<Rigidbody>();
        GetComponent<Renderer>().material.color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        int clickCount = eventData.clickCount;

        if (clickCount == 1)
        {
            OnSingleClick();
        }
        else if (clickCount == 2)
        {
            OnDoubleClick();
        }
    }
    private void OnSingleClick()
    {
        //Debug.Log("Box Single Click");
    }

    private void OnDoubleClick()
    {
        BoxDestroyEvent?.Invoke();
        GameManager.DestroyBox(this);
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        DragActive = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        rigidbody.velocity = new Vector3(0, 0, 0);
        DragActive = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Не перемещаем если курсор за пределами экрана
        if (Input.mousePosition.y > 0 && Input.mousePosition.y < Screen.height && Input.mousePosition.x > 0 && Input.mousePosition.x < Screen.width)
        {
            dragEventPosition = eventData.pointerCurrentRaycast.worldPosition;
        }
    }

    // Логика перемещения объекта
    private void MoveBox()
    {
        if (!DragActive)
        {
            return;
        }

        Vector2 moveDirection = new Vector2(dragEventPosition.x - transform.position.x,
                                            dragEventPosition.y - transform.position.y);

        rigidbody.velocity = Vector3.zero; // Тормозим до нуля, что бы вектор скорости всегда был в направлении точки движения
        var currDistance = Vector2.Distance(dragEventPosition, transform.position);

        if (currDistance > _stopDistance)
        {
            moveDirection.Normalize();

            switch (movementType)
            {
                case MovementType.Force:
                    rigidbody.AddForce(moveDirection * _dragSpeedForce * (currDistance > 1 ? currDistance : 1));
                    break;

                case MovementType.Velocity:
                    rigidbody.velocity = moveDirection * _dragSpeedVelocity * (currDistance > 1 ? currDistance : 1);
                    break;
            }
        }
    }
    void FixedUpdate()
    {
        MoveBox();
    }
}
