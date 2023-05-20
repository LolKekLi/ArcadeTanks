namespace Project.Service
{
    public class DefaultAnalyticsService : IAnalyticsService
    {
        public void TrackStart()
        {
            DebugSafe.Log("Track Start");
        }

        public void TrackFinish()
        {
            DebugSafe.Log("Track Finish");
        }

        public void TrackFail()
        {
            DebugSafe.Log("Track Fail");
        }
    }
}