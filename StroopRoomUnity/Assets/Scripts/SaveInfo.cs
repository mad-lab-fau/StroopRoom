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
using UnityEngine;
using System.IO;
using System;

public class SaveInfo : MonoBehaviour {

    public static int subjectID = 0;
    public static int roomIndex;
    public static SystemLanguage language;
    public static StroopRoomDebug debugSystem = StroopRoomDebug.Off;

    /// <summary>
    /// The following three properties are important for the study protocol execution and need to be set by the experiment supervisor.
    /// TOOD: highlight them in the Unity GUI using https://docs.unity3d.com/ScriptReference/PropertyDrawer.html or https://docs.unity3d.com/ScriptReference/DecoratorDrawer.html
    /// </summary>
    public int predefinedSubjectId = -1;
    public StroopRoomCondition predefinedCondition = StroopRoomCondition.ZERO;
    public StroopRoomSequence predefinedPhaseSequnce = StroopRoomSequence.CONGRUENT_INCONGRUENT;
    public StroopRoomDebug debug = StroopRoomDebug.Off;

    public static string performancePath = null;
    public static string trackingPath = null;

    public static string csvColSeparator = "\t";

    public static StroopRoomSequence phaseSequence = StroopRoomSequence.CONGRUENT_INCONGRUENT;

    /// <summary>
    /// Duration of the break between congruent and incongruent trials.
    /// </summary>
    public const float PHASE_BREAK_DURATION = 300f;
    /// <summary>
    /// Number of practice stimuli. TODO: Check paper for consistent naming.
    /// </summary>
    public const int NUM_PRACTICE_STIMULI = 15;
    /// <summary>
    /// Number of trial stimuli.
    /// </summary>
    public const int NUM_TRIAL_STIMULI = 120;

    public enum StroopRoomSequence
    {
        UNKNOWN,
        CONGRUENT_INCONGRUENT,
        INCONGRUENT_CONGRUENT
    }

    public enum StroopRoomCondition
    {
        NONE,
        ZERO,
        A,
        B,
        C
    }

    public enum StroopRoomDebug
    {
        Off,
        Debug,
        DebugSuperfast
    }

    public enum StroopRoomPhase
    {
        NONE,
        SIGNAL_TEST,
        RESTING_BASELINE,
        /// <summary>
        /// While they are already in the Stroop Room able to select their room and language
        /// </summary>
        PREPARATION,
        CONGRUENT_PRACTICE,
        CONGRUENT_TRIAL,
        RESTING_BREAK,
        INCONGRUENT_PRACTICE,
        INCONGRUENT_TRIAL,
        RESTING_RECOVERY,
        END_OF_EXPERIMENT
    }

    /// <summary>
    /// Status of the Stroop Room. Rework this entire file. TODO: SaveInfo could be also included as status. Make it factory static, or maybe use in-game object for life-time management.
    /// </summary>
    public class StroopRoomStatus
    {
        /// <summary>
        /// Indicated whether someone is currently running a room and which one.
        /// TODO: change this to a class property and an enum type with character toString representation.
        /// </summary>
        public StroopRoomCondition condition = StroopRoomCondition.NONE;

        public StroopRoomPhase phase = StroopRoomPhase.NONE;

        // TODO: Include in property!
        public void SetCondition(StroopRoomCondition cond = StroopRoomCondition.NONE)
        {
            condition = cond;
        }

        public void SetPhase(StroopRoomPhase phase)
        {
            this.phase = phase;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>True if room currently in the practice phase.</returns>
        public bool IsPracticePhase()
        {
            return (phase == StroopRoomPhase.CONGRUENT_PRACTICE || phase == StroopRoomPhase.INCONGRUENT_PRACTICE);
        }
        public bool IsCongruentPhase()
        {
            return (phase == StroopRoomPhase.CONGRUENT_PRACTICE || phase == StroopRoomPhase.CONGRUENT_TRIAL);
        }

        private bool transitionPhase = false;

        public int difficulty = 0;
        public float pauseTimeLeft = PHASE_BREAK_DURATION;

        public bool TransitionPhase { get => transitionPhase; set => transitionPhase = value; }
    }
            
    public static StroopRoomStatus roomStatus = new StroopRoomStatus();

    /// <summary>
    /// Returns a standardized timestamp string.
    /// </summary>
    /// <returns></returns>
    public static string strTimestamp()
    {
        return System.DateTime.Now.ToString("HH:mm:ss:fff");        
    }

    public static void SetID(int id)
    {
        subjectID = id;
        Debug.Log("Subject ID set to " + subjectID);
        SetLogPaths();
    }

    /// <summary>
    /// From https://stackoverflow.com/questions/20405965/how-to-ensure-there-is-trailing-directory-separator-in-paths
    /// </summary>
    /// <returns>The add backslash.</returns>
    /// <param name="path">Path.</param>
    private static string PathAddBackslash(string path)
    {
        if (path == null)
            throw new ArgumentNullException(nameof(path));

        path = path.TrimEnd();

        if (PathEndsWithDirectorySeparator())
            return path;

        return path + GetDirectorySeparatorUsedInPath();

        bool PathEndsWithDirectorySeparator()
        {
            if (path.Length == 0)
                return false;

            char lastChar = path[path.Length - 1];
            return lastChar == Path.DirectorySeparatorChar
                || lastChar == Path.AltDirectorySeparatorChar;
        }

        char GetDirectorySeparatorUsedInPath()
        {
            if (path.Contains(Path.AltDirectorySeparatorChar.ToString()))
                return Path.AltDirectorySeparatorChar;

            return Path.DirectorySeparatorChar;
        }
    }

    /// <summary>
    /// Sets the log paths for the current subject.
    /// </summary>
    public static void SetLogPaths()
    {
        string myDocFolder = PathAddBackslash(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments));
        string basePath = myDocFolder + "StroopRoomLogs";
        Directory.CreateDirectory(basePath);
        basePath += Path.DirectorySeparatorChar + "LogID";
        performancePath = basePath + subjectID + ".csv";
        trackingPath = basePath + subjectID + "tracking.csv";

        int subId = 0;
        while (File.Exists(performancePath))
        {
            subId++;
            performancePath = basePath + subjectID + "_" + subId + ".csv";
            trackingPath = basePath + subjectID + "_" + subId + "tracking.csv";
        }

        // Write CSV header lines with general subject, date and column definitions.
        string cs = csvColSeparator;
        File.WriteAllText(performancePath, StroopTask.logHeader(subjectID));
        File.WriteAllText(trackingPath, MovementTracking.logHeader(subjectID));

        Debug.Log("Subject path set to: " + performancePath);
    }

    public static void SetRoom(int room)
    {
        roomIndex = room;
    }

    public static void SetLang(SystemLanguage lang)
    {
        language = lang;
    }

    public static int GetID()
    {
        return subjectID;
    }

    public static int GetRoom()
    {
        return roomIndex;
    }

    public static SystemLanguage GetLanguage()
    {
        return language;
    }

    void Start()
    {
        debugSystem = debug;
        if (predefinedSubjectId == -1)
        {
            Debug.LogError("Subject ID not set!");
        }
        else
        {
            SetID(predefinedSubjectId);
            phaseSequence = predefinedPhaseSequnce;
        }

        if (debugSystem == StroopRoomDebug.DebugSuperfast)
        {
            roomStatus.pauseTimeLeft = 5f;
        }
    }
}
