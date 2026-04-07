using UnityEngine;
using UnityEngine.EventSystems;

public class TrashDropZone : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            CardDrag draggedCard = eventData.pointerDrag.GetComponent<CardDrag>();

            if (draggedCard != null)
            {
                Debug.Log($"[SAMPAH] Membuang kartu {draggedCard.cardUI.myData.cardName}");

                // Hapus dari memori Hand Manager agar slot tangan kosong
                HandManager.Instance.RemoveCardFromHand(draggedCard.cardUI.myData);

                // Hancurkan fisik kartu
                Destroy(draggedCard.gameObject);
            }
        }
    }
}