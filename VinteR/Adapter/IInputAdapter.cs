using VinteR.Model;

namespace VinteR.Adapter
{
    public delegate void MocapFrameAvailableEventHandler(IInputAdapter adapter, MocapFrame frame);

    public interface IInputAdapter
    {
        event MocapFrameAvailableEventHandler FrameAvailable;
    }
}
