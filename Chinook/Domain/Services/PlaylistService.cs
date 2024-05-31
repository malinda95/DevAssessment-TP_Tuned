using AutoMapper;
using Chinook.ClientModels;
using Chinook.Domain.Contracts;
using Chinook.Models;
using Microsoft.EntityFrameworkCore;
using static Chinook.Domain.Constants;

namespace Chinook.Domain.Services
{
    public class PlaylistService : IPlaylistService
    {
        private readonly IDbContextFactory<ChinookContext> dbFactory;
        private readonly IMapper mapper;
        private readonly IUserService userService;

        public PlaylistService(
            IDbContextFactory<ChinookContext> dbFactory, 
            IMapper mapper, 
            IUserService userService)
        {
            this.dbFactory = dbFactory;
            this.mapper = mapper;
            this.userService = userService;
        }

        public async Task AddFavoriteTrackByIdAsync(long trackId)
        {
            var result = await ValidateAddNewPlayListAsync(DefaultPlaylistNames.Favorites);
            if (result.IsSuccess)
            {
                await CreateNewPlayListAsync(DefaultPlaylistNames.Favorites, trackId);
            }
            else
            {
                await UpdatePlaylistTracksAsync(trackId, playlistName: DefaultPlaylistNames.Favorites);
            }
        }

        public async Task RemoveFavoriteTrackByIdAsync(long trackId)
        {
            await UpdatePlaylistTracksAsync(trackId, isAdding: false, playlistName: DefaultPlaylistNames.Favorites);
        }

        public async Task<List<PlaylistViewModel>> GetUserPlaylistsAsync(bool includeFavorites = true)
        {
            string currentUserId = await userService.GetAuthenticatedUserIdAsync();
            using (var dbContext = await dbFactory.CreateDbContextAsync())
            {
                var userPlaylists = dbContext.UserPlaylists
                    .Include(up => up.Playlist)
                    .Where(up => up.UserId == currentUserId && (includeFavorites || up.Playlist.Name != DefaultPlaylistNames.Favorites))
                    .Select(up => up.Playlist)
                    .ToList();
                return mapper.Map<List<Playlist>, List<PlaylistViewModel>>(userPlaylists);
            }
        }

        public async Task<OperationalResult> AddTrackToNewPlaylistAsync(string newPlaylistName, long trackId)
        {
            var result = await ValidateAddNewPlayListAsync(newPlaylistName);
            if (result.IsSuccess)
            {
                await CreateNewPlayListAsync(newPlaylistName, trackId);
            }
            return result;
        }

        public async Task AddTrackToExistingPlaylistAsync(long playlistId, long trackId)
        {
            await UpdatePlaylistTracksAsync(trackId, playlistId: playlistId);
        }

        public async Task<PlaylistViewModel> GetPlaylistWithTracksByPlaylistIdAsync(long playlistId)
        {
            string currentUserId = await userService.GetAuthenticatedUserIdAsync();
            using (var dbContext = await dbFactory.CreateDbContextAsync())
            {
                if (!dbContext.Playlists.Include(pl => pl.UserPlaylists).Where(pl => pl.PlaylistId == playlistId && pl.UserPlaylists.Any(up => up.UserId == currentUserId)).Any())
                {
                    throw new InvalidOperationException("You are not allowed to view this playlist.");
                }
                var playlistWithTracks = dbContext.Playlists
                    .Include(a => a.Tracks)
                    .ThenInclude(a => a.Album)
                    .ThenInclude(a => a.Artist)
                    .Where(p => p.PlaylistId == playlistId)
                    .Select(p => new PlaylistViewModel()
                    {
                        Name = p.Name,
                        Tracks = p.Tracks.Select(t => new PlaylistTrack()
                        {
                            AlbumTitle = t.Album.Title,
                            ArtistName = t.Album.Artist.Name,
                            TrackId = t.TrackId,
                            TrackName = t.Name,
                            IsFavorite = t.Playlists.Where(p => p.UserPlaylists.Any(up => up.UserId == currentUserId && up.Playlist.Name == DefaultPlaylistNames.Favorites)).Any()
                        }).ToList()
                    })
                    .FirstOrDefault();
                return playlistWithTracks;
            }
        }

        public async Task<OperationalResult> RemoveTrackFromPlaylistAsync(long playlistId, long trackId)
        {
            await UpdatePlaylistTracksAsync(trackId, isAdding: false, playlistId: playlistId);
            return new OperationalResult { IsSuccess = true };
        }

        private async Task UpdatePlaylistTracksAsync(long trackId, bool isAdding = true, long playlistId = 0, string? playlistName = null)
        {
            string currentUserId = await userService.GetAuthenticatedUserIdAsync();
            using (var dbContext = await dbFactory.CreateDbContextAsync())
            {
                var playlist = playlistId > 0 ? dbContext.Playlists.Include(pl => pl.Tracks).SingleOrDefault(p => p.PlaylistId == playlistId) :
                    dbContext.UserPlaylists.Include(up => up.Playlist).ThenInclude(pl => pl.Tracks).FirstOrDefault(up => up.UserId == currentUserId && up.Playlist.Name == playlistName)?.Playlist;
                var track = dbContext.Tracks.SingleOrDefault(t => t.TrackId == trackId);
                if (playlist != null && track != null)
                {
                    if (isAdding)
                    {
                        playlist.Tracks.Add(track);
                    }
                    else
                    {
                        playlist.Tracks.Remove(track);
                    }
                    await dbContext.SaveChangesAsync();
                }
            }
        }

        private async Task CreateNewPlayListAsync(string newPlaylistName, long trackId)
        {
            string currentUserId = await userService.GetAuthenticatedUserIdAsync();
            using (var dbContext = await dbFactory.CreateDbContextAsync())
            {
                var track = dbContext.Tracks.SingleOrDefault(t => t.TrackId == trackId);
                if (track != null) 
                {
                    var newUserPlaylist = new UserPlaylist
                    {
                        UserId = currentUserId,
                        Playlist = new Playlist
                        {
                            Name = newPlaylistName,
                            Tracks = new List<Track>
                            {
                                track
                            }
                        }
                    };
                    dbContext.UserPlaylists.Add(newUserPlaylist);
                    await dbContext.SaveChangesAsync();
                }
            }
        }

        private async Task<OperationalResult> ValidateAddNewPlayListAsync(string newPlaylistName)
        {
            var result = new OperationalResult { IsSuccess = true };
            string currentUserId = await userService.GetAuthenticatedUserIdAsync();
            using (var dbContext = await dbFactory.CreateDbContextAsync())
            {
                var isSamePlaylistAvailable = dbContext.Playlists
                    .Any(pl => pl.UserPlaylists.Any(up => up.UserId == currentUserId && up.Playlist.Name == newPlaylistName));

                if (isSamePlaylistAvailable)
                {
                    result.IsSuccess = false;
                    result.Message = $"{newPlaylistName} playlist is already existing.";
                }
                return result;
            }
        }
    }
}
