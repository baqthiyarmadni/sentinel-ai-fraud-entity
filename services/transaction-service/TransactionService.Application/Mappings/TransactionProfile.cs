using AutoMapper;
using TransactionService.Domain.Entities;
using TransactionService.Application.DTOs;

namespace TransactionService.Application.Mappings
{
    public class TransactionProfile : Profile
    {
        public TransactionProfile()
        {
            CreateMap<Transaction, TransactionResponse>()
                .ForMember(
                dest => dest.TransactionId,
                opt => opt.MapFrom(src => src.Id))
                .ForMember(
                dest => dest.Status,
                opt => opt.MapFrom(src => src.Status.ToString()));
        }
    }
}
