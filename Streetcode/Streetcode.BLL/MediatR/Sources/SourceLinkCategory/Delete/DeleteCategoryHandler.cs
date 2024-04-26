using FluentResults;
using MediatR;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources.Errors;
using Streetcode.DAL.Repositories.Interfaces.Base;
using SourceLinkCategoryEntity = Streetcode.DAL.Entities.Sources.SourceLinkCategory;

namespace Streetcode.BLL.MediatR.Sources.SourceLinkCategory.Delete;

public class DeleteCategoryHandler : IRequestHandler<DeleteCategoryCommand, Result<Unit>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;

    public DeleteCategoryHandler(IRepositoryWrapper repositoryWrapper, ILoggerService logger)
    {
        _repositoryWrapper = repositoryWrapper;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        int id = request.Id;

        var category = await _repositoryWrapper.SourceCategoryRepository.GetFirstOrDefaultAsync(x => x.Id == id);

        if (category is null)
        {
            string errorMsg = string.Format(
                ErrorMessages.EntityByIdNotFound,
                typeof(SourceLinkCategoryEntity).Name,
                request.Id);

            _logger.LogError(request, errorMsg);

            return Result.Fail(errorMsg);
        }

        _repositoryWrapper.SourceCategoryRepository.Delete(category !);

        var isSuccessful = await _repositoryWrapper.SaveChangesAsync() > 0;

        if (! isSuccessful)
        {
            var errorMessage = string.Format(
                ErrorMessages.DeleteFailed,
                typeof(SourceLinkCategoryEntity).Name,
                request.Id);

            _logger.LogError(request, errorMessage);

            return Result.Fail(new Error(errorMessage));
        }

        return Result.Ok(Unit.Value);
    }
}
