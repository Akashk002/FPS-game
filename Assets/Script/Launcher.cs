using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher Instance; // Singleton instance of the Launcher class for easy access from other scripts

    [SerializeField] private TMP_InputField roomNameInputField; // Input field for player name, serialized for easy access in the Unity Editor
    [SerializeField] private TMP_Text errorText;
    [SerializeField] private TMP_Text roomNameText;
    [SerializeField] private Transform roomListContent; // Transform for the content area of the room list, serialized for easy access in the Unity Editor
    [SerializeField] private GameObject roomListPrefab; // Prefab for displaying individual room items in the room list, serialized for easy access in the Unity Editor
    [SerializeField] private Transform playerListContent;
    [SerializeField] private GameObject playerListPrefab;
    [SerializeField] private GameObject startRoomButton;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Launcher Start"); // Log message to console for debugging purposes
        PhotonNetwork.ConnectUsingSettings(); // Connect to Photon server using settings defined in the PhotonServerSettings file
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master Server"); // Log message when connected to the master server
        PhotonNetwork.JoinLobby(); // Join the default lobby when connected to the master server
        PhotonNetwork.AutomaticallySyncScene = true; // Enable automatic scene synchronization across all clients in the room
    }

    public override void OnJoinedLobby()
    {
        MenuManager.Instance.OpenMenu("main"); // Open the main menu using the MenuManager singleton instance
        PhotonNetwork.NickName = "Player" + Random.Range(0, 1000).ToString("0000");
    }

    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(roomNameInputField.text))
        {
            return;
        }

        PhotonNetwork.CreateRoom(roomNameInputField.text); // Create a room with the specified name and maximum players
        MenuManager.Instance.OpenMenu("loading");
    }

    public override void OnJoinedRoom()
    {
        MenuManager.Instance.OpenMenu("room"); // Open the room menu when successfully joined a room
        roomNameText.text = PhotonNetwork.CurrentRoom.Name; // Display the current room name in the UI

        Player[] players = PhotonNetwork.PlayerList;

        foreach (Transform child in playerListContent)
        {
            Destroy(child.gameObject); // Clear existing player list items before populating with current players
        }

        for (int i = 0; i < players.Length; i++)
        {
            Instantiate(playerListPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i]);
        }

        startRoomButton.SetActive(PhotonNetwork.IsMasterClient); // Show the start room button only if the current player is the master client 
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startRoomButton.SetActive(PhotonNetwork.IsMasterClient); // Show the start room button only if the current player is the master client
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError("Create Room Failed: " + message); // Log error message if room creation fails

        errorText.text = "Failed to create room: " + message; // Display error message in the UI
        MenuManager.Instance.OpenMenu("error"); // Reopen the main menu if room creation fails
    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel(1); // Load the game scene when the player starts the game
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom(); // Leave the current room
        MenuManager.Instance.OpenMenu("loading"); // Open the main menu after leaving the room
    }
    public override void OnLeftRoom()
    {
        Debug.Log("Left Room"); // Log message when successfully left a room
        MenuManager.Instance.OpenMenu("main"); // Open the main menu after leaving the room
    }

    public void JoinRoom(RoomInfo roomInfo)
    {
        PhotonNetwork.JoinRoom(roomInfo.Name); // Join the specified room by its name
        MenuManager.Instance.OpenMenu("loading"); // Open the loading menu while joining the room
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform child in roomListContent)
        {
            Destroy(child.gameObject); // Destroy existing room list items to refresh the list
        }
        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].RemovedFromList) continue; // Skip rooms that have been removed from the list 
            Instantiate(roomListPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(roomList[i]); // Instantiate new room list items for each room in the updated list
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(playerListPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
    }
}
