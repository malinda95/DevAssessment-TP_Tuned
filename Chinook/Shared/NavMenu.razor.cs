using Chinook.ClientModels;
using Chinook.Domain.Contracts;
using Chinook.Domain.Utils;
using Microsoft.AspNetCore.Components;
using static Chinook.Domain.Constants;

namespace Chinook.Shared
{
    public partial class NavMenu
    {
        [Inject] IPlaylistService PlaylistService { get; set; }
        [Inject] CustomNotificationService CustomNotificationService { get; set; }
        private List<PlaylistViewModel> Playlists;
        private bool collapseNavMenu = true;

        private string? NavMenuCssClass => collapseNavMenu ? "collapse" : null;

        protected override async Task OnInitializedAsync()
        {
            CustomNotificationService.OnSubscribeEvent += onCustomNotification;
            Playlists = await PlaylistService.GetUserPlaylistsAsync();
        }

        private async Task onCustomNotification(string notificationEvent)
        {
            if (notificationEvent == CustomEvents.NewPlaylistAdded)
            {
                await InvokeAsync(StateHasChanged);
                Playlists = await PlaylistService.GetUserPlaylistsAsync();
            }
        }

        private void ToggleNavMenu()
        {
            collapseNavMenu = !collapseNavMenu;
        }

        public void Dispose()
        {
            CustomNotificationService.OnSubscribeEvent -= onCustomNotification;

        }
    }
}