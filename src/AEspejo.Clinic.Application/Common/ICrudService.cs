namespace AEspejo.Clinic.Application.Common;

/// <summary>Standard CRUD contract for manageable entities.</summary>
public interface ICrudService<TCreateDto, TUpdateDto, TResponseDto>
{
    Task<PagedResult<TResponseDto>> ListAsync(PagedRequest request, CancellationToken ct = default);
    Task<Result<TResponseDto>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<TResponseDto>> CreateAsync(TCreateDto dto, CancellationToken ct = default);
    Task<Result<TResponseDto>> UpdateAsync(Guid id, TUpdateDto dto, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken ct = default);
}
