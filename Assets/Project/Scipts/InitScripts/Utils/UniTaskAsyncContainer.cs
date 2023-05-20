using System;
using System.Collections.Generic;
using System.Threading;

namespace Project
{
    public class UniTaskAsyncContainer : IDisposable
    {
        private readonly List<CancellationTokenSource> AsyncsContainer = null;

        public UniTaskAsyncContainer()
        {
            AsyncsContainer = new List<CancellationTokenSource>();
        }

        public CancellationToken RefreshToken(ref CancellationTokenSource tokenSource)
        {
            var token = UniTaskUtil.RefreshToken(ref tokenSource);
            
            AsyncsContainer.Add(tokenSource);

            return token;
        }

        public void CancelToken(ref CancellationTokenSource tokenSource)
        {
            AsyncsContainer.Remove(tokenSource);
            
            UniTaskUtil.CancelToken(ref tokenSource);
        }
        
        public void Dispose()
        {
            AsyncsContainer.Do(a => CancelToken(ref a));
        }
    }
}