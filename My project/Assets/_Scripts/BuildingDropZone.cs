using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingDropZone : MonoBehaviour, IDropHandler
{
    // Hubungkan ini ke script Building.cs di Inspector
    public Building buildingLogic;

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            CardDrag draggedCard = eventData.pointerDrag.GetComponent<CardDrag>();

            if (draggedCard != null)
            {
                // Tanya ke logika bangunan: Boleh tidak kartu ini masuk ke sini?
                if (buildingLogic.TryPlaceCard(draggedCard.cardUI))
                {
                    // Sukses masuk!
                    Debug.Log($"[BANGUNAN] {draggedCard.cardUI.myData.cardName} dipasang ke {buildingLogic.buildingType}");

                    // Kunci kartu di bangunan ini
                    draggedCard.transform.SetParent(this.transform);

                    // Matikan script drag agar kartu tidak bisa ditarik lagi setelah dipasang
                    draggedCard.enabled = false;

                    // Panggil HandManager untuk menghapus data kartu dari tangan
                    HandManager.Instance.RemoveCardFromHand(draggedCard.cardUI.myData);
                }
                else
                {
                    Debug.LogWarning("[BANGUNAN] Kartu tidak kompatibel atau slot penuh!");
                    // Kartu otomatis akan kembali ke tangan karena OnEndDrag di CardDrag.cs
                }
            }
        }
    }
}