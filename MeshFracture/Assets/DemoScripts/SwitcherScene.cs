using UnityEngine;

public class SwitcherScene : MonoBehaviour
{
    public static SwitcherScene Instance { get; private set; }

    private int sceneIndex = 1;
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
        Debug.Log("Max Scene Index: " + maxSceneIndex);
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (sceneIndex > maxSceneIndex)
                sceneIndex = 0;

            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Scene" + sceneIndex);
            Debug.Log("Loading Scene" + sceneIndex);
            sceneIndex++;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (sceneIndex < 0)
                sceneIndex = maxSceneIndex;

            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Scene" + sceneIndex);
            Debug.Log("Loading Scene" + sceneIndex);
            sceneIndex--;
        }

    }
}
