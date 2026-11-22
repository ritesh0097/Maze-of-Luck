using UnityEngine;

public class ExitDoorInteractionScript : MonoBehaviour
{

    [SerializeField] GameObject doorUnlockPanel;
    [SerializeField] PasswordInput passwordInput;

    Animator animator;

    bool isDoorOpen;

    public bool isDoorLock = true;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (passwordInput.isDoorUnlock)
        {
            doorUnlockPanel.SetActive(true);
        }
        else
        {
            OpenDoor();
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && !passwordInput.isDoorUnlock && !isDoorOpen)
        {
            OpenDoor();
        }
    }

    void OpenDoor()
    {
        doorUnlockPanel.SetActive(false);
        animator.SetBool("IsExitDoorOpen", true);

        if (!isDoorOpen)
        {
            AudioManager.Instance.DoorOpeningSound();
            isDoorOpen = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            doorUnlockPanel.SetActive(false);
            animator.SetBool("IsExitDoorOpen", false);

            if (isDoorOpen)
            {
                AudioManager.Instance.DoorClosingSound();
                isDoorOpen = false;
            }
        }
    }

}
