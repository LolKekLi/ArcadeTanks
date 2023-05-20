namespace Project.Service
{
    public interface IAnalyticsService
    {
        public abstract void TrackStart();
        public abstract void TrackFinish();
        public abstract void TrackFail();
    }
}