using CommunityToolkit.Mvvm.Input;
using WuYanApps.Models;

namespace WuYanApps.PageModels
{
    public interface IProjectTaskPageModel
    {
        IAsyncRelayCommand<ProjectTask> NavigateToTaskCommand { get; }
        bool IsBusy { get; }
    }
}