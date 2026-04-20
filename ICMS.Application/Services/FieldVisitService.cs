using ICMS.Application.DTOs.FieldVisit;
using ICMS.Application.DTOs.Pagination;
using ICMS.Application.Interfaces;
using ICMS.Application.Interfaces.Repositories;
using ICMS.Application.Interfaces.Services;
using ICMS.Application.Extensions;
using ICMS.Domain.Entites.Visits;
using ICMS.Domain.Exceptions;
using ICMS.Domain.ValueObjects;

namespace ICMS.Application.Services
{
    public class FieldVisitService : IFieldVisitService
    {
        private readonly IUnitOfWork _unitOfWork;

        public FieldVisitService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedResult<FieldVisitReadDto>> GetAllAsync(PaginationParams paginationParams, CancellationToken ct = default)
        {
            var query = _unitOfWork.FieldVisitRepository.GetQueryable()
                .Select(fv => fv.ToReadDto());

            return query.ApplyPagination(paginationParams.PageNumber, paginationParams.PageSize);
        }

        public async Task<FieldVisitDetailsDto> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var fieldVisit = await _unitOfWork.FieldVisitRepository.GetByIdWithDetailsAsync(id, ct);

            if (fieldVisit == null)
                throw new NotFoundException("NotFound");

            return fieldVisit.ToDetailsDto();
        }

        public async Task<FieldVisitReadDto> AddAsync(FieldVisitCreateDto dto, CancellationToken ct = default)
        {
            var fieldVisit = FieldVisit.Create(dto.VisitDate, dto.SubNeighborhoodId);

            await _unitOfWork.FieldVisitRepository.AddAsync(fieldVisit, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return fieldVisit.ToReadDto();
        }

        public async Task<bool> UpdateAsync(int id, FieldVisitCreateDto dto, CancellationToken ct = default)
        {
            var fieldVisit = await _unitOfWork.FieldVisitRepository.GetByIdAsync(id, ct);

            if (fieldVisit == null)
                throw new NotFoundException("NotFound");

            fieldVisit.UpdateVisitInfo(dto.VisitDate, dto.SubNeighborhoodId);

            return await _unitOfWork.SaveChangesAsync(ct) > 0;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var fieldVisit = await _unitOfWork.FieldVisitRepository.GetByIdAsync(id, ct);

            if (fieldVisit == null)
                throw new NotFoundException("NotFound");

            await _unitOfWork.FieldVisitRepository.DeleteAsync(fieldVisit, ct);

            return await _unitOfWork.SaveChangesAsync(ct) > 0;
        }

        public async Task<bool> MarkCompletedAsync(int id, CancellationToken ct = default)
        {
            var fieldVisit = await _unitOfWork.FieldVisitRepository.GetByIdAsync(id, ct);

            if (fieldVisit == null)
                throw new NotFoundException("NotFound");

            fieldVisit.MarkCompleted();

            return await _unitOfWork.SaveChangesAsync(ct) > 0;
        }
    }
}
