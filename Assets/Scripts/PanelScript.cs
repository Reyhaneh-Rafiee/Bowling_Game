using UnityEngine;
using UnityEngine.SceneManagement;

public class PanelScrip : MonoBehaviour
{
    public void LoadGameScene()
    {
        SceneManager.LoadScene("GameScene");
    }
}
