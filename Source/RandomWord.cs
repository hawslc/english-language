using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.IO;

public class RandomWord : MonoBehaviour
{
  public TextMeshProUGUI text;
  List<string> records = new List<string>();

  void Start()
  {
    records = Dictionary.GetWords();
    Generate();

    /*
    code to change files to right format
    string filePath = Application.streamingAssetsPath + "/" + Dictionary.fileSize + " words.csv";
    StreamReader reader = new StreamReader(filePath);
    var file = reader.ReadToEnd();
    reader.Close();

    File.WriteAllText(filePath, "huh");
    StreamWriter sw = new StreamWriter(filePath);


    List<string> words = new List<string>(file.ToLower().Split('\n'));

    for (int i = 0; i < words.Count; i++)
    {
      if (words[i].Length <= 1)
      {
        words.RemoveAt(i);
        i--;
        continue;
      }

      //clean up different formats
      if (words[i][0] == '\"') words[i] = words[i].Substring(1, words[i].Length - 2);
      if (words[i][words[i].Length - 1] == '\r') words[i] = words[i].Substring(0, words[i].Length - 1);

      sw.WriteLine(words[i]);
    }

    sw.Close();
    */
  }

  public void Generate()
  {
    //pick a random index and display the word from that index
    int number = UnityEngine.Random.Range(0, records.Count);
    text.text = records[number];
  }
}
