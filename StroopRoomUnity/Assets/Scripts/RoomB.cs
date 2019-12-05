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
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using DFTGames.Localization;

public class RoomB : MonoBehaviour
{
    
    #region Variables
    public int roomIdx;
    private int frameCounter = 0;

    private Text focus;                   //shows Task : WORD or COLOR
    private Text worcol;                 //shows colorword in color

    private Text phaseText;              //shown in practice, pause and end

    private Text pauseTimer;             //timer in pause
    private Text scoreRight;             //shows score text
    private Text scoreWrong;             //shows score text
    private Text numRight;               //shows score value
    private Text numWrong;               //shows score value

    private int right = 0;              //total score
    private int wrong = 0;              //total score

    private Image timebar;               //time
    private float time;
    private float startTime = 5.2f;

    private Text timer;                  //time
    private float timeFactor = 0.2f;     //time, fillFactor for timebar
    private GameObject hurtPanel;
    private GameObject[] selectables;     //basics
    private GameObject target;          //

    public GameObject followCameraCanvas;

    private bool showScore = false;           //
    private bool stop = false;               //
    private bool end = false;                //

    public StroopTask stroopTaskManager;    

    #endregion
        

    private int rightDifficulty;
    private int wrongDifficulty;
    private int numStimNextDif;     //set in start = 2;     //numStiumuli/5 //5 differnt levels of difficulty
    private int countStimuli;       //count++ in right, reset in IncreaseDifficulty



    //RoomA Variables
    private float minTime;          //set in start = 3.0f
    private float lessTime;         //set in start = 0.5f   
    

    //RoomB Variables
   // private float startScale;
    private float scaleWalls;
   // private float ceilingHeight;
    private float minScale;
  //  private float minHeight;
    private float scaleFactorWalls;
   // private float ceilingHeightFactor;
    private GameObject room;
    private GameObject colorWalls;
   // private GameObject ceiling;

    //VR
    public GameObject head;
    public GameObject RightController;
    private GameObject controllerHelper;
    private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
    private Valve.VR.EVRButtonId touchButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad;
    private SteamVR_TrackedObject trackedObject;
    private SteamVR_Controller.Device device;

    void OnEnable()
    {
        followCameraCanvas.SetActive(true);
        controllerHelper = GameObject.Find("ControllerHelper");
        controllerHelper.SetActive(false);
        focus = GameObject.Find("focus").GetComponent<Text>();
        worcol = GameObject.Find("worcol").GetComponent<Text>();
        phaseText = GameObject.Find("PhaseText").GetComponent<Text>();
        selectables = new GameObject[6];
        selectables[0] = GameObject.Find("Blue");
        selectables[1] = GameObject.Find("Red");
        selectables[2] = GameObject.Find("Green");
        selectables[3] = GameObject.Find("Yellow");
        selectables[4] = GameObject.Find("Orange");
        selectables[5] = GameObject.Find("Purple");
        pauseTimer = GameObject.Find("PauseTimer").GetComponent<Text>();
        numRight = GameObject.Find("numRight").GetComponent<Text>();
        numWrong = GameObject.Find("numWrong").GetComponent<Text>();

        pauseTimer.gameObject.SetActive(false);

        target = selectables[0];

        //subjectID = SaveInfo.GetID();
        roomIdx = SaveInfo.GetRoom();

        Localize.SetCurrentLanguage(SaveInfo.GetLanguage());

        stroopTaskManager = new StroopTask(SaveInfo.GetLanguage(), SaveInfo.GetID());


        time = startTime;
        startTime = 5.2f;
        timeFactor = 0.2f;

        hurtPanel = GameObject.Find("Hurt");
        hurtPanel.SetActive(false);

        timer = GameObject.Find("timer").GetComponent<Text>();
        scoreWrong = GameObject.Find("ScoreWrong").GetComponent<Text>();
        scoreRight = GameObject.Find("ScoreRight").GetComponent<Text>();
        timebar = GameObject.Find("Timebar").GetComponent<Image>();

        //RoomA Variables
        minTime = 3.0f;
        lessTime = 0.5f;
        countStimuli = 0;


        //RoomB Variables
        room = GameObject.Find("Walls");
        colorWalls = GameObject.Find("Selectables");
      //  ceiling = GameObject.Find("Ceiling");



        scaleWalls = 1.0f;
      //  ceilingHeight = ceiling.transform.position.y;
        minScale = 0.2f;
      //  minHeight = ceilingHeight * 0.5f;
        scaleFactorWalls = (scaleWalls - minScale) / 4.0f;
        // ceilingHeightFactor = (ceilingHeight - minHeight) / 4.0f;
        //  ceilingHeightFactor = 0.0f;


        //   Debug.Log("OnEnable: ceiligHeight: " + ceilingHeight + ", ceilingHeightFactor: " + ceilingHeightFactor);

        head = GameObject.Find("Camera (eye)");
        trackedObject = RightController.GetComponent<SteamVR_TrackedObject>();

        //Random.InitState(subjectID);

        Debug.Log("SubjectId: " + SaveInfo.GetID() + "RoomB");
    }

