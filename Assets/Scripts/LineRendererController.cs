using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererController : MonoBehaviour
{
    private BoxController box1, box2;
    [SerializeField] private Rigidbody rbBox1, rbBox2;
    private LineRenderer lineRenderer;


    public void SetConnectedBoxes(BoxController b1, BoxController b2)
    {
        box1 = b1;
        box2 = b2;

        rbBox1 = b1.GetComponent<Rigidbody>();
        rbBox2 = b2.GetComponent<Rigidbody>();
        lineRenderer = GetComponent<LineRenderer>();

        box1.BoxDestroyEvent += BoxDestroyed;
        box2.BoxDestroyEvent += BoxDestroyed;

        Draw();
    }

    public void BoxDestroyed()
    {
        GameManager.DestroyLR(this);
    }

    private void OnDisable()
    {
        // Обнуляем все связи
        if (box1 != null)
        {
            box1.BoxDestroyEvent -= BoxDestroyed;
        }
        if (box2 != null)
        {
            box2.BoxDestroyEvent -= BoxDestroyed;
        }

        box1 = null;
        box2 = null;

        rbBox1 = null;
        rbBox2 = null;
        lineRenderer = null;
    }

    public void LateUpdate()
    {
        // перерисовываем линии только если одна из коробок движется / перетаскивается
        if (rbBox1.velocity != Vector3.zero || rbBox2.velocity != Vector3.zero)
        {
            Draw();
        }
    }

    private void Draw()
    {
        Vector3[] linePoints = new Vector3[2];
        linePoints[0] = box1.transform.position;
        linePoints[1] = box2.transform.position;
        lineRenderer.SetPositions(linePoints);
    }

}