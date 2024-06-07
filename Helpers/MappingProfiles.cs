using AutoMapper;
using IBDb.Dto;
using IBDb.Models;

namespace PokemonReviewApp.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Book, BookDto>();
            CreateMap<User, PublisherDto>();
            CreateMap<Book, BookDto>()
                .ForMember(dest => dest.Publisher, opt => opt.MapFrom(src => src.Publisher));
            CreateMap<Book, BookCreateDto>();
            CreateMap<BookCreateDto, Book>()
                .ForMember(dest => dest.BookGenres, opt => opt.Ignore());
            CreateMap<BookUpdateDto,Book>();
            CreateMap<Book, BookUpdateDto>();

            CreateMap<User, UserCreateDto>();
            CreateMap<UserCreateDto, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.Ignore());
            CreateMap<User, UserSuccessfullLoginDto>();
            CreateMap<UserSuccessfullLoginDto, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.Ignore());
            CreateMap<User, UserListDto>();
            CreateMap<UserListDto, User>()
               .ForMember(dest => dest.Id, opt => opt.Ignore())
               .ForMember(dest => dest.Role, opt => opt.Ignore());

            CreateMap<RoleCreateDto, Role>();
            CreateMap<Role, RoleCreateDto>();
            CreateMap<RoleUpdateDto, Role>();
            CreateMap<Role, RoleUpdateDto>();

            CreateMap<GenreCreateDto, Genre>();
            CreateMap<Genre, GenreCreateDto>();

            CreateMap<ReviewDto, Review>();
            CreateMap<Review, ReviewDto>();
            CreateMap<Review, ReviewCreateDto>();
            CreateMap<ReviewCreateDto, Review>();
        }
    }
}
