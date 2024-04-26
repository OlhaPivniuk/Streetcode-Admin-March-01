using FluentResults;
using MediatR;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources.Errors;
using Streetcode.DAL.Repositories.Interfaces.Base;
using FactEntity = Streetcode.DAL.Entities.Streetcode.TextContent.Fact;

namespace Streetcode.BLL.MediatR.Streetcode.Fact.Delete;

public class DeleteFactHandler : IRequestHandler<DeleteFactCommand, Result<Unit>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;

    public DeleteFactHandler(IRepositoryWrapper repositoryWrapper, ILoggerService logger)
    {
        _repositoryWrapper = repositoryWrapper;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(DeleteFactCommand request, CancellationToken cancellationToken)
    {
        int id = request.Id;
        var fact = await _repositoryWrapper.FactRepository.GetFirstOrDefaultAsync(x => x.Id == id);

        if (fact is null)
        {
            string errorMsg = string.Format(
                ErrorMessages.EntityByIdNotFound,
                typeof(FactEntity).Name,
                request.Id);

            _logger.LogError(request, errorMsg);

            return Result.Fail(errorMsg);
        }

        _repositoryWrapper.FactRepository.Delete(fact);

        var resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

        if (resultIsSuccess)
        {
            return Result.Ok(Unit.Value);
        }
        else
        {
            string errorMsg = string.Format(
                ErrorMessages.DeleteFailed,
                typeof(FactEntity).Name,
                request.Id);

            _logger.LogError(request, errorMsg);

            return Result.Fail(new Error(errorMsg));
        }
    }
}
