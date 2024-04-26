using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.Dto.Toponyms;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Entities.Toponyms;
using Streetcode.BLL.Interfaces.Logging;

namespace Streetcode.BLL.MediatR.Toponyms.GetAll;

public class GetAllToponymsHandler : IRequestHandler<GetAllToponymsQuery,
    Result<GetAllToponymsResponseDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetAllToponymsHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
    }

    public Task<Result<GetAllToponymsResponseDto>> Handle(GetAllToponymsQuery query, CancellationToken cancellationToken)
    {
        var filterRequest = query.request;

        var toponyms = _repositoryWrapper.ToponymRepository
             .FindAll();

        if (filterRequest.Title is not null)
        {
            FindStreetcodesWithMatchTitle(ref toponyms, filterRequest.Title);
        }

        var toponymDtos = _mapper.Map<IEnumerable<ToponymDto>>(toponyms.AsEnumerable());

        var response = new GetAllToponymsResponseDto
        {
            Pages = 1,
            Toponyms = toponymDtos
        };

        return Task.FromResult(Result.Ok(response));
    }

    private void FindStreetcodesWithMatchTitle(
        ref IQueryable<Toponym> toponyms,
        string title)
    {
        toponyms = toponyms.Where(s => s.StreetName
            .ToLower()
            .Contains(title
            .ToLower()))
            .GroupBy(s => s.StreetName)
            .Select(g => g.First());
    }
}