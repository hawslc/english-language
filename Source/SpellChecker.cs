using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using TMPro;

public class SpellChecker : MonoBehaviour
{
  List<string> words = new List<string>();
  List<string[]> dict = new List<string[]>();
  public TMP_InputField inputText;
  bool super = false;
  bool ignore = false;

  public void Start()
  {
    words = Dictionary.GetWords();
    dict = Dictionary.GetDictionary(0);
    words.Add("i");
    words.Add("a");
  }

  public void SpellCheck()
  {
    string t = inputText.text;

    string[] input = t.Split(' ');

    for (int i = 0; i < input.Length; i++)
    {
      string s = input[i].ToLower();
      if (s.Length < 1) continue;
      string allowed = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPLKJHGFDSAZXCVBNM ";
      if (s[0] == '<') continue;
      for (int k = 0; k < s.Length; k++)
      {
        if (allowed.IndexOf(s[k]) == -1)
        {
          s = s.Substring(0, k) + s.Substring(k + 1, s.Length - k - 1);
          k--;
        }
      }
      if (words.IndexOf(s) == -1 || ignore)
      {
        string w = CorrectWord(s);
        if (w == "")
        {
          input[i] = "<" + input[i] + ">";
        } else {
          input[i] = "" + input[i] + "(" + w + ")";
        }
      }
    }

    inputText.text = String.Join(" ", input);
  }

  public string CorrectWord(string word)
  {
    //corrects a word
    //already in standardized formats
    List<string> permutations = new List<string>();
    string s = word;
    string alphabet = Dictionary.GetAlphabet();

    //operations done first have higher chance of being right - swapping letters is more common than deletions

    //first swap every pair of letters
    for (int i = 1; i < s.Length - 1; i++)
    {
      permutations.Add(s.Substring(0, i) + s[i + 1] + s[i] + s.Substring(i + 2, s.Length - i - 2));
    }

    //then go through all single letter deletions
    for (int i = 1; i < s.Length; i++)
    {
      permutations.Add(s.Substring(0, i) + s.Substring(i + 1, s.Length - i - 1));
    }

    //now go through every single letter insertion
    for (int i = 1; i <= s.Length; i++)
    {
      //each letter
      for (int k = 0; k < alphabet.Length; k++)
      {
        //each letter to replace it
        permutations.Add(s.Substring(0, i) + alphabet[k] + s.Substring(i, s.Length - i));
      }
    }

    //now replace every letter in the word with every possible letter
    for (int i = 1; i < s.Length; i++)
    {
      //each letter
      for (int k = 0; k < alphabet.Length; k++)
      {
        //each letter to replace it
        permutations.Add(s.Substring(0, i) + alphabet[k] + s.Substring(i + 1, s.Length - i - 1));
      }
    }

    //super
    //now go do some extra checks for super
    if (super)
    {
      string current = "";

      //every swap and then deletion
      for (int i = 1; i < s.Length - 1; i++)
      {
        current = s.Substring(0, i) + s[i + 1] + s[i] + s.Substring(i + 2, s.Length - i - 2);

        for (int k = 1; k < current.Length; k++)
        {
          permutations.Add(current.Substring(0, k) + current.Substring(k + 1, current.Length - k - 1));
        }
      }

      //adding a letter to the front
      for (int k = 0; k < alphabet.Length; k++)
      {
        permutations.Add(alphabet[k] + s);
      }

      //changing the first letter
      for (int k = 0; k < alphabet.Length; k++)
      {
        permutations.Add(alphabet[k] + s.Substring(1));
      }

      //now go through every deletion and try an insertion

      for (int i = 0; i < s.Length; i++)
      {
        //each deletion spot
        string t = s.Substring(0, i) + s.Substring(i + 1);
        for (int k = 0; k < alphabet.Length; k++)
        {
          //each letter
          for (int j = 1; j <= t.Length; j++)
          {
            //each insertion
            permutations.Add(t.Substring(0, j) + alphabet[k] + t.Substring(j));
          }
        }
      }
    }
    //end super

    List<string> matches = new List<string>(); //words with a Levensthien distance of 1, or 2 if super

    for (int i = 0; i < permutations.Count; i++)
    {
      //print(permutations[i] + " is from " + word);

      if (Array.IndexOf(dict[LetterToIndex(permutations[i][0])], permutations[i]) != -1)
      {
        matches.Add(permutations[i]);
      }
    }

    if (matches.Count > 0)
    {
      string w = "";

      matches = matches.Distinct().ToList();

      for (int i = 0; i < matches.Count; i++)
      {
        w += matches[i] + ", ";
      }

      return w.Substring(0, w.Length - 2);
    }
    return "";
  }

  public void Undo()
  {
    //cleans up all modifications of text from the spell checker
    string s = inputText.text;
    bool inParentheses = false;


    for (int i = 0; i < s.Length; i++)
    {
      if (s[i] == '<' || s[i] == '>')
      {
        s = s.Substring(0, i) + s.Substring(i + 1);
        i--;
      } else {
        if (inParentheses)
        {
          //delete letters unless the end
          if (s[i] == ')')
          {
            inParentheses = false;
          }
          s = s.Substring(0, i) + s.Substring(i + 1);
          i--;
        } else {
          if (s[i] == '(')
          {
            inParentheses = true;
            s = s.Substring(0, i) + s.Substring(i + 1);
            i--;
          }
        }
      }
    }

    inputText.text = s;
  }

  int LetterToIndex(char c)
  {
    return (int)c - 97;
  }

  public void ToggleSuper()
  {
    super = !super;
  }

  public void ToggleIgnore()
  {
    ignore = !ignore;
  }
}
