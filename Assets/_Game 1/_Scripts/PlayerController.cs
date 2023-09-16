using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    public int score = 0;
    public GameObject bulletPrefab;

    [SerializeField] private float moveSpeed = 15f;

    private float hAxis;
    private float vAxis;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // Check if this is the local player
        if (IsLocalPlayer)
        {
            // Set the initial position for the local player
            transform.position = new Vector3(-5f, 0f, 0f);
        }
    }

    private void Update()
    {
        if (!IsLocalPlayer)
            return; // Only run the code for the local player

        GetInput();
        Move();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shot();
        }
    }

    private void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
    }

    private void Shot()
    {
        // Instantiate bullet locally and spawn it on the server
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

        if (NetworkManager.Singleton.IsServer)
        {
            // If running on the server, spawn the bullet directly
            NetworkObject networkObject = bullet.GetComponent<NetworkObject>();
            networkObject.Spawn();
        }
        else
        {
            // If running on a client, send a client-to-server RPC to spawn the bullet
            ServerSpawnBulletServerRpc();
        }
    }

    // Server method to spawn the bullet
    [ServerRpc]
    private void ServerSpawnBulletServerRpc()
    {
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        NetworkObject networkObject = bullet.GetComponent<NetworkObject>();
        networkObject.Spawn();
    }

    private void Move()
    {
        Vector3 direction = new Vector3(hAxis, vAxis, 0).normalized;
        transform.position += direction * Time.deltaTime * moveSpeed;
    }
}
