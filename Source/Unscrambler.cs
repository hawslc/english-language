using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using TMPro;

public class Unscrambler : MonoBehaviour
{
  List<string[]> dict = new List<string[]>();
  List<string[]> dictSuper = new List<string[]>();
  public TMP_InputField input;
  public TextMeshProUGUI output;
  public RectTransform  content;
  bool super = false;
  bool sort = false;

  void Start()
  {
    Unscramble();
    dictSuper = Dictionary.GetDictionary(0);
  }

  public void Unscramble()
  {
    //Unscrambles a single word
    output.text = "";
    string text = input.text;
    string buffer = text.ToLower();
    List<string> results = new List<string>();

    if (text.Length > 25)
    {
      output.text = "Too long!";
      return;
    }
    if (text.Length == 0)
    {
      output.text = "Start typing \nto find words...";
      return;
    }

    string alphabet = Dictionary.GetAlphabet();

    if (super)
    {
      dict = dictSuper;
    } else {
      dict = Dictionary.GetDictionary(text.Length);
    }


    buffer = SortString(buffer);
    int count = 0;

    for (int i = 0; i < alphabet.Length; i++)
    {
      if (text.IndexOf(alphabet[i]) != -1)
      {
        //this letter is in the word to unscramble
        //now go through all of the words, sort them, and compare them to the sorted original word
        for (int k = 0; k < dict[i].Length; k++)
        {
          string word = SortString(dict[i][k]);
          if (word == buffer)
          {
            //the word is a match
            count++;
            output.text += dict[i][k] + "\n";
            results.Add(dict[i][k]);
          } else {
            if (super)
            {
              //detect subwords
              //if the user typed in "bear", this will find "bar"
              //aber  abr
              int j = 0; //the index we are at on the word the user typed in
              int y = 0; //the index we are at on the word we are checking
              //for each letter in the search word, check for a corresponding letter in the word being checked
              while (j < buffer.Length && y < word.Length)
              {
                //if the letter matches, increase j
                if (word[y] == buffer[j])
                {
                  y++;
                }
                j++;
              }

              if (y == word.Length)
              {
                //the word is a match
                count++;
                output.text += dict[i][k] + "\n";
                results.Add(dict[i][k]);
              }
            }
          }
        }
      }
    }

    //if sort by length, do so
    if (sort)
    {
      results = results.OrderBy(o => 100 - o.Length).ToList();

      output.text = "";

      for (var i = 0; i < results.Count; i++)
      {
        output.text += results[i] + "\n";
      }
    }

    //if none, say so
    if (output.text == "") output.text = "Nothing was \nfound :(";

    //done searching, update sizes of things
    content.sizeDelta = new Vector2(0, 55.6f * count);
    content.GetChild(0).localPosition = new Vector2(140, 0);
  }

  public void ToggleSuper()
  {
    super = !super;
    Unscramble();
  }

  public void ToggleSort()
  {
    sort = !sort;
    Unscramble();
  }

  string SortString(string input)
  {
    char[] c = input.ToArray();
    Array.Sort(c);
    return new string(c);
  }
}
