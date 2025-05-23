using ParkingSystem.Core.Aggregates;
using ParkingSystem.Application.DTOs;

namespace ParkingSystem.Application.Mappers
{
    public static class ParkMapper
    {
        public static Park ToParkDomain(ParkDTO dto)
        {
            return new Park
            {
                name = dto.name,
                location = dto.location,
                capacity = dto.capacity,
                gateOpen = dto.gateOpen
            };
        }

        public static ParkDTO ToParkDto(Park park)
        {
            return new ParkDTO
            {
                name = park.name,
                location = park.location,
                capacity = park.capacity,
                gateOpen = park.gateOpen
            };
        }
    }
}
