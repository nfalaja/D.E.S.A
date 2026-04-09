using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI; // WAJIB ada untuk memodifikasi LayoutElement

public class BuildingDropZone : MonoBehaviour, IDropHandler
{
    [Header("Referensi Logika & Visual")]
    public Building buildingLogic;
    public Transform miniCardContainer; // Rak khusus di atas bangunan

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            CardDrag draggedCard = eventData.pointerDrag.GetComponent<CardDrag>();

            if (draggedCard != null)
            {
                if (buildingLogic.TryPlaceCard(draggedCard.cardUI))
                {
                    // 1. Pindahkan parent ke rak
                    draggedCard.transform.SetParent(miniCardContainer);

                    // 2. Kunci kartu
                    draggedCard.isLocked = true;

                    UnityEngine.UI.Button tombolKartu = draggedCard.GetComponent<UnityEngine.UI.Button>();
                    if (tombolKartu != null)
                    {
                        tombolKartu.enabled = false; // Tombol mati total, tidak bisa memicu pembelian lagi
                    }

                    HandManager.Instance.RemoveCardFromHand(draggedCard.cardUI.myData);
                }
                else
                {
                    Debug.LogWarning("[BANGUNAN] Kartu tidak kompatibel atau slot penuh!");
                }
            }
        }
    }
}