using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PlayerManager : MonoBehaviour
{
    private PhotonView PV; // Reference to the PhotonView component for network synchronization
    private GameObject controller; // Reference to the PlayerController GameObject

    private void Awake()
    {
        PV = GetComponent<PhotonView>(); // Get the PhotonView component attached to this GameObject
    }

    private void Start()
    {
        if (PV.IsMine)
        {
            CreateController(); // Call CreateController if this player is the local player
        }
    }

    void CreateController()
    {
        Transform spawnPoint = SpawnManager.Instance.GetSpawnpoint(); // Get a random spawn point from the SpawnManager

        controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), spawnPoint.position, spawnPoint.rotation, 0, new object[]
        {
            PV.ViewID // Pass the PhotonView ID to the PlayerController
        });
    }

    public void Die()
    {
        PhotonNetwork.Destroy(controller); // Destroy the PlayerController GameObject when the player dies
        CreateController(); // Create a new PlayerController for respawning
    }
}
