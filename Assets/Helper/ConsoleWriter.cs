namespace Assets.Helper {
    class ConsoleWriter : Singleton<ConsoleWriter> {
        public void Write(object type, string msg) {
            UnityEngine.Debug.Log(type.GetType().Name + " " + msg);
        }
        public void WriteError(object type, string msg) {
            UnityEngine.Debug.LogError(type.GetType().Name + " " + msg);
        }
    }
}