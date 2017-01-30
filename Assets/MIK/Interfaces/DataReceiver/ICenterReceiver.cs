namespace ModularIK {
    interface ICenterReceiver : IDataReceiver {
        new void VectorData(float[] position, float[] rotation);
    }
}