    void Update()
    {
        if (!end)
        {
            UpdateScore();
        }

        if (!stop)
        {
            SaveInfo.roomStatus.SetCondition(SaveInfo.StroopRoomCondition.B);

            StroopTimer();

            //VR Input
            device = SteamVR_Controller.Input((int)trackedObject.index);

            if (device == null)
            {
                return;
            }


            Ray raycast = new Ray(trackedObject.transform.position, trackedObject.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(raycast, out hit) && device.GetPressDown(triggerButton))
            {

                if (hit.collider.CompareTag("TARGET"))
                {
                    Right(hit.collider.gameObject.name);                  
                }
                else
                {
                    Wrong(hit.collider.gameObject.name);
                }
                countStimuli++;
                IncreaseDifficultyRoomSize();
            }
        }
        else
        {
            SaveInfo.roomStatus.SetCondition();

            if (end)
            {
                return;
            }
            else
            {
                //Pause
                SaveInfo.roomStatus.pauseTimeLeft -= Time.deltaTime;
                string minutes = Mathf.Floor(SaveInfo.roomStatus.pauseTimeLeft / 60).ToString("00");
                string seconds = Mathf.Floor(SaveInfo.roomStatus.pauseTimeLeft % 60).ToString("00");
                pauseTimer.text = minutes + ":" + seconds;
                Debug.Log("Pause: " + minutes + ":" + seconds);
                if (SaveInfo.roomStatus.pauseTimeLeft < 0)
                {
                    pauseTimer.gameObject.SetActive(false);
                   // Debug.Log("end pause");
                    //stroopItemIndex = 999;      //trigger next phase
                    //stroopTaskManager.stroopItemIndex = 999;
                    SaveInfo.roomStatus.TransitionPhase = true;
                    stop = false;
                }

            }
        }
    }

    #region Methods
    
    //Initializes Phase
    void Init()
    {
        ShowScore(showScore);
        stroopTaskManager.Init(SaveInfo.roomStatus.phase);

        countStimuli = 0;
        startTime = 5.2f;
        //difficulty = 0;
        scaleWalls = 1.0f;
     //   ceilingHeight = 5.0f;
        room.transform.localScale = new Vector3(scaleWalls, 1.0f, scaleWalls);
        colorWalls.transform.localScale = new Vector3(scaleWalls, 1.0f, scaleWalls);
    //    ceiling.transform.position = new Vector3(-0.158f, ceilingHeight, -1.0f);
        rightDifficulty = 0;
        wrongDifficulty = 0;
        Refresh();
    }


    //Sets GameObject as Target
    void SetTarget(StroopItem item)
    {
        if (stroopTaskManager.FocusTask == stroopTaskManager.task[0]) //WORD
        {
            for (int i = 0; i < selectables.Length; i++)
            {
                if (item.Name == stroopTaskManager.words[i])
                {
                    target = selectables[i];
                }
            }
        }
        else //COLOR
        {
            for (int i = 0; i < selectables.Length; i++)
            {
                if (item.ColorString == stroopTaskManager.words[i])
                {
                    target = selectables[i];
                }
            }
        }
        target.tag = "TARGET";
    }

