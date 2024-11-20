using AutoMapper;
using PumpPalace.Models;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CustomerRegisterViewModel, Customer>();
    }
}
