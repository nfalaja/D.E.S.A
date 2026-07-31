using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections; // Wajib untuk Coroutine

public class CardDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    private CanvasGroup canvasGroup;
    [HideInInspector] public CardUI cardUI;
    public bool isLocked = false;

    [Header("Visual Efek")]
    public float hoverScale = 1.05f;
    public float dragScale = 1.15f; // BARU: Skala saat diseret
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

        // PENYEMBUHAN: Jangan ambil Canvas terdekat, ambil Canvas PALING LUAR (Root)
        Canvas[] canvases = GetComponentsInParent<Canvas>();
        if (canvases.Length > 0)
        {
            mainCanvas = canvases[canvases.Length - 1]; // Kasta tertinggi
        }
        else
        {
            Debug.LogError("[CardDrag] Tidak ada Canvas yang ditemukan untuk kartu ini!");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isLocked) return;
        transform.localScale = originalScale * hoverScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isLocked) return;
        transform.localScale = originalScale;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("<color=red>[CardDrag] Sinyal Drag Diterima pada: </color>" + gameObject.name);
        if (isLocked) return;

        canvasGroup.alpha = 0.4f;
        canvasGroup.blocksRaycasts = false;
        if (cardUI != null) cardUI.SetTextVisibility(false);

        // 1. Buat Clone di Root Canvas
        dragClone = Instantiate(gameObject, mainCanvas.transform);

        // 2. PENYELAMATAN NYAWA: Paksa ukuran UI agar tidak hancur saat keluar dari LayoutGroup
        RectTransform cloneRect = dragClone.GetComponent<RectTransform>();
        RectTransform originalRect = GetComponent<RectTransform>();
        cloneRect.sizeDelta = originalRect.sizeDelta; // Kopi ukuran (Width/Height)
        cloneRect.position = originalRect.position;   // Mulai dari posisi asli

        CardUI cloneUI = dragClone.GetComponent<CardUI>();
        if (cloneUI != null) cloneUI.SetTextVisibility(true);

        Destroy(dragClone.GetComponent<CardDrag>());

        dragClone.transform.SetAsLastSibling(); // Pastikan di urutan paling depan
        dragClone.transform.localScale = originalScale * dragScale;

        CanvasGroup cloneGroup = dragClone.GetComponent<CanvasGroup>();
        if (cloneGroup != null) cloneGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragClone != null)
        {
            // 3. CARA LEGAL MENGGERAKKAN UI: Konversi pixel layar ke posisi dunia UI
            RectTransform cloneRect = dragClone.GetComponent<RectTransform>();
            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                cloneRect,
                eventData.position,
                eventData.pressEventCamera,
                out Vector3 globalMousePos);

            cloneRect.position = globalMousePos;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("<color=yellow>[CardDrag] OnEndDrag Dipanggil!</color> Status isLocked: " + isLocked);

        if (isLocked)
        {
            Debug.Log("<color=green>[CardDrag] Kartu Terkunci (Drop Sukses). Hancurkan Clone.</color>");
            if (dragClone != null) Destroy(dragClone);

            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
            transform.localScale = originalScale;
            if (cardUI != null) cardUI.SetTextVisibility(false);

            this.enabled = false;
        }
        else
        {
            Debug.Log("<color=orange>[CardDrag] Drop Gagal. Memulai Animasi Pulang...</color>");
            if (dragClone != null)
            {
                // PERIKSA STATUS OBJEK ASLI
                Debug.Log("Apakah kartu asli aktif di Hierarchy? " + gameObject.activeInHierarchy);
                StartCoroutine(ReturnCloneRoutine());
            }
            else
            {
                Debug.LogError("[CardDrag] GAGAL: dragClone ternyata NULL saat OnEndDrag!");
                ResetCardVisuals();
            }
        }
    }

    private IEnumerator ReturnCloneRoutine()
    {
        Debug.Log("<color=cyan>[CardDrag] Coroutine Dimulai: Frame Pertama</color>");

        float time = 0;
        float duration = 0.25f;
        Vector3 startPos = dragClone.transform.position;
        Vector3 startScale = dragClone.transform.localScale;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            float easeOut = 1f - (1f - t) * (1f - t);

            if (dragClone != null)
            {
                dragClone.transform.position = Vector3.Lerp(startPos, transform.position, easeOut);
                dragClone.transform.localScale = Vector3.Lerp(startScale, originalScale, easeOut);
            }
            else
            {
                Debug.LogError("[CardDrag] Kloningan tiba-tiba hancur di tengah animasi!");
                break;
            }
            yield return null;
        }

        Debug.Log("<color=cyan>[CardDrag] Coroutine Selesai: Tiba di Tujuan</color>");

        if (dragClone != null) Destroy(dragClone);
        ResetCardVisuals();
    }

    private void ResetCardVisuals()
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        transform.localScale = originalScale;
        if (cardUI != null) cardUI.SetTextVisibility(true);
    }
}