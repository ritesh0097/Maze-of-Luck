using UnityEngine;
using UnityEngine.EventSystems;

public class GameWin : MonoBehaviour
{

    [SerializeField] GameObject escapePanel;
    [SerializeField] GameObject playAgainButton;

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            escapePanel.SetActive(true);
            EventSystem.current.SetSelectedGameObject(playAgainButton);
        }
    }
}
