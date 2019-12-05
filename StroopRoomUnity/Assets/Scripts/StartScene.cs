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
using DFTGames.Localization;
using UnityEngine;
using UnityEngine.UI;

public class StartScene : MonoBehaviour {


    bool selected;
    private int id;
    private Text subjectID;
    private Text welcome;
    private GameObject selectionText;
    private GameObject dontDestroy;

    private Button buttonZero;
    private Button buttonA;
    private Button buttonB;
    private Button buttonC;
    ColorBlock cbZ;
    ColorBlock cbA;
    ColorBlock cbB;
    ColorBlock cbC;

    Color grey = new Color(0f,0f,0f,0f);


    void Start () {
        //SetLanguage=de deutsch default
        Localize.SetCurrentLanguage(SystemLanguage.German);
        SaveInfo.SetLang(SystemLanguage.German);
        id = SaveInfo.GetID();
        subjectID = GameObject.Find("SubjectID").GetComponent<Text>();
        welcome = GameObject.Find("Welcome").GetComponent<Text>();
        selectionText = GameObject.Find("SelectionText");
        dontDestroy = GameObject.Find("DontDestroy");
        buttonZero = GameObject.Find("ButtonZero").GetComponent<Button>();
        buttonA = GameObject.Find("ButtonA").GetComponent<Button>();
        buttonB = GameObject.Find("ButtonB").GetComponent<Button>();
        buttonC = GameObject.Find("ButtonC").GetComponent<Button>();
        subjectID.text = "ID: " + id;
        selected = false;
        cbZ = buttonZero.colors;
        cbA = buttonA.colors;
        cbB = buttonB.colors;
        cbC = buttonC.colors;
    }

    void Update()
    {
        welcome.text = string.Format(welcome.text, "\n");
    }

    public void ButtonZero()
    {
        SaveInfo.SetRoom(0);
        selected = true;
        selectionText.GetComponent<Localize>().localizationKey = "room0";
        ResetLanguage();
        cbZ.normalColor = Color.white;
        cbA.normalColor = grey;
        cbB.normalColor = grey;
        cbC.normalColor = grey;
    }
    public void ButtonA()
    {
        SaveInfo.SetRoom(1);
        selected = true;
        selectionText.GetComponent<Localize>().localizationKey = "roomA";
        ResetLanguage();
        cbZ.normalColor = grey;
        cbA.normalColor = Color.white;
        cbB.normalColor = grey;
        cbC.normalColor = grey;
    }
    public void ButtonB()
    {
        SaveInfo.SetRoom(2);
        selected = true;
        selectionText.GetComponent<Localize>().localizationKey = "roomB";
        ResetLanguage();
        cbZ.normalColor = grey;
        cbA.normalColor = grey;
        cbB.normalColor = Color.white;
        cbC.normalColor = grey;
    }
    public void ButtonC()
    {
        SaveInfo.SetRoom(3);
        selected = true;
        selectionText.GetComponent<Localize>().localizationKey = "roomC";
        ResetLanguage();
        cbZ.normalColor = grey;
        cbA.normalColor = grey;
        cbB.normalColor = grey;
        cbC.normalColor = Color.white;
    }
    public void OkButton(int roomIdx)
    {
        if (selected)
        {
            DontDestroyOnLoad(dontDestroy);
            UnityEngine.SceneManagement.SceneManager.LoadScene(1);
        }
        else
        {
            selectionText.GetComponent<Localize>().localizationKey = "noselection";
            ResetLanguage();
        }
    }
    private void ResetLanguage()
    {
        if (Locale.CurrentLanguage == "German")
        {
            Localize.SetCurrentLanguage(SystemLanguage.German);
        }
        else if (Locale.CurrentLanguage == "Dutch")
        {
            Localize.SetCurrentLanguage(SystemLanguage.Dutch);
        }
        else
        {
            Localize.SetCurrentLanguage(SystemLanguage.English);
        }
    }
}
