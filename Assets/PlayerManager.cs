using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerManager : MonoBehaviour
{
    private PhotonView PV; // Reference to the PhotonView component for network synchronization

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
        Debug.Log("CreateController called"); // Log when the CreateController method is called
    }
}
