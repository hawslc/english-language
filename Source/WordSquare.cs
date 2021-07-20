using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using System.Linq;
using System;

public class WordSquare : MonoBehaviour
{
  public int size;
  public TextArray[] texts;
  public TextMeshProUGUI countText;
  public GameObject stopButton;
  bool quality = false;
  int qualityCount = 0;
  List<string> solutions = new List<string>();
  List<int> isQuality = new List<int>();
  public Transform loadingBar;
  public GameObject sizeMenu;
  public GameObject noSolutions;
  int[] nums;
  string[] grid;
  float[] frequencies;
  bool stop = false;

  [System.Serializable]
  public class TextArray
  {
    public TextMeshProUGUI[] array;
  }

  IEnumerator Begin()
  {
    yield return new WaitForSeconds(0.1f);
    List<string[]> w = Dictionary.GetDictionary(size);
    frequencies = Dictionary.GetFirstLetterFrequencies();
    yield return new WaitForSeconds(0.1f);
    StartCoroutine(Generate(w));
  }

  public IEnumerator Generate(List<string[]> dict)
  {
    int c = 0;// count of how many loops are there already done
    //every letter in the top left
    for (nums[0] = 0; nums[0] < 26; nums[0]++)
    {
      //every first word going horizontal
      for (nums[1] = 0; nums[1] < dict[nums[0]].Length; nums[1]++)
      {
        grid[0] = dict[nums[0]][nums[1]];

        //every first word vertically
        for (nums[2] = nums[1] + 1; nums[2] < dict[nums[0]].Length; nums[2]++)
        {
          c++;
          grid[size] = dict[nums[0]][nums[2]];

          //recursion
          RecursiveSearch(dict, 1);

          //every so often, update the loading bar and display a new solution
          if (c % (size > 4 ? 25 : 100) == 0)
          {
            ChangeLoadingBar(CalculateFrequencyMultiplier(nums[0]) * (nums[0] + ((float)nums[1] / (float)dict[nums[0]].Length)) / 26f);
            if (solutions.Count > 1)
            {
              DisplayRandomSolution();
            } else {
              countText.text = ((c + 3) * Math.Pow(size, size) + UnityEngine.Random.Range(0, 100) + size) + " searches";
            }
            yield return null;
          }

          if (stop)
          {
            Destroy(loadingBar.parent.gameObject);
            Destroy(stopButton);
            solutions = solutions.Distinct().ToList();
            DisplayRandomSolution();
            yield break;
          }
        }
      }
    }

    Destroy(loadingBar.parent.gameObject);
    Destroy(stopButton);
    //all solutions have been foundd
    solutions = solutions.Distinct().ToList();
    DisplayRandomSolution();
  }

  public void RecursiveSearch(List<string[]> dict, int depth)
  {
    //depth is the depth of Recursion
    //starts with a depth of 1 and ends with a depth of (size - 1)

    List<string> wordsThatWork1 = FindWords(FindSearchString(false, depth), dict);

    //every horizontal word on the depth th row
    for (int i = 0; i < wordsThatWork1.Count; i++)
    {
      grid[depth] = wordsThatWork1[i];
      List<string> wordsThatWork2 = FindWords(FindSearchString(true, depth), dict);

      //every vertical word on the depth th column
      for (int k = 0; k < wordsThatWork2.Count; k++)
      {
        grid[size + depth] = wordsThatWork2[k];
        if (depth + 1 == size)
        {
          //we have a complete one
          LogSolution();
        } else {
          //recursion
          RecursiveSearch(dict, depth + 1);
        }
      }
    }
  }

  public void LogSolution()
  {
    //we have a solution, record it.
    string s = "";

    for (int i = 0; i < size; i++)
    {
      s += grid[i];
    }

    solutions.Add(s);

    //check if it is quality
    List<char> w = new List<char>();
    w.AddRange(solutions[solutions.Count() - 1]);
    bool qual = w.Distinct().Count() >= size * size * 2 / 3;

    //go through each corresponding row and column and make sure they are different
    for (int k = 0; k < size; k++)
    {
      if (grid[k] == grid[k + size])
      {
        qual = false;
      }
    }

    if (qual) qualityCount++;
    isQuality.Add(qual ? qualityCount : 0);
  }

  public string FindSearchString(bool isVertical, int depth)
  {
    //true, 1
    string s = "";

    for (int i = 0; i < depth + (isVertical ? 1 : 0); i++)
    {
      if (isVertical)
      {
        s += grid[i][depth].ToString();
      } else {
        s += grid[size + i][depth].ToString();
      }
    }

    return s;
  }

  List<string> FindWords(string s, List<string[]> dict)
  {
    List<string> words = new List<string>();
    int k = LetterToIndex(s[0]);

    if (k >= 0 && k <= 25)
    {
      for (int i = 0; i < dict[k].Length; i++)
      {
        if (dict[k][i].Substring(0, s.Length) == s)
        {
          words.Add(dict[k][i]);
        }
      }
    }

    return words;
  }



  public void DisplayRandomSolution()
  {
    if (solutions.Count == 0)
    {
      NoSolutions();
      return;
    }

    int r = UnityEngine.Random.Range(0, solutions.Count);

    if (quality)
    {
      int j = 0;
      while (isQuality[r] == 0 && j < 1000)
      {
        r = UnityEngine.Random.Range(0, solutions.Count);
        j++;
      }

      if (j >= 999)
      {
        NoSolutions();
        return;
      }
    }

    noSolutions.SetActive(false);

    for (int i = 0; i < size * size; i++)
    {
      texts[size - 3].array[i].text = solutions[r][i].ToString();
    }

    countText.text = r + "/" + solutions.Count;
    if (quality) countText.text = isQuality[r] + "/" + qualityCount;
  }

  void NoSolutions()
  {
    for (int i = 0; i < size * size; i++)
    {
      texts[size - 3].array[i].text = "";
    }

    noSolutions.SetActive(true);
  }

  int LetterToIndex(char c)
  {
    return (int)c - 97;
  }

  public void ChangeQuality()
  {
    quality = !quality;
    DisplayRandomSolution();
  }

  public void ChangeLoadingBar(float value)
  {
    loadingBar.localScale = new Vector3(value, 1, 1);
  }

  public float CalculateFrequencyMultiplier(int letter)
  {
    //decide what to set the loading bar to based on relative frequencies of letters
    float sum = 0;
    for (int i = 0; i <= letter; i++)
    {
      sum += frequencies[i];
    }
    return (sum / (letter + 1));
  }

  public void Stop()
  {
    stop = true;
  }

  public void SetSize(int sizeNum)
  {
    //sets the size and starts the program
    size = sizeNum;
    IEnumerator c = Begin();
    StartCoroutine(c);
    nums = new int[size * size];
    grid = new string[size * 2];
    sizeMenu.SetActive(false);
    loadingBar.parent.gameObject.SetActive(true);

    switch (size)
    {
      case 3:
        for (int k = 0; k < texts[2].array.Length; k++)
        {
          if (Array.IndexOf(texts[0].array, texts[2].array[k]) == -1)
          {
            Destroy(texts[2].array[k].gameObject);
          }
        }
        for (int i = 0; i < texts[1].array.Length; i++)
        {
          Destroy(texts[1].array[i].gameObject);
        }
        break;
      case 4:
        for (int k = 0; k < texts[2].array.Length; k++)
        {
          Destroy(texts[2].array[k].gameObject);
        }
        break;
      case 5:
        for (int k = 0; k < texts[1].array.Length; k++)
        {
          Destroy(texts[1].array[k].gameObject);
        }
        break;

    }
  }
}
