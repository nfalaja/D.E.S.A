using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class TrashDropZone : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;

        CardDrag draggedCard = eventData.pointerDrag.GetComponent<CardDrag>();

        if (draggedCard == null) return;

        Debug.Log($"[SAMPAH] Membuang kartu {draggedCard.cardUI.myData.cardName}");

        // 1. Hapus dari data tangan
        HandManager.Instance.RemoveCardFromHand(draggedCard.cardUI.myData);

        // 2. Matikan interaksi dulu (biar EventSystem tidak error)
        draggedCard.enabled = false;

        CanvasGroup cg = draggedCard.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.blocksRaycasts = false;
            cg.interactable = false;
            cg.alpha = 0f; // optional: langsung hilang
        }

        // 3. Destroy dengan delay 1 frame (INI YANG PALING PENTING)
        StartCoroutine(DestroyNextFrame(draggedCard.gameObject));
    }

    private IEnumerator DestroyNextFrame(GameObject obj)
    {
        yield return null; // tunggu 1 frame supaya EventSystem selesai proses
        Destroy(obj);
    }
}