namespace Vogelhochzeit.Models;

public record Photo(Guid Id = Guid.NewGuid(), string Url, string Category);
