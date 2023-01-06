using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    public string mainSceneName = "SampleScene";
    
    public void StartGame()
    {
        SceneManager.LoadSceneAsync(mainSceneName);
    }
}
