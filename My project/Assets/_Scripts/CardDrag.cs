using UnityEngine;
using UnityEngine.EventSystems;

public class CardDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    private CanvasGroup canvasGroup;
    [HideInInspector] public CardUI cardUI;
    [HideInInspector] public bool isLocked = false;

    [Header("Visual Efek")]
    public float hoverScale = 1.05f;
    private Vector3 originalScale;

    private GameObject dragClone;
    private Canvas mainCanvas;

    private void OnDestroy()
    {
        if (dragClone != null) Destroy(dragClone);
    }

    private void Awake()
    {
        cardUI = GetComponent<CardUI>();
        originalScale = transform.localScale;

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();

        mainCanvas = GetComponentInParent<Canvas>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = originalScale * hoverScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = originalScale;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.4f;
        canvasGroup.blocksRaycasts = false;

        dragClone = Instantiate(gameObject, mainCanvas.transform);

        Destroy(dragClone.GetComponent<CardDrag>());

        dragClone.transform.SetAsLastSibling();
        dragClone.transform.localScale = originalScale;

        CanvasGroup cloneGroup = dragClone.GetComponent<CanvasGroup>();
        if (cloneGroup != null) cloneGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragClone != null)
        {
            dragClone.transform.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragClone != null)
        {
            Destroy(dragClone);
        }

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        transform.localScale = originalScale;

        if (isLocked)
        {
            this.enabled = false;
        }
    }
}

