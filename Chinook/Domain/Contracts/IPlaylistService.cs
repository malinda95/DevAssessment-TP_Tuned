using Chinook.ClientModels;

namespace Chinook.Domain.Contracts
{
    public interface IPlaylistService
    {
        Task AddFavoriteTrackByIdAsync(long trackId);
        Task RemoveFavoriteTrackByIdAsync(long trackId);
        Task<List<PlaylistViewModel>> GetUserPlaylistsAsync(bool includeFavorites = true);
        Task<OperationalResult> AddTrackToNewPlaylistAsync(string newPlaylistName, long trackId);
        Task AddTrackToExistingPlaylistAsync(long playlistId, long trackId);
        Task<PlaylistViewModel> GetPlaylistWithTracksByPlaylistIdAsync(long playlistId);
        Task<OperationalResult> RemoveTrackFromPlaylistAsync(long playlistId, long trackId);
    }
}