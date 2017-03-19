namespace MIKA {
    interface ILeftFootReceiver : IDataReceiver {
        new void VectorData(float[] position, float[] rotation);
    }
}
