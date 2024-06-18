using Cfo.Cats.Application.Features.Documents.DTOs;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Cfo.Cats.Application.Features.Participants.DTOs;

public class ConsentDto
{
    public DateTime ConsentDate { get; set; }
    public Guid? DocumentId { get; set; } 
    
    public string? FileName { get; set; }
}
