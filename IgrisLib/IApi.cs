namespace IgrisLib
{
    public interface IApi
    {
        string Name { get; }
        bool ConnectTarget();
        bool AttachProcess();
        int DisconnectTarget();
        void DetachProcess();
        void SetMemory(uint address, byte[] buffer);
        void GetMemory(uint address, byte[] buffer);
        byte[] GetBytes(uint address, uint length);
    }
}
