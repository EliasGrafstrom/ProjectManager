using Business.Dtos;
using Business.Models;
using Data.Entitites;
using Domain.Models;

namespace Business.Mappers;

public static class ProjectMapper
{
    public static ProjectEntity ToEntity(AddProjectDto? dto, string? newImageFileName = null)
    {
        if (dto == null) return null!;
        return new ProjectEntity
        {
            Image = newImageFileName,
            ProjectName = dto.ProjectName,
            ClientId = dto.ClientId,
            Description = dto.Description,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Budget = dto.Budget,
            UserId = dto.UserId,
        };
    }

    public static ProjectEntity ToEntity(UpdateProjectDto? dto, string? newImageFileName = null)
    {
        if (dto == null) return null!;
        return new ProjectEntity
        {
            Id = dto.Id,
            Image = newImageFileName ?? dto.ImageFileName,
            ProjectName = dto.ProjectName,
            ClientId = dto.ClientId,
            Description = dto.Description,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Budget = dto.Budget,
            UserId = dto.UserId,
            StatusId = dto.StatusId
        };
    }

    public static Project ToModel(ProjectEntity? entity)
    {
        if (entity == null) return null!;
        return new Project
        {
            Id = entity.Id,
            Image = entity.Image,
            ProjectName = entity.ProjectName,
            Client = ClientMapper.ToModel(entity.Client),
            Description = entity.Description,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            Budget = entity.Budget,
            User = UserMapper.ToModel(entity.User),
            Status = StatusMapper.ToModel(entity.Status),
        };
    }
}