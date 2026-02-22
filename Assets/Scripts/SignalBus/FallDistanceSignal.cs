
namespace Player.Signals
{
    public sealed class FallDistanceSignal
    {
        public float FallDistance { get; }
        public FallDistanceSignal(float fallDistance)
        {
            FallDistance = fallDistance;
        }
    }
}