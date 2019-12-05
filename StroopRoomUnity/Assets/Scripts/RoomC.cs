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

public class RoomC : MonoBehaviour
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
    public GameObject[] selectables;     //basics
    public GameObject[] walls;     //basics
    private GameObject target;          //

    public GameObject followCameraCanvas;

    private bool showScore = false;           //
    private bool stop = false;               //
    private bool end = false;                //

    public StroopTask stroopTaskManager; 

    #endregion

    //VR
    public GameObject head;
    public GameObject RightController;
    private GameObject controllerHelper;
    private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
    private Valve.VR.EVRButtonId touchButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad;
    private SteamVR_TrackedObject trackedObject;
    private SteamVR_Controller.Device device;

    private float minTime;          //set in start = 3.0f
    private float lessTime;         //set in start = 0.5f   
    private int numStimNextDif;     //set in start = 2;     //numStiumuli/5 //5 differnt levels of difficulty
    private int countStimuli;       //count++ in right, reset in IncreaseDifficulty
    private int rightDifficulty;
    private int wrongDifficulty;

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
        walls = new GameObject[6];
        walls[0] = GameObject.Find("BlueWall");
        walls[1] = GameObject.Find("RedWall");
        walls[2] = GameObject.Find("GreenWall");
        walls[3] = GameObject.Find("YellowWall");
        walls[4] = GameObject.Find("OrangeWall");
        walls[5] = GameObject.Find("PurpleWall");
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

        head = GameObject.Find("Camera (eye)");
        trackedObject = RightController.GetComponent<SteamVR_TrackedObject>();

        //Random.InitState(subjectID);
        
        Debug.Log("SubjectId: " + SaveInfo.GetID() + "RoomC");
    }

    void Update()
    {
        if (!end)
        {
            UpdateScore();
        }

        if (!stop)
        {
            SaveInfo.roomStatus.SetCondition(SaveInfo.StroopRoomCondition.C);

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
                IncreaseDifficultySearchTask();
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
        selecScaleX = 10.0f;
        selecScaleY = 3.5f;
        for (int z = 0; z < selectables.Length; z++)
        {
            selectables[z].transform.localScale = new Vector3(selecScaleX, selecScaleY, 0.2f);
        }
        selectables[5].transform.position = new Vector3(0.0f, 1.75f, 8.66f);    //purple
        selectables[0].transform.position = new Vector3(0.0f, 1.75f, -8.66f);    //blue
        selectables[2].transform.position = new Vector3(-7.5f, 1.75f, 4.33f);    //green
        selectables[4].transform.position = new Vector3(7.5f, 1.75f, -4.33f);    //orange
        selectables[3].transform.position = new Vector3(7.5f, 1.75f, 4.33f);    //yellow
        selectables[1].transform.position = new Vector3(-7.5f, 1.75f, -4.33f);    //red
        increaseCounter = 0;
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
    int countScale = 0;
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
        
        //taskInfo = subjectID  + "," + timestamp + "," + roomIdx + "," + phase + "," + taskEnglish(focusTasks[stroopItemIndex]) + ","
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
            //File.AppendAllText(path, "time,Wrong,");
            Wrong(null);
            countStimuli++;
            IncreaseDifficultySearchTask();
            time = startTime;
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
        pauseTimer.gameObject.SetActive(true);
        hurtPanel.SetActive(false);
        focus.transform.parent.gameObject.SetActive(false);
        timebar.gameObject.SetActive(false);
        //stroopItemIndex = 0;
        stroopTaskManager.stroopItemIndex = 0;
        SaveInfo.roomStatus.TransitionPhase = false;

        selecScaleX = 10.0f;
        selecScaleY = 3.5f;
        for (int z = 0; z < selectables.Length; z++)
        {
            selectables[z].transform.localScale = new Vector3(selecScaleX, selecScaleY, 0.2f);
        }
        selectables[5].transform.position = new Vector3(0.0f, 1.75f, 8.66f);    //purple
        selectables[0].transform.position = new Vector3(0.0f, 1.75f, -8.66f);    //blue
        selectables[2].transform.position = new Vector3(-7.5f, 1.75f, 4.33f);    //green
        selectables[4].transform.position = new Vector3(7.5f, 1.75f, -4.33f);    //orange
        selectables[3].transform.position = new Vector3(7.5f, 1.75f, 4.33f);    //yellow
        selectables[1].transform.position = new Vector3(-7.5f, 1.75f, -4.33f);    //red

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
        hurtPanel.SetActive(false);
        focus.transform.parent.gameObject.SetActive(false);
        timebar.gameObject.SetActive(false);

        selecScaleX = 10.0f;
        selecScaleY = 3.5f;
        for (int z = 0; z < selectables.Length; z++)
        {
            selectables[z].transform.localScale = new Vector3(selecScaleX, selecScaleY, 0.2f);
        }
        selectables[5].transform.position = new Vector3(0.0f, 1.75f, 8.66f);    //purple
        selectables[0].transform.position = new Vector3(0.0f, 1.75f, -8.66f);    //blue
        selectables[2].transform.position = new Vector3(-7.5f, 1.75f, 4.33f);    //green
        selectables[4].transform.position = new Vector3(7.5f, 1.75f, -4.33f);    //orange
        selectables[3].transform.position = new Vector3(7.5f, 1.75f, 4.33f);    //yellow
        selectables[1].transform.position = new Vector3(-7.5f, 1.75f, -4.33f);    //red

        if (!showScore)
        {
            scoreWrong.gameObject.SetActive(true);
            scoreRight.gameObject.SetActive(true);
            numRight.gameObject.SetActive(true);
            numWrong.gameObject.SetActive(true);
        }
        numRight.text = "" + right;
        numWrong.text = "" + wrong;

     //   File.AppendAllText(path, "Total Right: " + right + "; Total Wrong: " + wrong);

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

    float minScaleX = 1.5f;
    float selecScaleX = 10.0f;
    float selecScaleY = 3.5f;
    int increaseCounter = 0;



    void IncreaseDifficultySearchTask()        //5s   4,5s   4s   3,5s    3s    END
    {
        numStimNextDif = stroopTaskManager.numStimuli / 5;    //5 differnt levels of difficulty
        if (!SaveInfo.roomStatus.IsPracticePhase())
        {
            //make selectables smaller
            if (countStimuli == numStimNextDif && selecScaleX > minScaleX)
            {
                for (int z = 0; z < selectables.Length; z++)
                {
                    selectables[z].transform.localScale -= new Vector3(2.125f, 0.6875f, 0f);
                }
                countStimuli = 0;
                increaseCounter++;
                SaveInfo.roomStatus.difficulty++;
                //  File.AppendAllText(path, "Right: " + rightDifficulty + "; Wrong: " + wrongDifficulty + "\n");
                //   File.AppendAllText(path, "-Difficulty increased:-\n");
                rightDifficulty = 0;
                wrongDifficulty = 0;
            }

            //switch color
            if (increaseCounter < 4)
            {
                ChangePosition();
            }
            else //jump around
            {
                if (stroopTaskManager.stroopItemIndex % 2 == 0)
                {
                    NewCoords();
                }
                else
                {
                    NewCoords();
                    ChangePosition();
                }
            }
        }
    }



    //switches opposing selectables, called by Refresh()
    void ChangePosition()
    {

      /*  selectables[0].transform.position = new Vector3(selectables[0].transform.position.x, selectables[0].transform.position.y, -1 * selectables[0].transform.position.z);    //blue
        selectables[5].transform.position = new Vector3(selectables[5].transform.position.x, selectables[5].transform.position.y, -1 * selectables[5].transform.position.z);    //purple
        selectables[2].transform.position = new Vector3(-1 * selectables[2].transform.position.x, selectables[2].transform.position.y, -1 * selectables[2].transform.position.z);    //green
        selectables[4].transform.position = new Vector3(-1 * selectables[4].transform.position.x, selectables[4].transform.position.y, -1 * selectables[4].transform.position.z);    //orange
        selectables[3].transform.position = new Vector3(-1 * selectables[3].transform.position.x, selectables[3].transform.position.y, -1 * selectables[3].transform.position.z);    //yellow
        selectables[1].transform.position = new Vector3(-1 * selectables[1].transform.position.x, selectables[1].transform.position.y, -1 * selectables[1].transform.position.z);    //red
        */
        

        //purple and blue 
        Vector3 tempPositionOne = selectables[5].transform.position;
            selectables[5].transform.position = selectables[0].transform.position;
            selectables[0].transform.position = tempPositionOne;

            //green and orange
            Vector3 tempPositionTwo = selectables[2].transform.position;
            selectables[2].transform.position = selectables[4].transform.position;
            selectables[4].transform.position = tempPositionTwo;

            //yellow and red
            Vector3 tempPositionThree = selectables[3].transform.position;
            selectables[3].transform.position = selectables[1].transform.position;
            selectables[1].transform.position = tempPositionThree;

        //purple and blue 
        Vector3 tempPositionOneOne = walls[5].transform.position;
        walls[5].transform.position = walls[0].transform.position;
        walls[0].transform.position = tempPositionOneOne;

        //green and orange
        Vector3 tempPositionTwoTwo = walls[2].transform.position;
        walls[2].transform.position = walls[4].transform.position;
        walls[4].transform.position = tempPositionTwoTwo;

        //yellow and red
        Vector3 tempPositionThreeThree = walls[3].transform.position;
        walls[3].transform.position = walls[1].transform.position;
        walls[1].transform.position = tempPositionThreeThree;

    }

    void NewCoords()
    {
        /*
        X:
        purple and blue: +-3.8
        yellow and orange: 5.6-9.4
        red and green: -9.4--5.6
        
        Y: 0.8-4.2

        Z:
        yellow: -8.66/5 * x + 17.32
        orange: 8.66/5 * x - 17.32
        red: -8.66/5 * x - 17.32
        green: 8.66/5 * x + 17.32
         */

        //purple and blue 
       /* Vector3 tempPositionOne = selectables[5].transform.position;
        selectables[5].transform.position = selectables[0].transform.position;
        selectables[0].transform.position = tempPositionOne;

        //green and orange
        Vector3 tempPositionTwo = selectables[2].transform.position;
        selectables[2].transform.position = selectables[4].transform.position;
        selectables[4].transform.position = tempPositionTwo;

        //yellow and red
        Vector3 tempPositionThree = selectables[3].transform.position;
        selectables[3].transform.position = selectables[1].transform.position;
        selectables[1].transform.position = tempPositionThree;*/

        float x_publ = UnityEngine.Random.Range(-3.8f, 3.8f);
        float x_yeor = UnityEngine.Random.Range(5.6f, 9.4f);
        float x_regr = UnityEngine.Random.Range(-9.4f, -5.6f);
        float z_m = 8.66f / 5;
        float z_t = 17.32f;

        selectables[5].transform.position = new Vector3(x_publ, UnityEngine.Random.Range(0.8f, 2.7f), 8.66f);                 //purple
        selectables[0].transform.position = new Vector3(x_publ, UnityEngine.Random.Range(0.8f, 2.7f), -8.66f);                //blue
        selectables[2].transform.position = new Vector3(x_regr, UnityEngine.Random.Range(0.8f, 2.7f), z_m * x_regr + z_t);    //green
        selectables[4].transform.position = new Vector3(x_yeor, UnityEngine.Random.Range(0.8f, 2.7f), z_m * x_yeor - z_t);    //orange
        selectables[3].transform.position = new Vector3(x_yeor, UnityEngine.Random.Range(0.8f, 2.7f), -z_m * x_yeor + z_t);    //yellow
        selectables[1].transform.position = new Vector3(x_regr, UnityEngine.Random.Range(0.8f, 2.7f), -z_m * x_regr - z_t);    //red




    }

    /*
    selectables[0] = GameObject.Find("Blue");
    selectables[1] = GameObject.Find("Red");
    selectables[2] = GameObject.Find("Green");
    selectables[3] = GameObject.Find("Yellow");
    selectables[4] = GameObject.Find("Orange");
    selectables[5] = GameObject.Find("Purple");*/

}