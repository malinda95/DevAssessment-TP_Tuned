using Chinook.ClientModels;
using Chinook.Domain.Contracts;
using Microsoft.AspNetCore.Components;
using static Chinook.Domain.Constants;

namespace Chinook.Pages
{
    public partial class PlaylistPage
    {
        [Parameter] public long PlaylistId { get; set; }
        [Inject] IPlaylistService PlaylistService { get; set; }
        [Inject] ILogger<PlaylistPage> Logger { get; set; }

        private PlaylistViewModel playlist;
        private string message;
        private bool isError = false;

        protected override async Task OnInitializedAsync()
        {
            await InitializePageAsync();
        }

        protected override async Task OnParametersSetAsync()
        {
            await InitializePageAsync();
        }
        private async Task InitializePageAsync()
        {
            message = "";
            await GetPlaylistWithTracks();
        }

        private async Task UnfavoriteTrack(long trackId)
        {
            try
            {
                var track = GetTrackFromPlaylist(trackId);
                await PlaylistService.RemoveFavoriteTrackByIdAsync(trackId);
                playlist.Tracks.Remove(track);
                message = $"Track {track.ArtistName} - {track.AlbumTitle} - {track.TrackName} removed from {DefaultPlaylistNames.Favorites}.";
            }
            catch (Exception ex)
            {
                HandleError($"An error occurred while unfavoriting the track.", ex);
            }
        }

        private async Task RemoveTrack(long trackId)
        {
            try
            {
                var result = await PlaylistService.RemoveTrackFromPlaylistAsync(PlaylistId, trackId);
                if (result.IsSuccess)
                {
                    var track = GetTrackFromPlaylist(trackId);
                    playlist.Tracks.Remove(track);
                    message = $"Track {track.ArtistName} - {track.AlbumTitle} - {track.TrackName} removed from {playlist.Name}.";
                }
            }
            catch (Exception ex)
            {

                HandleError($"An error occurred while removing the track.", ex);
            }
        }

        private PlaylistTrack GetTrackFromPlaylist(long trackId)
        {
            return playlist.Tracks.Single(t => t.TrackId == trackId);
        }

        private async Task GetPlaylistWithTracks()
        {
            try
            {
                playlist = await PlaylistService.GetPlaylistWithTracksByPlaylistIdAsync(PlaylistId);
            }
            catch (Exception ex)
            {
                HandleError("An error occurred while fetching the playlist with tracks.", ex);
            }
        }

        private void HandleError(string errorMessage, Exception ex)
        {
            isError = true;
            message = errorMessage;
            Logger.LogError(ex, errorMessage);
        }
    }
}