namespace MIKA {
    interface IRightFootReceiver : IDataReceiver {
        new void VectorData(float[] position, float[] rotation);
    }
}