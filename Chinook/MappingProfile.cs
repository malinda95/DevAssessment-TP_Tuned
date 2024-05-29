using AutoMapper;
using Chinook.ClientModels;
using Chinook.Models;

namespace Chinook
{
    public class MappingProfile: Profile
    {
        public MappingProfile() 
        {
            CreateMap<Artist, ArtistViewModel>().ReverseMap();
            CreateMap<Album, AlbumViewModel>().ReverseMap();
        }
    }
}
