using System.Threading;

namespace Project
{
    public static class UniTaskUtil
    {
        public static CancellationToken RefreshToken(ref CancellationTokenSource tokenSource)
        {
            CancelToken(ref tokenSource);
            tokenSource = new CancellationTokenSource();
            return tokenSource.Token;
        }

        public static void CancelToken(ref CancellationTokenSource tokenSource)
        {
            tokenSource?.Cancel();
            tokenSource?.Dispose();
            tokenSource = null;
        }
    }
}