    //Called after every Task, Resets Task
    void Refresh()
    {
        focus.text = stroopTaskManager.focusTasks[stroopTaskManager.stroopItemIndex];
        worcol.text = stroopTaskManager.stroopItems[stroopTaskManager.stroopItemIndex].Name;
        worcol.color = stroopTaskManager.stroopItems[stroopTaskManager.stroopItemIndex].Color;
        target.tag = "Selectables";
        SetTarget(stroopTaskManager.stroopItems[stroopTaskManager.stroopItemIndex]);
        time = startTime;
        timebar.fillAmount = 1f;
        hurtPanel.SetActive(false);
                
        //taskInfo = subjectID + "," + timestamp + "," + roomIdx + "," + phase + "," + taskEnglish(focusTasks[stroopItemIndex]) + ","
        //    + stroopItems[stroopItemIndex].NameEn + "," + stroopItems[stroopItemIndex].ColorEn + ",";
        //File.AppendAllText(path, taskInfo);
        Debug.Log("stroopItemIndex: " + stroopTaskManager.stroopItemIndex);
        stroopTaskManager.taskIsVisible();
    }


    //Called when correct selection
    void Right(string gameObjectName)
    {
        right++;
        //rightTemp++;
        rightDifficulty++;
        //float timetaken = startTime - time;
        //timetaken.ToString("f2");
        //File.AppendAllText(path, right + "," + wrong + ",");
        //File.AppendAllText(path, difficulty + "," + timetaken + "\n");

        stroopTaskManager.colorSelected(true, gameObjectName, SaveInfo.roomStatus.difficulty);
        if (stroopTaskManager.nextTask())
            Refresh();
        
       //  Debug.Log("Right");
    }

    //Called when incorrect selection or time is up
    void Wrong(string gameObjectName)
    {
        wrong++;
        //wrongTemp++;
        wrongDifficulty++;
        //float timetaken = startTime - time;
        //timetaken.ToString("f2");
        //File.AppendAllText(path, right + "," + wrong + ",");
        //File.AppendAllText(path, difficulty + "," + timetaken + "\n");
        hurtPanel.SetActive(true);

        stroopTaskManager.colorSelected(false, gameObjectName, SaveInfo.roomStatus.difficulty);
        if (stroopTaskManager.nextTask())
            Invoke("Refresh", 0.2f);
        
    }

    //timer
    void StroopTimer()
    {
        time -= Time.deltaTime;
        timer.text = time.ToString("f2");
        timebar.fillAmount = timeFactor * time;

        if (time <= 0.2f)
        {
            hurtPanel.SetActive(true);
        }

        if (time <= 0f)
        {
            //File.AppendAllText(path, "time,Wrong");
            Wrong(null);
            countStimuli++;
            IncreaseDifficultyRoomSize();
            time = startTime;
         //   Debug.Log("write");
         //   File.AppendAllText(path, focusTasks[stroopItemIndex] + " " + stroopItems[stroopItemIndex].ColorString + " TIME\n");
        }
    }

    //Updates score values
    void UpdateScore()
    {
        numRight.text = "" + stroopTaskManager.numCorrect;
        numWrong.text = "" + stroopTaskManager.numIncorrect;

    }

    //Show/hide score
    void ShowScore(bool score)
    {
        if (score)
        {
            scoreWrong.gameObject.SetActive(true);
            scoreRight.gameObject.SetActive(true);
            numRight.gameObject.SetActive(true);
            numWrong.gameObject.SetActive(true);
            UpdateScore();
        }
        else
        {
            scoreWrong.gameObject.SetActive(false);
            scoreRight.gameObject.SetActive(false);
            numRight.gameObject.SetActive(false);
            numWrong.gameObject.SetActive(false);
        }
    }

    //Pause setup
    void Pause()
    {
        stop = true;
        scaleWalls = 1.0f;
    //    ceilingHeight = 5.0f;
        room.transform.localScale = new Vector3(scaleWalls, 1.0f, scaleWalls);
        colorWalls.transform.localScale = new Vector3(scaleWalls, 1.0f, scaleWalls);
   //     ceiling.transform.position = new Vector3(-0.158f, ceilingHeight, -1.0f);

        pauseTimer.gameObject.SetActive(true);
        hurtPanel.SetActive(false);
        focus.transform.parent.gameObject.SetActive(false);
        timebar.gameObject.SetActive(false);
        //stroopItemIndex = 0;
        stroopTaskManager.stroopItemIndex = 0;
        SaveInfo.roomStatus.TransitionPhase = false;

        if (showScore)
        {
            scoreWrong.gameObject.SetActive(false);
            scoreRight.gameObject.SetActive(false);
            numRight.gameObject.SetActive(false);
            numWrong.gameObject.SetActive(false);
        }
    }

