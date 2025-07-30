using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerNameManager : MonoBehaviour
{
    [SerializeField] TMP_InputField playerNameInputField;

    private void Start()
    {
        if (PlayerPrefs.HasKey("username"))
        {
            playerNameInputField.text = PlayerPrefs.GetString("username");
            PhotonNetwork.NickName = playerNameInputField.text;
        }
        else
        {
            playerNameInputField.text = "Player" + Random.Range(1000, 9999).ToString();
            OnUserNameInputValueChanged();
        }
    }

    public void OnUserNameInputValueChanged()
    {
        PhotonNetwork.NickName = playerNameInputField.text;
        PlayerPrefs.SetString("username", playerNameInputField.text);
    }
}
