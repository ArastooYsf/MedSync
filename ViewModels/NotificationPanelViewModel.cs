using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MedSync.Models;
using MedSync.Services;

namespace MedSync.ViewModels;

public partial class NotificationPanelViewModel : ViewModelBase
{
    private readonly NotificationService _notificationService;

    [ObservableProperty]
    private ObservableCollection<NotificationItemViewModel> _notifications = new();

    [ObservableProperty]
    private int _unreadCount;

    [ObservableProperty]
    private bool _isLoading;

    public NotificationPanelViewModel(NotificationService notificationService)
    {
        _notificationService = notificationService;
        
        // Subscribe to new notifications
        _notificationService.OnNotificationCreated += OnNewNotification;
        
        _ = LoadNotificationsAsync();
    }

    private void OnNewNotification(Notification notification)
    {
        var item = new NotificationItemViewModel(notification, _notificationService);
        item.ReadStatusChanged += UpdateUnreadCount;
        item.DeleteRequested += RemoveNotification;
        
        Notifications.Insert(0, item);
        UpdateUnreadCount();
    }

    public async Task LoadNotificationsAsync()
    {
        IsLoading = true;
        try
        {
            var notifications = await _notificationService.GetAllNotificationsAsync();
            
            Notifications.Clear();
            foreach (var notification in notifications)
            {
                var item = new NotificationItemViewModel(notification, _notificationService);
                item.ReadStatusChanged += UpdateUnreadCount;
                item.DeleteRequested += RemoveNotification;
                Notifications.Add(item);
            }
            
            UpdateUnreadCount();
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void RemoveNotification(NotificationItemViewModel item)
    {
        Notifications.Remove(item);
        UpdateUnreadCount();
    }

    [RelayCommand]
    private async Task MarkAllAsReadAsync()
    {
        await _notificationService.MarkAllAsReadAsync();
        
        foreach (var notification in Notifications)
        {
            notification.IsRead = true;
        }
        
        UpdateUnreadCount();
    }

    [RelayCommand]
    private async Task ClearAllAsync()
    {
        await _notificationService.ClearAllNotificationsAsync();
        Notifications.Clear();
        UpdateUnreadCount();
    }

    private void UpdateUnreadCount()
    {
        UnreadCount = Notifications.Count(n => !n.IsRead);
    }
}

public partial class NotificationItemViewModel : ObservableObject
{
    private readonly Notification _notification;
    private readonly NotificationService _notificationService;

    public event Action? ReadStatusChanged;
    public event Action<NotificationItemViewModel>? DeleteRequested;

    [ObservableProperty]
    private bool _isRead;

    public int Id => _notification.Id;
    public string Title => _notification.Title;
    public string Message => _notification.Message;
    public NotificationType Type => _notification.Type;
    public DateTime CreatedAt => _notification.CreatedAt;
    public NotificationPriority Priority => _notification.Priority;

    public string TimeAgo
    {
        get
        {
            var diff = DateTime.Now - CreatedAt;
            
            if (diff.TotalMinutes < 1)
                return "الان";
            if (diff.TotalMinutes < 60)
                return $"{(int)diff.TotalMinutes} دقیقه پیش";
            if (diff.TotalHours < 24)
                return $"{(int)diff.TotalHours} ساعت پیش";
            if (diff.TotalDays < 7)
                return $"{(int)diff.TotalDays} روز پیش";
            
            return CreatedAt.ToString("yyyy/MM/dd");
        }
    }

    public string TypeIcon => Type switch
    {
        NotificationType.AppointmentCreated => "\ue186",
        NotificationType.AppointmentUpdated => "\ue19e",
        NotificationType.AppointmentCancelled => "\ue10c",
        NotificationType.AppointmentReminder => "\ued2c",
        NotificationType.AppointmentMissed => "\ue006",
        NotificationType.AppointmentEmergency => "\ue56e",
        NotificationType.System => "\uee44",
        _ => "\ue0d0"
    };

    public string TypeColor => Type switch
    {
        NotificationType.AppointmentCreated => "#1FC198",
        NotificationType.AppointmentUpdated => "#0069FF",
        NotificationType.AppointmentCancelled => "#FF5271",
        NotificationType.AppointmentReminder => "#EDD851",
        NotificationType.AppointmentMissed => "#FF5271",
        NotificationType.AppointmentEmergency => "#FF5271",
        NotificationType.System => "#0069FF",
        _ => "#0069FF"
    };

    public string ReadIcon => IsRead ? "\ue182" : "\ue53a";
    public string ReadTooltip => IsRead ? "علامت به عنوان خوانده نشده" : "علامت به عنوان خوانده شده";

    public NotificationItemViewModel(Notification notification, NotificationService notificationService)
    {
        _notification = notification;
        _notificationService = notificationService;
        _isRead = notification.IsRead;
    }

    [RelayCommand]
    private async Task ToggleReadStatusAsync()
    {
        IsRead = !IsRead;
        
        if (IsRead)
        {
            await _notificationService.MarkAsReadAsync(Id);
        }
        else
        {
            // Update in database to unread
            _notification.IsRead = false;
            await Task.CompletedTask; // Add actual update logic if needed
        }
        
        OnPropertyChanged(nameof(ReadIcon));
        OnPropertyChanged(nameof(ReadTooltip));
        ReadStatusChanged?.Invoke();
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        await _notificationService.DeleteNotificationAsync(Id);
        DeleteRequested?.Invoke(this);
    }
}
