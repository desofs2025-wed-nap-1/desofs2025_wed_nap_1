namespace ParkingSystem.Application.DTOs
{
  public class InOutRecordDTO
  {
    public class InOutRecordRequestDto
    {
      public long Vehicle { get; set; }
      public long Park { get; set; }
    }

    public class InOutRecordResponseDto
    {
      public long Id { get; set; }
      public long Vehicle { get; set; }
      public long Park { get; set; }
      public DateTime DataEntrada { get; set; }
      public DateTime? DataSaida { get; set; }
    }

  }
}


