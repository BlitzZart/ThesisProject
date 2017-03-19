namespace MIKA {
    interface IRightHandReceiver : IDataReceiver {
        new void VectorData(float[] position, float[] rotation);
    }
}
