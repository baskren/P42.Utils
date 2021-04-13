using System;
using System.Threading.Tasks;

namespace P42.Utils
{
    public static class LongRunningTask
    {
        public static async Task Start(Action action)
            => await Start(action, (Action)null);

        public static async Task Start(Func<Task> action)
            => await Start(action, (Action)null);

        public static async Task<T> Start<T>(Func<T> action)
            => await Start(action, (Func<T>)null);

        public static async Task<T> Start<T>(Func<Task<T>> action)
            => await Start(action, (Func<T>)null);



        public static async Task<T> Start<T>(Func<T> func, Func<T> cancellationAction)
        {
            if (Environment.PlatformLongRunningTaskType is null)
                throw new Exception("Enviroment not set.  Did you initiate P42.Utils.**platform**?");
            var task = (ILongRunningTask)Activator.CreateInstance(Environment.PlatformLongRunningTaskType);
            return await task.RunAsync(func, cancellationAction);
        }

        public static async Task<T> Start<T>(Func<Task<T>> func, Func<T> cancellationAction)
        {
            if (Environment.PlatformLongRunningTaskType is null)
                throw new Exception("Enviroment not set.  Did you initiate P42.Utils.**platform**?");
            var task = (ILongRunningTask)Activator.CreateInstance(Environment.PlatformLongRunningTaskType);
            return await task.RunAsync(func, cancellationAction);
        }

        public static async Task<T> Start<T>(Func<T> func, Func<Task<T>> cancellationAction)
        {
            if (Environment.PlatformLongRunningTaskType is null)
                throw new Exception("Enviroment not set.  Did you initiate P42.Utils.**platform**?");
            var task = (ILongRunningTask)Activator.CreateInstance(Environment.PlatformLongRunningTaskType);
            return await task.RunAsync(func, cancellationAction);
        }

        public static async Task<T> Start<T>(Func<Task<T>> func, Func<Task<T>> cancellationAction)
        {
            if (Environment.PlatformLongRunningTaskType is null)
                throw new Exception("Enviroment not set.  Did you initiate P42.Utils.**platform**?");
            var task = (ILongRunningTask)Activator.CreateInstance(Environment.PlatformLongRunningTaskType);
            return await task.RunAsync(func, cancellationAction);
        }



        public static async Task Start(Action action, Action cancellationAction)
        {
            if (Environment.PlatformLongRunningTaskType is null)
                throw new Exception("Enviroment not set.  Did you initiate P42.Utils.**platform**?");
            var task = (ILongRunningTask)Activator.CreateInstance(Environment.PlatformLongRunningTaskType);
            await task.RunAsync(action, cancellationAction);
        }

        public static async Task Start(Action action, Func<Task> cancellationAction)
        {
            if (Environment.PlatformLongRunningTaskType is null)
                throw new Exception("Enviroment not set.  Did you initiate P42.Utils.**platform**?");
            var task = (ILongRunningTask)Activator.CreateInstance(Environment.PlatformLongRunningTaskType);
            await task.RunAsync(action, cancellationAction);
        }

        public static async Task Start(Func<Task> action, Action cancellationAction)
        {
            if (Environment.PlatformLongRunningTaskType is null)
                throw new Exception("Enviroment not set.  Did you initiate P42.Utils.**platform**?");
            var task = (ILongRunningTask)Activator.CreateInstance(Environment.PlatformLongRunningTaskType);
            await task.RunAsync(action, cancellationAction);
        }

        public static async Task Start(Func<Task> action, Func<Task> cancellationAction)
        {
            if (Environment.PlatformLongRunningTaskType is null)
                throw new Exception("Enviroment not set.  Did you initiate P42.Utils.**platform**?");
            var task = (ILongRunningTask)Activator.CreateInstance(Environment.PlatformLongRunningTaskType);
            await task.RunAsync(action, cancellationAction);
        }

    }
}
