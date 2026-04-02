using System;
using System.Threading.Tasks;

namespace ContextMenuCustomApp.View.Common
{
    public abstract class BaseViewModel : BaseModel
    {
        private bool _busy;
        public bool IsBusy { get => _busy; set => SetProperty(ref _busy, value); }

        private string _message;
        public string Message { get => _message; set => SetProperty(ref _message, value); }

        public delegate void MessageEventHandler(string message, Exception exception);
        public event MessageEventHandler Handler;

        public void OnError(Exception e, string message = null)
        {
            Message = message ?? e.Message;
            Handler?.Invoke(message, e);
        }

        public void OnMessage(string message)
        {
            Handler?.Invoke(message, null);
        }

        public void Busy(bool busy, string message = null)
        {
            IsBusy = busy;
            Message = message ?? string.Empty;
        }

        public void RunWith(Action action)
        {
            Busy(true);
            Message = string.Empty;
            try
            {
                action();
            }
            catch (Exception e)
            {
                Message = e.Message;
                Handler?.Invoke(Message, e);
            }
            finally
            {
                Busy(false);
            }
        }

        public async Task RunWith(Func<Task> action)
        {
            Busy(true);
            Message = string.Empty;
            try
            {
                await action();
            }
            catch (Exception e)
            {
                Message = e.Message;
                Handler?.Invoke(Message, e);
            }
            finally
            {
                Busy(false);
            }
        }

        public async Task<T> RunWith<T>(Func<Task<T>> action)
        {
            Busy(true);
            Message = string.Empty;
            try
            {
                return await action();
            }
            catch (Exception e)
            {
                Message = e.Message;
                Handler?.Invoke(Message, e);
            }
            finally
            {
                Busy(false);
            }

            return default;
        }
    }
}
