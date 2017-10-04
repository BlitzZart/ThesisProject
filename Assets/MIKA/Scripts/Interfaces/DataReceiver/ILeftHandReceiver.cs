namespace MIKA {
    interface ILeftHandReceiver : IDataReceiver {
        new void VectorData(float[] position, float[] rotation);
    }
}
