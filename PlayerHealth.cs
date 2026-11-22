using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] Image healthSlider;

    [SerializeField] float playerHealthAmount;
    [SerializeField] GameObject playerDiedPanel;

    

    public float playerGameHealth;


    // Respawn Settings
    public Transform[] respawnPoints;
    public float respawnDelay = 3f;

    // Alive Status
    bool isDead = false;

    // Player hit sound
    bool isPlayerHit = false;


    void Start()
    {
        playerGameHealth = playerHealthAmount;        
    }

    void Update()
    {
        healthSlider.fillAmount = playerGameHealth / playerHealthAmount;

        if (playerGameHealth >= playerHealthAmount)
        {
            playerGameHealth = playerHealthAmount;
        } 
        else if (playerGameHealth <= 0)
        {
            // playerGameHealth = 0;
            Die();
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Axe"))
        {
            playerGameHealth -= 15;
            
            isPlayerHit = true;

            if (isPlayerHit)
            {
                AudioManager.Instance.PlayerHitSound();
                isPlayerHit = false;
            }
        }
    }

    void Die()
    {
        if (isDead)
            return;

        isDead = true;

        playerDiedPanel.SetActive(true);        

        StartCoroutine(RespawnPlayer());
    }

    IEnumerator RespawnPlayer()
    {
        yield return new WaitForSeconds(respawnDelay);

        if (respawnPoints == null || respawnPoints.Length == 0)
        {            
            yield break;
        }

        // Select random spawn point
        int randomIndex = Random.Range(0, respawnPoints.Length);

        Transform spawnPoint = respawnPoints[randomIndex];

        // Move player to spawn point
        transform.SetPositionAndRotation(
            spawnPoint.position,
            spawnPoint.rotation
        );

        // Reset health
        playerGameHealth = playerHealthAmount;        

        isDead = false;

        playerDiedPanel.SetActive(false);
        
    }

}
