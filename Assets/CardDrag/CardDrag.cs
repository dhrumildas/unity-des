using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardDrag : MonoBehaviour
{
    public GameObject instructionsPanel;
    public GameObject clock;
    public GameObject card;

    public void DisablePanel()
    {
        instructionsPanel.SetActive(false);
        clock.SetActive(true);
        card.SetActive(true);
    }

    
}