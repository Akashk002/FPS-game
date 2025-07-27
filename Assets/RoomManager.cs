using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance; // Singleton instance of the RoomManager class for easy access from other scripts

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; // Assign this instance to the singleton instance if it is null
        }
        else
        {
            Destroy(gameObject); // Destroy this instance if another instance already exists
        }

        DontDestroyOnLoad(gameObject); // Ensure this object persists across scene loads
    }

    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded; // Subscribe to the sceneLoaded event to handle scene changes
    }

    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded; // Unsubscribe from the sceneLoaded event to avoid memory leaks
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.buildIndex == 1)
        {
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity); // Instantiate the player prefab in the game scene when it is loaded
        }
    }
}
