using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    // [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource sfxSource;

    [Header("Audio Clips")]
    // [SerializeField] AudioClip backgroundMusic;
    [SerializeField] AudioClip doorOpeningSound;    
    [SerializeField] AudioClip doorClosingSound;
    [SerializeField] AudioClip playerHitSound;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }


    public void DoorOpeningSound()
    {
        sfxSource.PlayOneShot(doorOpeningSound);
    }
        
    public void DoorClosingSound()
    {
        sfxSource.PlayOneShot(doorClosingSound);
    }    
    
    public void PlayerHitSound()
    {
        sfxSource.PlayOneShot(playerHitSound, 10f);
    }    
}