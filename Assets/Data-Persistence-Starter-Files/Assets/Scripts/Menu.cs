using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{

    public TMP_InputField playerNameInput;
   public void StartGame()
   {

     // Save player name using PlayerPrefs
        string playerName = playerNameInput.text;
        PlayerPrefs.SetString("PlayerName", playerName);
        PlayerPrefs.Save();

    SceneManager.LoadScene(1);
   }

   public void Back()
   {
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex-1);
   }

   public void QuitGame()
   {
    #if UNITY_EDITOR
    EditorApplication.ExitPlaymode();
    #endif
    Application.Quit();
   }
}
