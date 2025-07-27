using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using TMPro;
using Photon.Pun;

public class RoomListItem : MonoBehaviour
{
    [SerializeField] private TMP_Text text; // Text component to display the room name

    public RoomInfo roomInfo; // Reference to the RoomInfo object representing the room

    public void SetUp(RoomInfo roomInfo)
    {
        this.roomInfo = roomInfo; // Assign the RoomInfo object to the local variable
        text.text = roomInfo.Name; // Set the text component to display the room name
    }

    public void OnClick()
    {
        Launcher.Instance.JoinRoom(roomInfo); // Call the JoinRoom method in the Launcher instance to join the selected room
    }
}
