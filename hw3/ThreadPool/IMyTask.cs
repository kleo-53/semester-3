namespace ThreadPool;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IMyTask <TResult>
{
    public bool IsCompleted { get; }

    public TResult Result { get; }

    public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> function);
}
