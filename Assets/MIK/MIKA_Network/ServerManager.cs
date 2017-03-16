using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

namespace MIKA {
    public class ServerManager : NetworkManager {
        public string ConnectionIP = "localhost";
        public int ConnectionPort = 7777;
        public bool isServer;
        // Use this for initialization
        void Start() {
            StartCoroutine(StartupHost());
        }

        // Update is called once per frame
        void Update() {

        }


        public IEnumerator StartupHost() {
            yield return new WaitForSeconds(1);
            SetPort();
            StartServer();
            isServer = true;
        }

        void SetIPAddress() {
            networkAddress = ConnectionIP;
        }
        void SetPort() {
            networkPort = ConnectionPort;
        }
    }
}