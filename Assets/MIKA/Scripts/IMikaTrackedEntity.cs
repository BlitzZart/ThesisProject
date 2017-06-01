using UnityEngine;

namespace MIKA {
    public interface IMikaTrackedEntity {
        int GetID();
        void SetID(int ID);

        UnityModelDataManager GetModelDataManager();
    }
}