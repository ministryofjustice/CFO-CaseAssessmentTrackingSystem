using Newtonsoft.Json;

namespace Cfo.Cats.Application.Features.Bio.DTOs;

public abstract class PathwayBase
{
    [JsonIgnore]
    public abstract string Title { get; }

    [JsonIgnore]
    public abstract string Icon { get; }

    public abstract IEnumerable<QuestionBase> Questions();
    
}