namespace MIKA {
    interface IHeadReceiver : IDataReceiver {
        new void VectorData(float[] position, float[] rotation);
    }
}
