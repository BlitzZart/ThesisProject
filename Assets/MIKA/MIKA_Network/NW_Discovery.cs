using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace MIKA {
    /// <summary>
    /// NW_Discovery is a simple unet hlapi broadcast receiver script
    /// Mode
    ///     Both: -start listening
    ///             -if a server suitable is broadcasting and send "found" event
    ///             -if not - start broadcasting and send "start" event
    ///     Server:     -start broadcasting and sedn "start" event
    ///     Client: -start listening and send "found" if a suitable server is broadcasting
    /// </summary>
    public enum DiscoveryMode {
        Both, Server, Client
    }
    public class NW_Discovery : NetworkDiscovery {
        public delegate void NWDiscoverDelegateString(string ip, string data);
        public delegate void NWDiscoverDelegateSimple();
        public static event NWDiscoverDelegateString EventFoundServer;
        public static event NWDiscoverDelegateSimple EventStartServer;

        private string gameName = "MIKA";
        private IEnumerator initiateServerStartCoroutine;
        #region unity callbacks
        void Start() {
            // initialize discovery
            Initialize();
            //// set data
            broadcastData = gameName;

            DiscoveryMode mode = ServerManager.Instance.mode;

            // android devices allways start as clients
#if UNITY_ANDROID && !UNITY_EDITOR
            mode = DiscoveryMode.Client;
#endif

            if (mode == DiscoveryMode.Both || mode == DiscoveryMode.Client) {
                //// start listening
                StartCoroutine(StartAsClientDelayed());
            }

            if (mode == DiscoveryMode.Both || mode == DiscoveryMode.Server) {
                float delay = 0.2f;

                // set a delay if mode is set to both
                if (mode == DiscoveryMode.Both)
                    delay = 5;

                //// set broadcast coroutine
                initiateServerStartCoroutine = InitiateServerStart(delay);
                //// start broadcast coroutine
                StartCoroutine(initiateServerStartCoroutine);
            }
        }

        public override void OnReceivedBroadcast(string fromAddress, string data) {
            base.OnReceivedBroadcast(fromAddress, data);
            // check if data is matching
            if (!data.Contains(gameName))
                return;
            // prevent broadcasting (Server) start
            if (initiateServerStartCoroutine != null)
                StopCoroutine(initiateServerStartCoroutine);
            // send found server event
            // ! NetworManager must handle/not handle furhter events (when it's allready connected)!
            if (EventFoundServer != null)
                EventFoundServer(fromAddress, data);
        }

        #endregion
        // TODO: this is only needed becaus of a likely unity bug
        IEnumerator StartAsClientDelayed() {
            yield return 0;
            Initialize(); // second init is necessary!
            StartAsClient();
        }

        private IEnumerator InitiateServerStart(float delay) {
            yield return new WaitForSeconds(delay);

            // stop listening
            if (isClient)
                StopBroadcast();
            else
                Initialize();

            // send start server event
            if (EventStartServer != null)
                EventStartServer();

            // start broadcasting
            StartAsServer();
        }
    }
}