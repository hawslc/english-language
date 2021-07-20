using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsMenu : MonoBehaviour
{
  public GameObject canvas;

  public void ToggleCanvas()
  {
    canvas.SetActive(!canvas.activeSelf);
  }

  void Update()
  {
    if (Input.GetKeyDown("escape"))
    {
      ToggleCanvas();
    }
  }

  public void ButtonPress(int i)
  {
    SceneManager.LoadScene(i);
  }
}
