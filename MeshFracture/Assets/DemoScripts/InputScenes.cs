using UnityEngine;

public class InputScenes : MonoBehaviour
{
    public static InputScenes Instance { get; private set; }

    private int sceneIndex;
    private int maxSceneIndex;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        maxSceneIndex = UnityEngine.SceneManagement.SceneManager.sceneCount;
        sceneIndex = 0;
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            sceneIndex++;

            if (sceneIndex > maxSceneIndex)
                sceneIndex = 0;

            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Scene" + sceneIndex);
            Debug.Log("New game Scene : " + sceneIndex);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            sceneIndex--;

            if (sceneIndex < 0)
                sceneIndex = maxSceneIndex;

            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Scene" + sceneIndex);
            Debug.Log("New game Scene : " + sceneIndex);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("Reload : " + sceneIndex);
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Scene" + (sceneIndex));
        }
    }
}
