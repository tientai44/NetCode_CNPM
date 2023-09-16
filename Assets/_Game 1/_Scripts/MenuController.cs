using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    bool m_anyKey = true;

    public GameObject Container;
    public GameObject AnyKey;

    [SerializeField]
    private SceneName nextScene = SceneName.CharacterSelection;

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => NetworkManager.Singleton.SceneManager != null);
        LoadingSceneManager.Instance.Init();
    }
    private void Update()
    {
        if (m_anyKey)
        {
            if (Input.anyKey)
            {
                m_anyKey = false;
                AnyKey.SetActive(false);
                Container.SetActive(true);
            }
        }
    }

    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();

        Container.SetActive(false);
        //// Tải scene trước khi khởi động host
        LoadingSceneManager.Instance.LoadScene(nextScene);
    }

    public void StartClient()
    {
        // Tải scene trước khi tham gia dưới vai trò client
        NetworkManager.Singleton.StartClient();
        Container.SetActive(false);

        LoadingSceneManager.Instance.LoadScene(nextScene);
    }

    public void PointerHover(Image image)
    {
        image.color = Color.red;
    }
    public void PointerExit(Image image)
    {
        image.color = Color.white;
    }
}
