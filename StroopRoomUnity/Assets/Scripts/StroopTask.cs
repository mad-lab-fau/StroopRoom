/* 
* This file is part of the Stroop Room distribution (https://github.com/mad-lab-fau/StroopRoom).
* Copyright (c) 2019 Machine Learning and Data Analytics Lab, Friedrich-Alexander-Universität Erlangen-Nürnberg (FAU).
* 
* This program is free software: you can redistribute it and/or modify  
* it under the terms of the GNU General Public License as published by  
* the Free Software Foundation, version 3.
*
* This program is distributed in the hope that it will be useful, but 
* WITHOUT ANY WARRANTY; without even the implied warranty of 
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU 
* General Public License for more details.
*
* You should have received a copy of the GNU General Public License 
* along with this program. If not, see <http://www.gnu.org/licenses/>.
* 
* 
* The Stroop Room has been published as
* 
* Stefan Gradl, Markus Wirth, Nico Mächtlinger, Romina Poguntke, Andrea Wonner, Nicolas Rohleder, and Bjoern M. Eskofier. 2019.
* The Stroop Room: A Virtual Reality-Enhanced Stroop Test.
* In 25th ACM Symposium on Virtual Reality Software and Technology (VRST '19), Tomas Trescak, Simeon Simoff, Deborah Richards,
* Anton Bogdanovych, Thierry Duval, Torsten Kuhlen, Huyen Nguyen, Shigeo Morishima, Yuichi Itoh, Richard Skarbez,
* Anton Bogdanovych, and Martin Masek (Eds.). ACM, New York, NY, USA, Article 28, 12 pages.
* DOI: https://doi.org/10.1145/3359996.3364247
* 
* Please cite this work if you use this for your own research.
*/
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class StroopTask : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

        
    SystemLanguage language = SaveInfo.GetLanguage();

    public List<StroopItem> stroopItems = new List<StroopItem>();
    public List<string> focusTasks = new List<string>();
    public string FocusTask { get { return focusTasks[stroopItemIndex]; } }

    public string[] words;
    public string[] task;
    public StroopItem[] combinations;
    //private string taskInfo = "";

    public int numStimuli;          //Number of stimuli
    public int stroopItemIndex = 0; //index of shown StroopItem 

    public int numCorrect = 0;
    public int numIncorrect = 0;

    private int subjectID;

    private float timeAtTaskStart = 0f;
    private string timestampAtTaskStart = null;

    public int LifetimeTaskIndex { get; private set; } = 0;

    public StroopTask(SystemLanguage lang, int subjectID)
    {       
        this.subjectID = subjectID;
        Random.InitState(subjectID);
        SetArrayLanguage(lang);
    }

    public void Init(SaveInfo.StroopRoomPhase phase)
    {
        // TODO: this can be beautified through an enum set.
        if (phase == SaveInfo.StroopRoomPhase.INCONGRUENT_PRACTICE || phase == SaveInfo.StroopRoomPhase.CONGRUENT_PRACTICE)
            SetNumStimuli(true);
        else
            SetNumStimuli(false);

        if (phase == SaveInfo.StroopRoomPhase.CONGRUENT_PRACTICE || phase == SaveInfo.StroopRoomPhase.CONGRUENT_TRIAL)
            SetCongruent(true);
        else
            SetCongruent(false);

        MakeTask();
        BuildList();
        stroopItemIndex = 0;
        numCorrect = 0;
        numIncorrect = 0;
        SaveInfo.roomStatus.TransitionPhase = false;
    }

    //Sets number of stimulis depending on phase (practice or trial)
    void SetNumStimuli(bool practice)
    {
        if (practice)
        {
            numStimuli = SaveInfo.NUM_PRACTICE_STIMULI;     //10,15
        }
        else
        {
            numStimuli = SaveInfo.NUM_TRIAL_STIMULI;     //80,100,120
        }
        if (SaveInfo.debugSystem == SaveInfo.StroopRoomDebug.DebugSuperfast)
            numStimuli = 5;
    }

    //Sets Language
    private void SetArrayLanguage(SystemLanguage lang)
    {
        if (lang.ToString() == "German")
        {
            words = StroopItem.colorWordsGer;
            task = new string[] { "WORT", "FARBE" };
        }
        else if (lang.ToString() == "English")
        {
            words = StroopItem.colorWordsEn;
            task = new string[] { "WORD", "COLOR" };
        }
        else if (lang.ToString() == "Dutch")
        {
            words = StroopItem.colorWordsNl;
            task = new string[] { "WOORD", "KLEUR" };
        }
    }

    //Creates congruent or incongruent StroopItem[]
    void SetCongruent(bool congruent)
    {
        //make congruent or incongruent combinations
        if (congruent)
        {
            //Debug.Log("congruent");
            combinations = new StroopItem[6];
            for (int i = 0; i < combinations.Length; i++)
            {
                combinations[i] = new StroopItem(words[i], words[i]);
                //printStroopItem(combinations[i]);
            }
        }
        else
        {
            //  Debug.Log("incongruent");
            combinations = new StroopItem[30];
            int position = 0;
            for (int j = 0; j < words.Length; j++)
            {
                for (int k = 0; k < words.Length; k++)
                {
                    if (j == words.Length - 1 && k == words.Length - 1)
                    {
                        return;
                    }
                    if (k == j)
                    {
                        k = j + 1;
                    }
                    combinations[position] = new StroopItem(words[j], words[k]);
                    //printStroopItem(combinations[position]);                          
                    position++;
                }
            }
        }
    }


    //Creates List of Tasks
    void MakeTask()
    {
        focusTasks.Clear();
        int taskIndex = 0;
        int wordCount = 0;
        int colorCount = 0;
        int colorRep = 0;
        int wordRep = 0;
        int repetition = 3;
        int maxCount = numStimuli / 2;
        int diff = 0;

        for (int i = 0; i < numStimuli; i++)
        {
            if (diff >= 3 && taskIndex == 0)
            {
                taskIndex = 1;
            }
            else if (diff >= 3 && taskIndex == 1)
            {
                taskIndex = 0;
            }
            else
            {
                taskIndex = (int)Random.Range(0, 2);
            }

            if (taskIndex == 0)
            {
                wordRep++;
                colorRep = 0;

                if (wordRep > repetition)
                {
                    taskIndex = 1;
                    colorRep++;
                    wordRep = 0;
                }
                else if (wordCount == maxCount)
                {
                    taskIndex = 1;
                    colorRep++;
                    wordRep = 0;
                }
            }
            else
            {
                colorRep++;
                wordRep = 0;
                if (colorRep > repetition)
                {
                    taskIndex = 0;
                    wordRep++;
                    colorRep = 0;
                }
                else if (colorCount == maxCount)
                {
                    taskIndex = 0;
                    wordRep++;
                    colorRep = 0;
                }
            }
            if (taskIndex == 0)
            {
                wordCount++;
            }
            else
            {
                colorCount++;
            }

            focusTasks.Add(task[taskIndex]);
            //   Debug.Log("focusTasks[i]: " + focusTasks[i]);
            diff = Mathf.Abs(wordCount - colorCount);
        }
        //  Debug.Log("focusTasks.length: " + focusTasks.Count);

        // File.AppendAllText(path, "Words: " + wordCount + " Colors: " + colorCount + "\n");
        //  Debug.Log("Words: " + wordCount + " Colors: " + colorCount);
    }


    //Creates List of StroopItems
    void BuildList()
    {
        int val;

        if (numStimuli % combinations.Length == 0)
        {
            val = numStimuli / combinations.Length;
        }
        else
        {
            val = (numStimuli / combinations.Length) + 1;
        }

        List<StroopItem> stroopBucket = new List<StroopItem>(numStimuli);

        for (int i = 0; i < val; i++)
        {
            for (int n = 0; n < combinations.Length; n++)
            {
                stroopBucket.Add(combinations[n]);
            }
        }
        /* for (int i = 0; i < stroopBucket.Count; i++)       
         {
             printStroopItem(stroopBucket[i]);
         }*/
        //  Debug.Log("stroopBucket.Count: " + stroopBucket.Count);

        stroopItems.Clear();

        for (int i = 0; i < numStimuli; i++)
        {
            int rndIdx = Random.Range(0, stroopBucket.Count - 1);
            StroopItem fetchedItemFromBucket = stroopBucket[rndIdx];
            stroopBucket.RemoveAt(rndIdx);
            stroopItems.Add(fetchedItemFromBucket);
        }
        /* for (int i = 0; i < numStimuli; i++)                
         {
             printStroopItem(stroopItems[i]);
         }*/
    }

    string taskEnglish(string focus)
    {
        string task = "";
        if (SaveInfo.GetLanguage().ToString() == "English")
        {
            task = focus;
        }
        else if (SaveInfo.GetLanguage().ToString() == "German")
        {
            if (focus == "WORT")
            {
                task = "WORD";
            }
            else
            {
                task = "COLOR";
            }
        }
        return task;
    }

    /// <summary>
    /// Called at the frame when the new task is visible to the user for the first time.
    /// </summary>
    public void taskIsVisible()
    {
        timeAtTaskStart = Time.time;
        timestampAtTaskStart = SaveInfo.strTimestamp();
    }

    /// <summary>
    /// Called at the frame when the user has selected a color.
    /// </summary>
    /// <param name="isRight"></param>
    /// <returns></returns>
    public bool colorSelected(bool isRight, string gameObjectName, int difficulty)
    {
        float timeDelta = Time.time - timeAtTaskStart;

        if (isRight)
            numCorrect++;
        else
            numIncorrect++;

        logPerformance(isRight, timeDelta, gameObjectName, difficulty);
        MovementTracking.TrackHeadAndController(LifetimeTaskIndex);
        return isRight;
    }

    /// <summary>
    /// Returns the csv header line for performance log entries (the column definitions).
    /// </summary>
    /// <returns></returns>
    public static string logHeader(int subjectID)
    {
        string cs = SaveInfo.csvColSeparator;
        return "SubjectID: " + subjectID + "\nDate: " + System.DateTime.Now.ToString("dd/MM/yyyy") + "\nTime: " + System.DateTime.Now.ToString("HH:mm:ss:fff") + "\nFileversion: 2\n\n" + "time" + cs + "stID" + cs + "room" + cs + "phase" + cs + "task" + cs + "word" + cs + "color" + cs + "selected" + cs + "isCorrect" + cs + "right" + cs + "wrong" + cs + "difficulty" + cs + "time taken\n";
    }

    /// <summary>
    /// Creates a log line entry into the performance log csv.
    /// </summary>
    /// <param name="isRight"></param>
    /// <param name="taskTime"></param>
    /// <param name="gameObjectName"></param>
    /// <param name="difficulty"></param>
    private void logPerformance(bool isRight, float taskTime, string gameObjectName, int difficulty)
    {
        string cs = SaveInfo.csvColSeparator;
        string logLine = timestampAtTaskStart + cs + LifetimeTaskIndex + cs + SaveInfo.roomStatus.condition + cs + SaveInfo.roomStatus.phase + cs + taskEnglish(focusTasks[stroopItemIndex]) + cs + stroopItems[stroopItemIndex].NameEn + cs + stroopItems[stroopItemIndex].ColorEn + cs + gameObjectName + cs + isRight + cs + numCorrect + cs + numIncorrect + cs + difficulty + cs + taskTime.ToString("f2") + "\n";
        File.AppendAllText(SaveInfo.performancePath, logLine);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>true if we continue running and a refresh needs to be triggered.</returns>
    public bool nextTask()
    {
        LifetimeTaskIndex++;
        if (stroopItemIndex == numStimuli - 1)
        {
            //stroopItemIndex = 999;              //trigger nextphase
            SaveInfo.roomStatus.TransitionPhase = true;
            return false;
        }
        else
        {
            stroopItemIndex++;
            //OnNextTask(EventArgs.Empty);
        }
        return true;
    }

    // TODO: Create that kind of handler or an override to externalize and automate difficulty increases in rooms.
    /*public event EventHandler NextTaskEvents;

    protected virtual void OnNextTask(EventArgs e)
    {
        EventHandler handler = NextTaskEvents;
        if (handler != null)
        {
            handler(this, e);
        }
    }*/
}
