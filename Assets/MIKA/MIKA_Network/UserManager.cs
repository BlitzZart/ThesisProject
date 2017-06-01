using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MIKA {
    public class UserManager : Singleton<UserManager> {
        private static UserManager instance;
        //public static UserManager Instance { get { return instance; } }

        public List<NetworkPlayer> networkPlayers;
        public NetworkPlayer nwPlayer1, nwPlayer2;
        public List<IMikaTrackedEntity> trackedEntities;

        #region unity callbacks
        private void Awake() {

            networkPlayers = new List<NetworkPlayer>();
            if (trackedEntities == null)
                trackedEntities = new List<IMikaTrackedEntity>();
        }
        #endregion


        public void AssignUserToEntity(int user, int entity) {
            UnityModelDataManager mdm = null;
            if (entity != 0) {
                foreach (IMikaTrackedEntity item in trackedEntities)
                    if (item.GetID() == entity)
                        mdm = item.GetModelDataManager();

                if (mdm == null)
                    return;
            }

            if (user == 1) {
                nwPlayer1.AssignModelDataManager(mdm);
            }
            else if (user == 2) {
                nwPlayer2.AssignModelDataManager(mdm);
            }
        }

        public List<NetworkPlayer> GetAllNetworkPlayers() {
            return networkPlayers;
        }
        public List<IMikaTrackedEntity> GetAllTrackedEntities() {
            return trackedEntities;
        }

        public void AddNetworkPlayer(NetworkPlayer nwp) {
            List<int> usedIDs = new List<int>();
            foreach (NetworkPlayer item in networkPlayers)
                usedIDs.Add(item.ID);

            for (int i = 1; i <= 10; i++) {
                if (!usedIDs.Contains(i)) {
                    nwp.ID = i;
                    break;
                }
            }
            if (nwp.ID == 1) {
                nwPlayer1 = nwp;
            } else if (nwp.ID == 2) {
                nwPlayer2 = nwp;
            }


            networkPlayers.Add(nwp);
        }
        public void RemoveNetworkPlayer(NetworkPlayer nwp) {
            networkPlayers.Remove(nwp);

            if (nwPlayer1 == nwp)
                nwPlayer1 = null;
            else if (nwPlayer2 == nwp)
                nwPlayer2 = null;

        }
        public void AddTrackedEntity(IMikaTrackedEntity entity) {
            if (trackedEntities == null)
                trackedEntities = new List<IMikaTrackedEntity>();

            trackedEntities.Add(entity);
        }
        public void RemoveTrackedEntity(IMikaTrackedEntity entity) {
            trackedEntities.Remove(entity);
        }
    }
}