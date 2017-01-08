using UnityEngine;
namespace Assets.Helper {
    class ConsoleWriter : Singleton<ConsoleWriter> {
        public void Write(Object type, string msg) {
            Debug.Log(type.GetType().Name + msg);
        }
    }
}
