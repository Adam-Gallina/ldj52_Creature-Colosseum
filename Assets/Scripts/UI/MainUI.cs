using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainUI : MonoBehaviour
{
    public void PressTutorial()
    {
        SceneManager.LoadScene(2);
    }

    public void PressBuilder()
    {
        SceneManager.LoadScene(1);
    }

    public void PressPvP()
    {
        SceneManager.LoadScene(3);
    }
}
