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
        sceneIndex = 1;
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Scene" + sceneIndex);
            sceneIndex++;
            
            if (sceneIndex > maxSceneIndex)
                sceneIndex = 0;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Scene" + sceneIndex);
            sceneIndex--;
           
            if (sceneIndex < 0)
                sceneIndex = maxSceneIndex;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Scene" + (sceneIndex - 1));
        }
    }
}
