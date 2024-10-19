using System.Collections.Concurrent;
using Microsoft.VisualBasic.CompilerServices;

namespace TinyUrl.Repository
{
    // not used after introducing SqlRepository
    public sealed class InMemoryRepository : IUrlRepository
    {
        private readonly ConcurrentDictionary<int, string> _idToUrl= new();
        private readonly ConcurrentDictionary<string, int> _codeToId = new();
        private static readonly object Lock = new();

        public Task<int> GenerateIdAsync(Uri url, CancellationToken cancellationToken)
        {
            lock (InMemoryRepository.Lock)
            {
                int id = this._idToUrl.Count;
                this._idToUrl[id] = url.ToString();
                return Task.FromResult(id);
            }
        }

        public Task<bool> TryUpdateCodeAsync(int id, string code, CancellationToken cancellationToken)
        {
            this._codeToId[code] = id;
            return Task.FromResult(true);
        }

        public Task<Uri?> TryGetUrlByCodeAsync(string code, CancellationToken cancellationToken)
        {
            if (this._codeToId.TryGetValue(code, out int id))
            {
                return Task.FromResult<Uri?>(new Uri(this._idToUrl[id]));
            }
            return Task.FromResult<Uri?>(null);
        }
    }
}
