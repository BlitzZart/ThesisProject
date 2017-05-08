using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace MIKA {
    public class MIKARecorder : MonoBehaviour {
        string toLoadPath = "ToLoad/";
        string path = "Recordings/";
        string fileName = "viveRec";
        string type = ".mika";
        public List<UnityTrackedData> newData;
        public List<UnityTrackedData> loadedData;
        public Transform recTracker01, recTracker02;
        public GameObject playTracker01, playTracker02;
        private UnityViveTracking player;

        public enum RecorderMode {
            DoNothing, Play, Record
        }
        public RecorderMode mode = RecorderMode.Record;

        #region unity callbacks
        private void Awake() {
            newData = new List<UnityTrackedData>();
            if (mode == RecorderMode.Play) {
                LoadData();

                playTracker01 = new GameObject("playTracker01");
                playTracker01.tag = "ViveTracker01";
                playTracker02 = new GameObject("playTracker02");
                playTracker02.tag = "ViveTracker02";
            }
        }
        private void OnApplicationQuit() {
            if (mode == RecorderMode.Record)
                WriteData();
        }
        private void FixedUpdate() {
            if (mode == RecorderMode.Record)
                RecordData();
            else if (mode == RecorderMode.Play)
                PlayData();
        }
        #endregion

        #region private
        private void RecordData() {
            // check if player is tracked
            if (player == null) {
                player = VivePlayerManager.Instance.trackedEntity;

                if (player == null) {
                    return;
                }
            }
            // check if feet are available
            if (recTracker01 == null || recTracker02 == null) {
                recTracker01 = player.viveLeftFoot.transform;
                recTracker02 = player.viveRightFoot.transform;

                if (recTracker01 == null || recTracker02 == null) {
                    return;
                }
            }

            newData.Add(new UnityTrackedData(recTracker01.position, recTracker01.rotation.eulerAngles));
            newData.Add(new UnityTrackedData(recTracker02.position, recTracker02.rotation.eulerAngles));
        }

        int i = 0;
        private void PlayData() {
            if (i > loadedData.Count - 1)
                i = 0;
            
            playTracker01.transform.position = new Vector3(loadedData[i].pos[0], loadedData[i].pos[1], loadedData[i].pos[2]);
            playTracker01.transform.rotation = Quaternion.Euler(new Vector3(loadedData[i].rot[0], loadedData[i].rot[1], loadedData[i].rot[2]));

            playTracker02.transform.position = new Vector3(loadedData[i+1].pos[0], loadedData[i+1].pos[1], loadedData[i + 1].pos[2]);
            playTracker02.transform.rotation = Quaternion.Euler(new Vector3(loadedData[i + 1].rot[0], loadedData[i + 1].rot[1], loadedData[i + 1].rot[2]));

            i += 2;
        }
        private void WriteData() {
            // create directoris if not exist
            Directory.CreateDirectory(path);
            Directory.CreateDirectory(path + toLoadPath);

            IFormatter formatter = new BinaryFormatter();
            DateTime now = DateTime.Now;
            string time = now.Year.ToString() + "_" + now.Month.ToString() + "_" + now.Day.ToString()
                + "_" + now.Hour.ToString() + now.Minute.ToString() + now.Second.ToString();
            Stream stream = new FileStream(path + fileName + type + time, FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, newData);
            stream.Close();
        }
        private void LoadData() {
            string toLoad = GetFilePathToLoad();
            if (toLoad == "")
                return;

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(toLoad, FileMode.Open, FileAccess.Read, FileShare.Read);

            loadedData = (List<UnityTrackedData>)formatter.Deserialize(stream);
            stream.Close();
        }
        private string GetFilePathToLoad() {
            string[] files = Directory.GetFiles(path + toLoadPath);
            if (files != null && files.Length > 0) {
                print(files[0]);
                return files[0];
            }
            return "";
        }
        #endregion
    }
}