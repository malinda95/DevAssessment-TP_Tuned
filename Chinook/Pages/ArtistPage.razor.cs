using Chinook.ClientModels;
using Chinook.Domain.Contracts;
using Chinook.Domain.Utils;
using Chinook.Shared.Components;
using Microsoft.AspNetCore.Components;
using static Chinook.Domain.Constants;

namespace Chinook.Pages
{
    public partial class ArtistPage
    {
        [Parameter] public long ArtistId { get; set; }
        [Inject] IArtistService ArtistService { get; set; }
        [Inject] IPlaylistService PlaylistService { get; set; }
        [Inject] IAlbumTrackService AlbumTrackService { get; set; }
        [Inject] ILogger<ArtistPage> Logger { get; set; }
        [Inject] CustomNotificationService CustomNotificationService { get; set; }
        private Modal PlaylistDialog { get; set; }
        private long SelectedPlaylistId { get; set; }

        private ArtistViewModel artist;
        private List<PlaylistTrack> tracks;
        private List<PlaylistViewModel> existingPlaylists;
        private PlaylistTrack selectedTrack;
        private string message = "";
        private bool isError = false;
        private string newPlaylistName = "";

        protected override async Task OnInitializedAsync()
        {
            artist = await GetArtistById();
            tracks = await GetTracksByArtistId();
            existingPlaylists = await GetUserPlaylists();
        }

        private async Task<ArtistViewModel> GetArtistById()
        {
            try
            {
                return await ArtistService.GetArtistByIdAsync(ArtistId);
            }
            catch (Exception ex)
            {
                HandleError("An error occurred while fetching artist.", ex);
                return null;
            }
        }

        private async Task<List<PlaylistTrack>> GetTracksByArtistId()
        {
            try
            {
                return await AlbumTrackService.GetTracksByArtistIdAsync(ArtistId);
            }
            catch (Exception ex)
            {
                HandleError("An error occurred while fetching tracks.", ex);
                return new List<PlaylistTrack>();
            }
        }

        private async Task<List<PlaylistViewModel>> GetUserPlaylists()
        {
            try
            {
                return await PlaylistService.GetUserPlaylistsAsync(false);
            }
            catch (Exception ex)
            {
                HandleError("An error occurred while fetching user playlists.", ex);
                return new List<PlaylistViewModel>();
            }
        }

        private async Task FavoriteTrack(long trackId)
        {
            try
            {
                isError = false;
                message = "";
                var track = tracks.FirstOrDefault(t => t.TrackId == trackId);
                await PlaylistService.AddFavoriteTrackByIdAsync(trackId);
                track.IsFavorite = true;
                await CustomNotificationService.NotifyEventAsync(CustomEvents.NewPlaylistAdded);

                message = $"Track {track.ArtistName} - {track.AlbumTitle} - {track.TrackName} added to {DefaultPlaylistNames.Favorites}.";
            }
            catch (Exception ex)
            {
                HandleError("An error occurred while marking the track as favorite.", ex);
            }
        }

        private async Task UnfavoriteTrack(long trackId)
        {
            try
            {
                isError = false;
                message = "";
                var track = tracks.FirstOrDefault(t => t.TrackId == trackId);
                await PlaylistService.RemoveFavoriteTrackByIdAsync(trackId);
                track.IsFavorite = false;

                message = $"Track {track.ArtistName} - {track.AlbumTitle} - {track.TrackName} removed from {DefaultPlaylistNames.Favorites}.";
            }
            catch (Exception ex)
            {
                HandleError("An error occurred while unmarking the track as favorite.", ex);
            }
        }

        private void OpenPlaylistDialog(long trackId)
        {
            try
            {
                SelectedPlaylistId = 0;
                isError = false;
                message = "";
                selectedTrack = tracks.FirstOrDefault(t => t.TrackId == trackId);
                newPlaylistName = "";
                PlaylistDialog.Open();
            }
            catch (Exception ex)
            {
                HandleError("An error occurred while opening the playlist dialog.", ex);
            }
        }

        private async Task AddTrackToPlaylist()
        {
            try
            {
                if (!string.IsNullOrEmpty(newPlaylistName))
                {
                    var result = await PlaylistService.AddTrackToNewPlaylistAsync(newPlaylistName, selectedTrack.TrackId);
                    if (result.IsSuccess)
                    {
                        message = $"Track {artist.Name} - {selectedTrack.AlbumTitle} - {selectedTrack.TrackName} added to playlist {newPlaylistName}.";
                        existingPlaylists = await PlaylistService.GetUserPlaylistsAsync(false);
                        await CustomNotificationService.NotifyEventAsync(CustomEvents.NewPlaylistAdded);
                    }
                    else
                    {
                        isError = true;
                        message = result.Message;
                    }
                }
                else if (SelectedPlaylistId > 0)
                {
                    await PlaylistService.AddTrackToExistingPlaylistAsync(SelectedPlaylistId, selectedTrack.TrackId);
                    var selectedExistingPlaylistName = existingPlaylists.Single(pl => pl.PlaylistId == SelectedPlaylistId).Name;
                    message = $"Track {artist.Name} - {selectedTrack.AlbumTitle} - {selectedTrack.TrackName} added to playlist {selectedExistingPlaylistName}.";
                }
                PlaylistDialog.Close();
            }
            catch (Exception ex)
            {
                HandleError("An error occurred while adding the track to the playlist.", ex);
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