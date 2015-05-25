using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputationalCluster.Common
{
    /// <summary>
    /// Provider dla czasu, aby umożliwić testowanie.
    /// </summary>
    public interface ITimeProvider
    {
        DateTime Now { get; }
    }

    /// <summary>
    /// Podstawowa implementacja zwracająca aktualny czas.
    /// </summary>
    public class TimeProviderUtcNow : ITimeProvider
    {
        public DateTime Now
        {
            get
            {
                return DateTime.UtcNow;
            }
        }
    }
}
