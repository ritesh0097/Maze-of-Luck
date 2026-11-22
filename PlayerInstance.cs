using UnityEngine;

public class PlayerInstance : MonoBehaviour
{
    public static PlayerInstance Instance { get; private set; }

    public Transform Player { get; private set; }
    public Transform PlayerHead { get; private set; }
    public Transform GroundPlayer { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        var playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject == null)
            return;

        Player = playerObject.transform;
        PlayerHead = Player.Find("Head") ?? Player;
        GroundPlayer = Player.Find("GroundPlayer") ?? Player;
    }
}
