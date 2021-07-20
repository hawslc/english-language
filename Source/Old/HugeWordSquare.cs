using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class HugeWordSquare : MonoBehaviour
{
  public TextMeshProUGUI[] texts;
  public TextMeshProUGUI countText;
  int size = 5;
  List<string> solutions = new List<string>();
  bool quality = false;
  int qualityCount = 0;
  List<int> isQuality = new List<int>();
  public Transform loadingBar;

  void Start()
  {
    IEnumerator c = Begin();
    StartCoroutine(c);
  }

  IEnumerator Begin()
  {
    yield return new WaitForSeconds(0.1f);
    List<string[]> w = Dictionary.GetDictionary(size);
    StartCoroutine(Generate(w));
  }


  public IEnumerator Generate(List<string[]> dict)
  {
    //print the dictionary
    for (int i = 0; i < dict.Count; i++)
    {
      string s = "";
      for (int k = 0; k < dict[i].Length; k++){
        s += "   " + dict[i][k];
        //print(dict[i][k]);
      }
      //print(s);
    }

    //now we find the squares
    int[] nums = new int[size * size];
    string[] grid = new string[size * 2];
    //the numbers change in meaning

    //every first letter
    for (nums[0] = 0; nums[0] < 26; nums[0]++)
    {
      ChangeLoadingBar(nums[0] / 26f);
      yield return null;
      //every word across the top with the first letter above
      for (nums[1] = 0; nums[1] < dict[nums[0]].Length; nums[1]++)
      {
        if (nums[1] == Mathf.Round(dict[nums[0]].Length / 2))
        {
          ChangeLoadingBar((nums[0] + 0.5f) / 26f);
          yield return null;
        }
        grid[0] = dict[nums[0]][nums[1]];

        //every word down from the top with the first letter above
        for (nums[2] = nums[1] + 1; nums[2] < dict[nums[0]].Length; nums[2]++)
        {
          grid[size] = dict[nums[0]][nums[2]];
          int topLetter1 = LetterToIndex(grid[size][1]);

          //every horizontal word on the second row
          for (nums[3] = 0; nums[3] < dict[topLetter1].Length; nums[3]++)
          {
            grid[1] = dict[topLetter1][nums[3]];
            List<string> wordsThatWork1 = FindWords(grid[0][1].ToString() + grid[1][1].ToString(), dict);

            //every vertical word on the second column
            for (nums[4] = 0; nums[4] < wordsThatWork1.Count; nums[4]++)
            {
              //now we are at this
              //  X X X X X
              //  X X X X X
              //  X X
              //  X X
              //  X X

              grid[size + 1] = wordsThatWork1[nums[4]];
              List<string> wordsThatWork2 = FindWords(grid[size][2].ToString() + grid[size + 1][2].ToString(), dict);

              //every horizontal word on the third column
              for (nums[5] = 0; nums[5] < wordsThatWork2.Count; nums[5]++)
              {
                grid[2] = wordsThatWork2[nums[5]];
                List<string> wordsThatWork3 = FindWords(grid[0][2].ToString() + grid[1][2].ToString() + grid[2][2].ToString(), dict);

                //every vertical word on the third column
                for (nums[6] = 0; nums[6] < wordsThatWork3.Count; nums[6]++)
                {
                  //now we are at this
                  //  X X X X X
                  //  X X X X X
                  //  X X X X X
                  //  X X X
                  //  X X X

                  grid[size + 2] = wordsThatWork3[nums[6]];
                  List<string> wordsThatWork4 = FindWords(grid[size][3].ToString() + grid[size + 1][3].ToString() + grid[size + 2][3].ToString(), dict);

                  //every horizontal word on the fourth column
                  for (nums[7] = 0; nums[7] < wordsThatWork4.Count; nums[7]++)
                  {
                    grid[3] = wordsThatWork4[nums[7]];
                    List<string> wordsThatWork5 = FindWords(grid[0][3].ToString() + grid[1][3].ToString() + grid[2][3].ToString() + grid[3][3].ToString(), dict);
                    //List<string> wordsThatWork5 = FindWords(FindSearchString(false, 3, grid), dict);

                    //every vertical word on the fourth column
                    for (nums[8] = 0; nums[8] < wordsThatWork5.Count; nums[8]++)
                    {
                      //now we are at this
                      //  X X X X X
                      //  X X X X X
                      //  X X X X X
                      //  X X X X X
                      //  X X X X

                      grid[size + 3] = wordsThatWork5[nums[8]];
                      List<string> wordsThatWork6 = FindWords(grid[size][4].ToString() + grid[size + 1][4].ToString() + grid[size + 2][4].ToString() + grid[size + 3][4].ToString(), dict);
                      //List<string> wordsThatWork6 = FindWords(FindSearchString(false, 4, grid), dict);

                      //every horizontal word on the fifth column
                      for (nums[9] = 0; nums[9] < wordsThatWork6.Count; nums[9]++)
                      {
                        grid[4] = wordsThatWork6[nums[9]];
                        //check if remaining part is a word
                        if (new List<string>(dict[LetterToIndex(grid[0][4])]).IndexOf(grid[0][4].ToString() + grid[1][4].ToString() + grid[2][4].ToString() + grid[3][4].ToString() + grid[4][4].ToString()) != -1)
                        {
                          grid[size + 3] = grid[0][4].ToString() + grid[1][4].ToString() + grid[2][4].ToString() + grid[3][4].ToString() + grid[4][4].ToString();
                          //we have a match
                          //print(" a solution is : \n" + grid[0] + "\n" + grid[1] + "\n" + grid[2] + "\n" + grid[3]);
                          solutions.Add(grid[0] + grid[1] + grid[2] + grid[3] + grid[4]);

                          //check if it is quality
                          List<char> w = new List<char>();
                          w.AddRange(solutions[solutions.Count() - 1]);
                          bool qual = w.Distinct().Count() >= 13;

                          if (grid[0][1] == grid[1][0]) qual = false;
                          if (grid[2][3] == grid[3][2]) qual = false;
                          if (grid[2] == grid[size + 2]) qual = false;
                          if (grid[3] == grid[size + 3]) qual = false;
                          if (grid[4] == grid[size + 4]) qual = false;

                          if (qual) qualityCount++;
                          isQuality.Add(qual ? qualityCount : 0);
                        }
                      }
                    }
                  }
                }
              }
            }
          }
        }
      }
    }

    Destroy(loadingBar.parent.gameObject);
    solutions = solutions.Distinct().ToList();
    DisplayRandomSolution();
  }

  List<string> FindWords(string s, List<string[]> dict)
  {
    /*
    List<string> words = new List<string>();
    int k = LetterToIndex(s[0]);

    for (int i = 0; i < dict[k].Length; i++)
    {
      if (s.Length > 1)
      {
        //if longer match
        if (dict[k][i][1] == s[1])
        {
          //if longer match, check it. otherwise, don't
          if (s.Length > 2)
          {
            if (dict[k][i][2] == s[2])
            {
              if (s.Length > 3)
              {
                if (dict[k][i][3] == s[3])
                {
                  words.Add(dict[k][i]);
                }
              } else {
                words.Add(dict[k][i]);
              }
            }
          } else {
            words.Add(dict[k][i]);
          }
        }
      } else {
        words.Add(dict[k][i]);
      }
    }

    return words;
    */
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

  public string FindSearchString(bool isVertical, int depth, string[] grid)
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

  int LetterToIndex(char c)
  {
    return (int)c - 97;
  }

  public void DisplayRandomSolution()
  {
    if (solutions.Count == 0)
    {
      for (int i = 0; i < size * size; i++)
      {
        texts[i].text = "";
      }
      texts[7].text = "No solutions found";
      return;
    }

    int r = Random.Range(0, solutions.Count);

    if (quality)
    {
      int j = 0;
      while (isQuality[r] == 0 && j < 10000)
      {
        r = Random.Range(0, solutions.Count);
        j++;
      }

      if (j >= 9999)
      {
        for (int i = 0; i < size * size; i++)
        {
          texts[i].text = "";
        }
        texts[4].text = "No solutions found";
        return;
      }
    }

    for (int i = 0; i < size * size; i++)
    {
      texts[i].text = solutions[r][i].ToString();
    }

    countText.text = r + "/" + solutions.Count;
    if (quality) countText.text = isQuality[r] + "/" + qualityCount;
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
}
