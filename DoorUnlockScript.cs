using UnityEngine;

public class DoorUnlockScript : MonoBehaviour
{

    [SerializeField] GameObject doorUnlockPanel;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            doorUnlockPanel.SetActive(true);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            doorUnlockPanel.SetActive(false);
        }
    }


}
