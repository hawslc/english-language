using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.IO;

public class NameMaker : MonoBehaviour
{
  public TextMeshProUGUI text;
  List<string> words = new List<string>();
  string vowels = "aeiou";
  string alphabet = Dictionary.GetAlphabet();
  List<string>[] doubleVowel = new List<string>[3]; //doubleVowel[0] is the first two letters, [1] is any middle letter, [2] is the last two
  List<string>[] doubleCon = new List<string>[3];
  List<string>[] vowelCon = new List<string>[3];
  List<string>[] conVowel = new List<string>[3];
  List<string> badWords = new List<string>();

  void Start()
  {
    words = Dictionary.GetWords();

    //setup
    int location = 0; // the start is 0, middle is 1, end is 2
    //setup arrays
    for (int i = 0; i < 3; i++)
    {
      doubleVowel[i] = new List<string>();
      doubleCon[i] = new List<string>();
      vowelCon[i] = new List<string>();
      conVowel[i] = new List<string>();
    }

    //each word
    for (int i = 0; i < words.Count; i++)
    {
      //each two letter gap
      for (int k = 0; k < words[i].Length - 1; k++)
      {
        location = (k == 0) ? 0 : ((k + 2 == words[i].Length) ? 2 : 1); //gotta love ternary operators
        //if non-alphabetical characters, don't add
        if (alphabet.IndexOf(words[i][k]) == -1 || alphabet.IndexOf(words[i][k + 1]) == -1) continue;
        //add two letters to the right spot
        if (IsVowel(words[i][k]) == false)
        {
          //the first letter is a consenant
          if (IsVowel(words[i][k + 1]) == false)
          {
            doubleCon[location].Add(words[i][k].ToString() + words[i][k + 1].ToString());
          } else {
            conVowel[location].Add(words[i][k].ToString() + words[i][k + 1].ToString());
          }
        } else {
          //first letter is a vowel
          if (IsVowel(words[i][k + 1]) == false)
          {
            vowelCon[location].Add(words[i][k].ToString() + words[i][k + 1].ToString());
          } else {
            doubleVowel[location].Add(words[i][k].ToString() + words[i][k + 1].ToString());
          }
        }
      }
    }

    int size = Dictionary.fileSize;
    Dictionary.fileSize = 6;
    badWords = Dictionary.GetWords();
    Dictionary.fileSize = size;

    Generate();
  }

  public void Generate()
  {
    //generate a random name
    string s = words[1];

    while(words.IndexOf(s) != -1 || ContainsBadWord(s))
    {
      s = "";
      int r = UnityEngine.Random.Range(0, 4);
      int len = UnityEngine.Random.Range(4, 10);


      if (r == 0)
      {
        //word starts with a vowel
        s += vowels[UnityEngine.Random.Range(0, vowels.Length)].ToString();

        for (int i = 0; i < len - 1; i++)
        {
          s = BuildNextLetter(s, i + 2 == len);
        }
      } else {
        //word starts with any letter (most likely a consenant but it could be a vowel)
        s += alphabet[UnityEngine.Random.Range(0, alphabet.Length)].ToString();

        for (int i = 0; i < len - 1; i++)
        {
          s = BuildNextLetter(s, i + 2 == len);
        }
      }

      if (ContainsBadWord(s)) print(s);
    }

    text.text = s;
  }

  public string BuildNextLetter(string s, bool end)
  {
    //generates the next letter in a word
    //assumes the first letter is already generated
    char last1 = s[s.Length - 1];
    char last2; //second last
    int location = (end) ? 2 : 1;

    if (s.Length == 1)
    {
      last2 = (1 == UnityEngine.Random.Range(0, 2)) ? 'a' : 'b';
      location = 0;
    } else {
      last2 = s[s.Length - 2];
    }


    //if last two letters were a vowel, the next letter is a consenant
    //and vice versa
    if (IsVowel(last1) && IsVowel(last2))
    {
      s += IterateLetter(vowelCon[location], last1);
    }
    else if (!IsVowel(last1) && !IsVowel(last2))
    {
      s += IterateLetter(conVowel[location], last1);
    }
    else
    {
      int r = UnityEngine.Random.Range(0, 2);

      if (IsVowel(last1))
      {
        if (r == 0)
        {
          s += IterateLetter(doubleVowel[location], last1);
        } else {
          s += IterateLetter(vowelCon[location], last1);
        }
      } else {
        if (r == 0)
        {
          s += IterateLetter(doubleCon[location], last1);
        } else {
          s += IterateLetter(conVowel[location], last1);
        }
      }
    }

    return s;
  }

  public string IterateLetter(List<string> search, char c)
  {
    //search is the list to search, c is the first letter
    //just finds a letter pair in the search list where c is the first letter
    //
    int r = UnityEngine.Random.Range(0, search.Count);

    int l = 0;

    while(search[r][0] != c && l < 5000)
    {
      r = UnityEngine.Random.Range(0, search.Count);
      l++;
    }

    return search[r][1].ToString();
  }

  public string Rot13(string s)
  {
    for (int i = 0; i < s.Length; i++)
    {
      if (alphabet.IndexOf(s[i]) != -1) s = s.Substring(0, i) + ((alphabet.IndexOf(s[i]) > 12) ? alphabet[alphabet.IndexOf(s[i]) - 13] : alphabet[alphabet.IndexOf(s[i]) + 13]) + s.Substring(i + 1);
    }

    return s;
  }

  public bool IsVowel(char c)
  {
    //returns if the character is a vowel or not
    return vowels.IndexOf(c) != -1;
  }

  public bool ContainsBadWord(string s)
  {
    string word = Rot13(s);

    for (int i = 0; i < s.Length; i++)
    {
      if (badWords[i].IndexOf(word) != -1) return true;
    }

    return false;
  }
}
