using UnityEngine;
using UnityEngine.EventSystems;

// Tambahan antarmuka IPointerEnter dan IPointerExit untuk efek Hover
public class CardDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    private CanvasGroup canvasGroup;
    [HideInInspector] public CardUI cardUI;

    [Header("Visual Efek")]
    public float hoverScale = 1.05f; // Seberapa besar memuai saat di-hover
    private Vector3 originalScale;

    // Referensi untuk sistem Kloning
    private GameObject dragClone;
    private Canvas mainCanvas;

    private void Awake()
    {
        cardUI = GetComponent<CardUI>();
        originalScale = transform.localScale;

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();

        // Mencari canvas utama (root) di scene untuk menempelkan klon agar tidak tertutup UI lain
        mainCanvas = GetComponentInParent<Canvas>();
    }

    // --- EFEK HOVER ---
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Membesar sedikit saat di-hover
        transform.localScale = originalScale * hoverScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Kembali normal saat kursor pergi
        transform.localScale = originalScale;
    }

    // --- EFEK DRAG & CLONE ---
    public void OnBeginDrag(PointerEventData eventData)
    {
        // 1. Kartu ASLI tetap di tempat, tapi redup dan tembus raycast
        canvasGroup.alpha = 0.4f;
        canvasGroup.blocksRaycasts = false;

        // 2. Cetak KLON visual dari kartu ini, taruh di Canvas utama
        dragClone = Instantiate(gameObject, mainCanvas.transform);

        // Hapus script CardDrag di Klon agar si klon tidak bisa di-drag juga (mencegah bug inception)
        Destroy(dragClone.GetComponent<CardDrag>());

        // Atur posisi Klon ke paling depan layar dan matikan raycast-nya
        dragClone.transform.SetAsLastSibling();
        dragClone.transform.localScale = originalScale; // Klon ukurannya normal

        CanvasGroup cloneGroup = dragClone.GetComponent<CanvasGroup>();
        if (cloneGroup != null) cloneGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Yang bergerak mengikuti kursor adalah KLON-nya, bukan kartu aslinya
        if (dragClone != null)
        {
            dragClone.transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 1. Hapus Klon dari layar (tugasnya sudah selesai)
        if (dragClone != null)
        {
            Destroy(dragClone);
        }

        // 2. Kembalikan kartu ASLI ke kondisi terang dan normal
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        transform.localScale = originalScale;

        // Catatan Sistem:
        // Jika kursor saat ini ada di atas "BuildingDropZone", script bangunan tersebut 
        // yang akan mengambil alih memindahkan kartu asli ini.
        // Jika tidak, kartu ini sudah otomatis kembali normal di dalam Tangan.
    }
}