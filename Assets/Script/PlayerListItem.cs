using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class PlayerListItem : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_Text playerName; // Reference to the TextMeshPro component for displaying player names
    private Player player; // Reference to the Photon player

    public void SetUp(Player player)
    {
        this.player = player; // Set the player reference
        playerName.text = player.NickName; // Display the player's nickname
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (player == otherPlayer)
        {
            Destroy(gameObject); // Destroy this GameObject if the player has left the room
        }
    }

    public override void OnLeftRoom()
    {
        Destroy(gameObject); // Destroy this GameObject when the local player leaves the room
    }
}
