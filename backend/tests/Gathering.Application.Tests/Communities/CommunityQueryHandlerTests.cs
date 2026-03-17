using FluentAssertions;
using Gathering.Application.Communities.GetAll;
using Gathering.Application.Communities.GetById;
using Gathering.Application.Communities.Shared;
using Gathering.Domain.Communities;
using Gathering.SharedKernel;
using NSubstitute;

namespace Gathering.Application.Tests.Communities;

public class GetCommunitiesQueryHandlerTests
{
  private readonly ICommunityRepository _communityRepository = Substitute.For<ICommunityRepository>();
  private readonly GetCommunitiesQueryHandler _sut;

  public GetCommunitiesQueryHandlerTests()
  {
    _sut = new GetCommunitiesQueryHandler(_communityRepository);
  }

  [Fact]
  public async Task HandleAsync_WithCommunities_ShouldReturnMappedResponses()
  {
    // Arrange
    var community = Community.Create("Community 1", "A valid description for community 1.").Value;
    _communityRepository.GetAllAsync(Arg.Any<CancellationToken>())
        .Returns(new List<Community> { community });

    // Act
    var result = await _sut.HandleAsync(new GetCommunitiesQuery());

    // Assert
    result.IsSuccess.Should().BeTrue();
    result.Value.Should().ContainSingle();
    result.Value[0].Name.Should().Be("Community 1");
  }

  [Fact]
  public async Task HandleAsync_WhenNoCommunities_ShouldReturnEmptyList()
  {
    // Arrange
    _communityRepository.GetAllAsync(Arg.Any<CancellationToken>())
        .Returns(new List<Community>());

    // Act
    var result = await _sut.HandleAsync(new GetCommunitiesQuery());

    // Assert
    result.IsSuccess.Should().BeTrue();
    result.Value.Should().BeEmpty();
  }
}

public class GetCommunityByIdQueryHandlerTests
{
  private readonly ICommunityRepository _communityRepository = Substitute.For<ICommunityRepository>();
  private readonly GetCommunityByIdQueryHandler _sut;

  public GetCommunityByIdQueryHandlerTests()
  {
    _sut = new GetCommunityByIdQueryHandler(_communityRepository);
  }

  [Fact]
  public async Task HandleAsync_WhenCommunityExists_ShouldReturnMappedResponse()
  {
    // Arrange
    var community = Community.Create("My Community", "A valid description for my community.").Value;
    _communityRepository.GetByIdAsync(community.Id, Arg.Any<CancellationToken>())
        .Returns(community);

    // Act
    var result = await _sut.HandleAsync(new GetCommunityByIdQuery(community.Id));

    // Assert
    result.IsSuccess.Should().BeTrue();
    result.Value.Id.Should().Be(community.Id);
    result.Value.Name.Should().Be("My Community");
  }

  [Fact]
  public async Task HandleAsync_WhenCommunityNotFound_ShouldReturnFailure()
  {
    // Arrange
    _communityRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
        .Returns((Community?)null);

    // Act
    var result = await _sut.HandleAsync(new GetCommunityByIdQuery(Guid.NewGuid()));

    // Assert
    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(CommunityError.NotFound);
  }
}
