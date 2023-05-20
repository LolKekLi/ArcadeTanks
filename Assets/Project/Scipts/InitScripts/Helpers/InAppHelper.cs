namespace Project
{
    public static class InAppHelper
    {
        public static bool IsInAppEnabled
        {
            get
            {
                bool isEnabled = false;

#if INAPP_ENABLED
                isEnabled = true;
#endif
                return isEnabled;
            }
        }
    }
}