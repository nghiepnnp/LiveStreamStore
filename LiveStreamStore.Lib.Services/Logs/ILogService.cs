using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LiveStreamStore.Lib.Services.Logs
{
    public interface ILogService
    {
        string WriteLog(MethodBase methodBase, params object[] values);
        string WriteLogFrontend(MethodBase methodBase, params object[] values);
    }
}
