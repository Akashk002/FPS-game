using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerController : MonoBehaviourPunCallbacks, IDamageable
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject cameraHolder;
    [SerializeField] private float mouseSensitivity;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float smoothTime;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private PhotonView pv;
    [SerializeField] private Item[] items;

    private int itemIndex;
    private int previousItemIndex = -1;
    private float verticalLookRotaion;
    private bool grounded;
    private Vector3 smoothMoveVelocity;
    private Vector3 moveAmount;

    const float maxHealth = 100f;
    private float currentHealth = maxHealth;

    PlayerManager playerManager;

    private void Awake()
    {
        playerManager = PhotonView.Find((int)pv.InstantiationData[0]).GetComponent<PlayerManager>();
    }
    private void Start()
    {
        if (pv.IsMine)
        {
            EquipItem(0);
        }
        else
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(rb);
            canvas.gameObject.SetActive(false);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (!pv.IsMine) return;

        Look();
        Move();
        Jump();

        for (int i = 0; i < items.Length; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                EquipItem(i);
                break;
            }
        }

        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
        {
            if (itemIndex >= items.Length - 1)
            {
                EquipItem(0);
            }
            else
            {
                EquipItem(itemIndex + 1);
            }
        }
        else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
        {
            if (itemIndex <= 0)
            {
                EquipItem(items.Length - 1);
            }
            else
            {
                EquipItem(itemIndex - 1);
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            items[itemIndex].Use();
        }

        if (transform.position.y < -10)
        {
            Die();
        }
    }

    private void FixedUpdate()
    {
        if (!pv.IsMine) return;
        rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            rb.AddForce(transform.up * jumpForce);
        }
    }

    private void Move()
    {
        Vector3 movedir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        moveAmount = Vector3.SmoothDamp(moveAmount, movedir * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed), ref smoothMoveVelocity, smoothTime);
    }

    private void EquipItem(int index)
    {
        if (index == previousItemIndex)
            return;

        itemIndex = index;
        items[itemIndex].itemGameObject.SetActive(true);

        if (previousItemIndex != -1)
        {
            items[previousItemIndex].itemGameObject.SetActive(false);
        }

        previousItemIndex = itemIndex;

        if (pv.IsMine)
        {
            Hashtable hash = new Hashtable();
            hash.Add("ItemIndex", itemIndex);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (!pv.IsMine && targetPlayer == pv.Owner)
        {
            EquipItem((int)changedProps["ItemIndex"]);
        }
    }

    void Look()
    {
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity);
        verticalLookRotaion += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
        verticalLookRotaion = Mathf.Clamp(verticalLookRotaion, -90f, 90f);
        cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotaion;
    }

    public void SetGroundedState(bool grounded)
    {
        this.grounded = grounded;
    }

    public void TakeDamage(float damage)
    {
        pv.RPC("RPC_TakeDamage", RpcTarget.All, damage);
    }

    [PunRPC]
    void RPC_TakeDamage(float damage)
    {
        if (!pv.IsMine) return;

        // Handle taking damage here, e.g., reduce health, play animation, etc.
        Debug.Log($"Player {pv.Owner.NickName} took {damage} damage.");

        currentHealth -= damage;

        Debug.Log($"Player {pv.Owner.NickName} has died.");
        // Handle player death, e.g., respawn or end game
        healthSlider.value = currentHealth / maxHealth;
        if (currentHealth <= 0)
        {
            Die();
        }

    }

    private void Die()
    {
        playerManager.Die(); // Call the PlayerManager's Die method to handle player death
    }
}
