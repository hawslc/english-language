using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using System.Linq;

public class SmallWordSquare : MonoBehaviour
{
  public TextMeshProUGUI[] texts;
  public TextMeshProUGUI countText;
  int size = 3;
  bool quality = false;
  int qualityCount = 0;
  List<string> solutions = new List<string>();
  List<int> isQuality = new List<int>();
  public Transform loadingBar;

  void Start()
  {
    //records = file.text.Replace("\"", "").Split('\n');
    //words have an extra "" around them
    //so "tree" and "house" instead of tree
    //Generate3();
    IEnumerator c = Begin();
    StartCoroutine(c);
  }

  IEnumerator Begin()
  {
    yield return new WaitForSeconds(0.1f);
    List<string[]> w = Dictionary.GetDictionary(3);
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
      //print(s + " --- " + i);
    }



    //now we find the squares

    int[] nums = new int[size * size];
    string[] grid = new string[size * 2];

    //the numbers change in meaning


    for (nums[0] = 0; nums[0] < 26; nums[0]++)
    {
      ChangeLoadingBar(nums[0] / 26f);
      yield return null;
      //every first letter on the top left
      for (nums[1] = 0; nums[1] < dict[nums[0]].Length; nums[1]++)
      {
        grid[0] = dict[nums[0]][nums[1]];
        //every first word with that first letter

        for (nums[2] = nums[1] + 1; nums[2] < dict[nums[0]].Length; nums[2]++)
        {
          grid[size] = dict[nums[0]][nums[2]];
          //every word going down from the top letter with the same word along the top
          int topLetter = (int)grid[0][1] - 97;
          for (nums[3] = 0; nums[3] < dict[topLetter].Length; nums[3]++)
          {
            grid[size + 1] = dict[topLetter][nums[3]];
            //every word going down the middle vertically with the middle letter of the first word as the first letter of this word


            //this is the first spot at which the search can fail
            //the middle row going horizontally like this
            //  t a n
            //  o r
            //  w e
            List<string> wordsThatWork1 = new List<string>();
            int firstLetter = (int)grid[size][1] - 97;
            for (int p = 0; p < dict[firstLetter].Length; p++)
            {
              if (dict[firstLetter][p][1] == grid[size + 1][1] && dict[firstLetter][p][0] == grid[size][1])
              {
                wordsThatWork1.Add(dict[firstLetter][p]);
              }
            }
            //if we can continue, do so
            if (wordsThatWork1.Count > 0)
            {
              for (nums[5] = 0; nums[5] < wordsThatWork1.Count; nums[5]++)
              {
                grid[1] = wordsThatWork1[nums[5]];
                //we have filled in this so far
                //  s a t
                //  i r e
                //  t e
                //now fill in the rest if it is possible

                //now check for the one on the right side going down
                List<string> wordsThatWork2 = new List<string>();
                int firstLetter2 = (int)grid[0][2] - 97;
                for (int k = 0; k < dict[firstLetter2].Length; k++)
                {
                  if (dict[firstLetter2][k][1] == grid[1][2]){
                    wordsThatWork2.Add(dict[firstLetter2][k]);
                  }
                }

                //now the whole grid is filled in, just left to check if the bottom row is a word
                if (wordsThatWork2.Count > 0)
                {
                  for (nums[6] = 0; nums[6] < wordsThatWork2.Count; nums[6]++)
                  {
                    int firstLetter3 = (int)grid[size][2] - 97;
                    for (int j = 0; j < dict[firstLetter3].Length; j++)
                    {
                      if (dict[firstLetter3][j][1] == grid[size + 1][2]){
                        if (dict[firstLetter3][j][2] == wordsThatWork2[nums[6]][2]){
                          //we have a complete square
                          grid[2] = dict[firstLetter3][j];
                          grid[size + 2] = wordsThatWork2[nums[6]];

                          bool works = true;
                          for (nums[7] = 0; nums[7] < 3; nums[7]++){
                            for (nums[8] = 0; nums[8] < 3; nums[8]++){
                              if (grid[nums[7]][nums[8]] != grid[size + nums[8]][nums[7]]) works = false;
                            }
                          }

                          if (works == true)
                          {
                            //yay, solved
                            solutions.Add(grid[0] + "" + grid[1] + "" + grid[2]);

                            //check if it is quality
                            List<char> w = new List<char>();
                            w.AddRange(solutions[solutions.Count() - 1]);
                            bool qual = w.Distinct().Count() >= 8;

                            if (qual) qualityCount++;
                            isQuality.Add(qual ? qualityCount : 0);
                            //print("A solution is ----- \n " + grid[0] + " \n" + grid[1] + " \n" + grid[2] + " \n---- " + grid[size] + " " + grid[size + 1] + " " + grid[size + 2]);
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
    }

    Destroy(loadingBar.parent.gameObject);
    //all solutions have been foundd
    DisplayRandomSolution();
  }

  public void DisplayRandomSolution()
  {
    if (solutions.Count == 0)
    {
      for (int i = 0; i < 9; i++)
      {
        texts[i].text = "";
      }
      texts[4].text = "No solutions found";
      return;
    }

    int r = Random.Range(0, solutions.Count);

    if (quality)
    {
      int j = 0;
      while (isQuality[r] == 0 && j < 1000)
      {
        r = Random.Range(0, solutions.Count);
        j++;
      }

      if (j >= 999)
      {
        for (int i = 0; i < 9; i++)
        {
          texts[i].text = "";
        }
        texts[4].text = "No solutions found";
        return;
      }
    }


    for (int i = 0; i < 9; i++)
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
