using AutoMapper;
using Chinook.ClientModels;
using Chinook.Models;
using Playlist = Chinook.Models.Playlist;

namespace Chinook
{
    public class MappingProfile: Profile
    {
        public MappingProfile() 
        {
            CreateMap<Artist, ArtistViewModel>().ReverseMap();
            CreateMap<Album, AlbumViewModel>().ReverseMap();
            CreateMap<Playlist, PlaylistViewModel>().ReverseMap();
        }
    }
}
