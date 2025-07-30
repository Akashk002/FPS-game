using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using System.IO;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerManager : MonoBehaviour
{
    private PhotonView PV; // Reference to the PhotonView component for network synchronization
    private GameObject controller; // Reference to the PlayerController GameObject
    private int kills;
    public int deaths; // Track the number of deaths for the player

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

        deaths++;
        Hashtable hash = new Hashtable();
        hash.Add("deaths", deaths); // Update the kills count in the player's custom properties
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash); // Set the updated properties for the local player
    }

    public void GetKill()
    {
        PV.RPC(nameof(RPC_GetKill), PV.Owner); // Call the RPC to update the kills count across all clients
    }

    [PunRPC]
    private void RPC_GetKill()
    {
        kills++;
        Hashtable hash = new Hashtable();
        hash.Add("kills", kills); // Update the kills count in the player's custom properties
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash); // Set the updated properties for the local player
    }

    public static PlayerManager Find(Player player)
    {
        return FindObjectsByType<PlayerManager>(FindObjectsSortMode.None).SingleOrDefault(pm => pm.PV.Owner == player);
    }
}
