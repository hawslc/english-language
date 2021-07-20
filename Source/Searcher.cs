using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Searcher : MonoBehaviour
{
  List<string> words = new List<string>();
  public TMP_InputField word;
  public TMP_InputField length;
  public TextMeshProUGUI output;
  public RectTransform content;

  public void Start()
  {
    words = Dictionary.GetWords();
    Search();
  }
  public void Search()
  {
    output.text = "";
    int count = 0;

    if (word.text.Length == 0)
    {
      output.text = "Start typing \nto find words...";
      return;
    }

    //go through each word and add it to the output if it works
    for (int i = 0; i < words.Count; i++)
    {
      if (words[i].Length >= word.text.Length && words[i].Substring(0, word.text.Length) == word.text)
      {
        int num = 0;
        bool success = Int32.TryParse(length.text, out num);
        if (success)
        {
          //there is a length restriction
          if (words[i].Length == num)
          {
            output.text += words[i] + "\n";
            count++;
          }
        } else {
          //no length restriction
          output.text += words[i] + "\n";
          count++;
        }
      }
    }

    //done searching, update sizes of things
    content.sizeDelta = new Vector2(0, 55.6f * count);
    content.GetChild(0).localPosition = new Vector2(140, 0);
  }
}
