﻿using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Business.Dtos;

public class AddProjectDto
{
    public IFormFile? ImageFile { get; set; }

    [Required]
    public string ProjectName { get; set; } = null!;

    [Required]
    public string ClientId { get; set; } = null!;

    public string? Description { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime EndDate { get; set; }

    public decimal? Budget { get; set; }

    [Required]
    public string UserId { get; set; } = null!;
}