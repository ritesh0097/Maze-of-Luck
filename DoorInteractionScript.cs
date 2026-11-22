using UnityEngine;

public class DoorInteractionScript : MonoBehaviour
{
    Animator animator;

    bool IsDoorOpen = true;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            animator.SetBool("IsDoorOpen", true);

            if (IsDoorOpen)
            {
                AudioManager.Instance.DoorOpeningSound();                
                IsDoorOpen = false;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            animator.SetBool("IsDoorOpen", false);

            if (!IsDoorOpen)
            {                
                AudioManager.Instance.DoorClosingSound();         
                IsDoorOpen = true;
            }
        }
    }
}
