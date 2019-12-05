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

/// <summary>
/// Manages the tracking and logging of movement information from the subject (headset and controllers).
/// </summary>
public class MovementTracking : MonoBehaviour
{
    public GameObject RightController;
    private static SteamVR_TrackedObject steamController;

    private const float SAMPLING_INTERVAL = 0.05f; // default sampling interval in seconds for the movement tracking
    private static float movementTrackingTimeCounter = 0; // time since last movement tracking sample

    /// <summary>
    /// How often to write buffered movement data log to disk.
    /// </summary>
    private const float DISK_WRITE_INTERVAL = 1f;
    private static float diskWriteWaitCounter = 0f;
    private static string textBuffer = "";

    private static string trackpath = null;       //path of tracking log file

    void Init()
    {
        steamController = RightController.GetComponent<SteamVR_TrackedObject>();
        trackpath = SaveInfo.trackingPath;
        textBuffer = "";
        diskWriteWaitCounter = 0f;
    }

    // Update is called once per frame
    public void Update()
    {
        // Only log movement while a room is "in session"
        if (SaveInfo.roomStatus.condition == SaveInfo.StroopRoomCondition.NONE)
            return;

        if (trackpath == null)
            Init();

        // TODO: sampling system doesn't seem to work perfectly. in the log we have a sampling rate of 15 Hz, instead of the expected 20 Hz. This might be a problem with missed frames or something. Try Time.realtime...
        movementTrackingTimeCounter += Time.deltaTime;
        diskWriteWaitCounter += Time.deltaTime;

        if (movementTrackingTimeCounter > SAMPLING_INTERVAL)
        {
            movementTrackingTimeCounter = 0;
            TrackHeadAndController();            
        }

        if (diskWriteWaitCounter > DISK_WRITE_INTERVAL)
        {            
            WriteToDisk();
        }
    }

    /// <summary>
    /// Returns the csv header line for log entries (the column definitions).
    /// </summary>
    /// <returns></returns>
    public static string logHeader(int subjectID)
    {
        string cs = SaveInfo.csvColSeparator;
        return "SubjectID: " + subjectID + "\nDate: " + System.DateTime.Now.ToString("dd/MM/yyyy") + "\nTime: " + System.DateTime.Now.ToString("HH:mm:ss:fff") + "\nFileversion: 3\n\n" + "time" + cs + "stID" + cs + "viewHitX" + cs + "viewHitY" + cs + "viewHitZ" + cs + "headX" + cs + "headY" + cs + "headZ" + cs + "headTarget" + cs + "controllerHitX" + cs + "controllerHitY" + cs + "controllerHitZ" + cs + "controllerX" + cs + "controllerY" + cs + "controllerZ" + cs + "controllerTarget\n";
    }

    // Tracks HMD and Controller
    public static void TrackHeadAndController(int logSTID = -1)
    {
        string cs = SaveInfo.csvColSeparator;

        RaycastHit hitHead;
        Ray rayHead = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        RaycastHit hitController;
        Ray rayController = new Ray(steamController.transform.position, steamController.transform.forward);

        textBuffer += SaveInfo.strTimestamp() + cs + logSTID + cs;

        if (Physics.Raycast(rayHead, out hitHead, 100.0f))
        {
            textBuffer += hitHead.point.x + cs + hitHead.point.y + cs + hitHead.point.z + cs + Camera.main.transform.position.x + cs + Camera.main.transform.position.y + cs + Camera.main.transform.position.z + cs + hitHead.collider.gameObject.name + cs;        
        }

        if (Physics.Raycast(rayController, out hitController, 100.0f))
        {
            textBuffer += hitController.point.x + cs + hitController.point.y + cs + hitController.point.z + cs + steamController.transform.position.x + cs + steamController.transform.position.y + cs + steamController.transform.position.z + cs + hitController.collider.gameObject.name;            
        }

        textBuffer += "\n";

        // If it is an important move information (a selected item), write the buffer to disk immediately.
        if (logSTID != -1)
            WriteToDisk();        
    }

    /// <summary>
    /// Writes the text buffer to disk.
    /// </summary>
    private static void WriteToDisk()
    {
        diskWriteWaitCounter = 0;
        File.AppendAllText(trackpath, textBuffer);
        textBuffer = "";
    }
}