    //End setup
    void End()
    {
        stop = true;
        end = true;
        scaleWalls = 1.0f;
    //    ceilingHeight = 5.0f;
        room.transform.localScale = new Vector3(scaleWalls, 1.0f, scaleWalls);
        colorWalls.transform.localScale = new Vector3(scaleWalls, 1.0f, scaleWalls);
    //    ceiling.transform.position = new Vector3(-0.158f, ceilingHeight, -1.0f);
        hurtPanel.SetActive(false);
        focus.transform.parent.gameObject.SetActive(false);
        timebar.gameObject.SetActive(false);

        if (!showScore)
        {
            scoreWrong.gameObject.SetActive(true);
            scoreRight.gameObject.SetActive(true);
            numRight.gameObject.SetActive(true);
            numWrong.gameObject.SetActive(true);
        }
        numRight.text = "" + right;
        numWrong.text = "" + wrong;

      //  File.AppendAllText(path, "Total Right: " + right + "; Total Wrong: " + wrong);

    }

    //Debug StroopItem
    public void printStroopItem(StroopItem item)
    {
        Debug.Log("Name: " + item.Name + ", Color: " + item.ColorString);
    }


    private void SetPauseText()
    {
        if (SaveInfo.phaseSequence == SaveInfo.StroopRoomSequence.INCONGRUENT_CONGRUENT)
            phaseText.GetComponent<Localize>().localizationKey = "pauseIncongFirst";
        else
            phaseText.GetComponent<Localize>().localizationKey = "pause";
    }

    private void SetPhasePanelsActive()
    {
        hurtPanel.SetActive(true);
        focus.transform.parent.gameObject.SetActive(true);
        timebar.gameObject.SetActive(true);
    }

    //Listens to NextPhase.cs, Setups for all phases of test 
    public void SetPhase(int phaseIdx)
    {
        SaveInfo.roomStatus.SetPhase((SaveInfo.StroopRoomPhase)phaseIdx);
        switch (phaseIdx)
        {
            case (int)SaveInfo.StroopRoomPhase.PREPARATION:
                break;

            case (int)SaveInfo.StroopRoomPhase.CONGRUENT_PRACTICE:
                SetPhasePanelsActive();
                Init();           //practice, congruent
                phaseText.GetComponent<Localize>().localizationKey = "practice";
                Localize.SetCurrentLanguage(SaveInfo.GetLanguage());
                break;

            case (int)SaveInfo.StroopRoomPhase.CONGRUENT_TRIAL:
                Init();      //trial, congruent
                phaseText.text = "";
                break;

            case (int)SaveInfo.StroopRoomPhase.RESTING_BREAK:
                SetPauseText();
                Localize.SetCurrentLanguage(SaveInfo.GetLanguage());
                Pause();
                // phaseText.text = "Pause";
                break;

            case (int)SaveInfo.StroopRoomPhase.INCONGRUENT_PRACTICE:
                SetPhasePanelsActive();
                Init();          //practice, incongruent
                phaseText.GetComponent<Localize>().localizationKey = "practice";
                Localize.SetCurrentLanguage(SaveInfo.GetLanguage());
                break;

            case (int)SaveInfo.StroopRoomPhase.INCONGRUENT_TRIAL:
                Init();     //trial, incongruent
                phaseText.text = "";
                break;

            case (int)SaveInfo.StroopRoomPhase.RESTING_RECOVERY:
                phaseText.GetComponent<Localize>().localizationKey = "end";
                Localize.SetCurrentLanguage(SaveInfo.GetLanguage());
                End();
                break;

            default:
                End();
                phaseText.text = "Default";
                break;
        }
    }

    #endregion    

    //RoomA: 5s   4,5s    4s     3,5s    3s    END  time
    //RoomB: 1    0,875   0,75   0,625   0.5   END  y scale -0,125
    //RoomB: 1    0,8125  0,625  0,4375  0.25  END  x,z scale



    void IncreaseDifficultyRoomSize()        
    {
        numStimNextDif = stroopTaskManager.numStimuli / 5;    //5 differnt levels of difficulty

        if (!SaveInfo.roomStatus.IsPracticePhase())
        {
            if (countStimuli == numStimNextDif && scaleWalls > minScale)
            {
                scaleWalls -= scaleFactorWalls;
                colorWalls.transform.localScale = new Vector3(scaleWalls, 1.0f, scaleWalls);
                countStimuli = 0;
                SaveInfo.roomStatus.difficulty++;
                rightDifficulty = 0;
                wrongDifficulty = 0;
            }
        }
    }
}