using System.Collections.Concurrent;

namespace SpirithubCofe.Web.Services;

public class ToastService
{
    public enum ToastType { Success, Error, Warning, Info }
    
    public class ToastMessage
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Message { get; set; } = "";
        public ToastType Type { get; set; }
        public bool IsVisible { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
    
    private readonly ConcurrentQueue<ToastMessage> _toasts = new();
    
    public event Action? OnToastAdded;
    
    public void ShowSuccess(string message)
    {
        ShowToast(message, ToastType.Success);
    }
    
    public void ShowError(string message)
    {
        ShowToast(message, ToastType.Error);
    }
    
    public void ShowWarning(string message)
    {
        ShowToast(message, ToastType.Warning);
    }
    
    public void ShowInfo(string message)
    {
        ShowToast(message, ToastType.Info);
    }
    
    private void ShowToast(string message, ToastType type)
    {
        var toast = new ToastMessage
        {
            Message = message,
            Type = type
        };
        
        _toasts.Enqueue(toast);
        OnToastAdded?.Invoke();
    }
    
    public IEnumerable<ToastMessage> GetToasts()
    {
        var toasts = new List<ToastMessage>();
        while (_toasts.TryDequeue(out var toast))
        {
            toasts.Add(toast);
        }
        return toasts;
    }
}