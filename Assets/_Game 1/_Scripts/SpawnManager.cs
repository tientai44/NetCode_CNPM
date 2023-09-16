using System.Collections;
using UnityEngine;
using Unity.Netcode;

public class SpawnManager : NetworkBehaviour
{
    public GameObject objectPrefab; // Prefab của đối tượng bạn muốn tạo
    public Transform SpawnPos; // Prefab của đối tượng bạn muốn tạo
    [SerializeField]
    [Range(0f, 10f)]
    public float spawnInterval = 1.0f; // Khoảng thời gian giữa mỗi lần spam
    private float timer = 0f;

    private void Update()
    {
        if (IsServer)
        {
            // Kiểm tra thời gian để spam đối tượng
            timer += Time.deltaTime;
            if (timer >= spawnInterval)
            {
                spawnInterval = Random.Range(0, 10);
                SpawnObject(); // Gọi hàm SpawnObject khi đến thời gian spam (chỉ server mới thực hiện)

                // Reset thời gian
                timer = 0f;
            }
        }
    }

    private void SpawnObject()
    {
        // Tạo một đối tượng trên mạng sử dụng SpawnManager

        NetworkObject spawnedObject = Instantiate(objectPrefab, SpawnPos.position, Quaternion.identity).GetComponent<NetworkObject>();
        spawnedObject.Spawn();

        // Đăng ký đối tượng với NetworkManager để nó có thể được đồng bộ hóa
    }
}
