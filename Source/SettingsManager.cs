using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class SettingsManager : MonoBehaviour
{
  public TMP_Dropdown input;

  void Start()
  {
    int num = Dictionary.fileSize;

    //update the dropdown to show current dictionary

    for (int i = 0; i < input.options.Count; i++)
    {
      if (num.ToString() == input.options[i].text) input.value = i;
    }
  }

  public void Change()
  {
    //change the dictionary based on the dropdown
    int number;

    bool success = Int32.TryParse(input.options[input.value].text, out number);

    if (success)
    {
      Dictionary.fileSize = number;
    }
  }
}
