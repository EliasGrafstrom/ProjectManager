using Business.Dtos;
using Business.Services;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Presentation.Models;
using System.Security.Claims;

namespace Presentation.Controllers;

[Authorize]

public class ProjectsController(IStatusService statusService, IClientService clientService, IProjectService projectService) : Controller
{
    private readonly IStatusService _statusService = statusService;
    private readonly IClientService _clientService = clientService;
    private readonly IProjectService _projectService = projectService;

    [Route("admin/projects")]
    public async Task<IActionResult> Index()
    {
        var clients = await GetClientsSelectListAsync();
        var statuses = await GetStatusesSelectListAsync();
        var projects = await GetProjectsAsync();

        var vm = new ProjectsViewModel
        {
            Projects = projects,
            AddProjectViewModel = new AddProjectViewModel() { Clients = clients },
            EditProjectViewModel = new EditProjectViewModel() { Clients = clients, Statuses = statuses },
        };

        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Add(AddProjectViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var clientsResult = await _clientService.GetClientsAsync();
            if (clientsResult.Succeeded)
            {
                model.Clients = clientsResult.Result.Select(client => new SelectListItem
                {
                    Value = client.Id,
                    Text = client.ClientName
                });
            }

            return View(model);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var dto = new AddProjectDto
        {
            ImageFile = model.Image,
            ProjectName = model.ProjectName,
            ClientId = model.ClientId,
            Description = model.Description,
            StartDate = model.StartDate ?? DateTime.MinValue,
            EndDate = model.EndDate ?? DateTime.MinValue,
            Budget = model.Budget,
            UserId = userId
        };

        var project = await _projectService.CreateProjectAsync(dto);

        if (project == null)
        {
            ModelState.AddModelError(string.Empty, "Could not create the project. Try again.");

            var clientsResult = await _clientService.GetClientsAsync();
            if (clientsResult.Succeeded)
            {
                model.Clients = clientsResult.Result.Select(client => new SelectListItem
                {
                    Value = client.Id,
                    Text = client.ClientName
                });
            }

            return View(model);
        }

        return RedirectToAction("Index");
    }


    [HttpGet]
    public async Task<IActionResult> Add()
    {
        var vm = new AddProjectViewModel
        {
            Clients = await GetClientsSelectListAsync(),
        };
        return PartialView("~/Views/Shared/Partials/Project/_AddProjectModal.cshtml", vm);
    }



    [HttpGet("edit")]
    public async Task<IActionResult> Edit(string id)
    {
        var projectResult = await _projectService.GetProjectByIdAsync(id);
        if (projectResult == null)
            return NotFound();

        var project = projectResult;

        var vm = new EditProjectViewModel
        {
            Id = project.Id,
            ImageUrl = project.Image,
            ProjectName = project.ProjectName,
            Description = project.Description,
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            Budget = project.Budget,
            ClientId = project.Client.Id,
            StatusId = project.Status.Id,

            Clients = await GetClientsSelectListAsync(),
            Statuses = await GetStatusesSelectListAsync(),
        };

        return PartialView("~/Views/Shared/Partials/Project/_EditProjectModal.cshtml", vm);
    }


    private async Task<IEnumerable<SelectListItem>> GetClientsSelectListAsync()
    {
        var result = await _clientService.GetClientsAsync();
        var statusList = result.Result?.Select(s => new SelectListItem
        {
            Value = s.Id,
            Text = s.ClientName,
        });

        return statusList!;
    }

    private async Task<IEnumerable<SelectListItem>> GetStatusesSelectListAsync()
    {
        var result = await _statusService.GetStatusesAsync();
        var statusList = result.Result?.Select(s => new SelectListItem
        {
            Value = s.Id,
            Text = s.StatusName
        });

        return statusList!;
    }

    private async Task<IEnumerable<Project>> GetProjectsAsync()
    {
        IEnumerable<Project> projects = Enumerable.Empty<Project>();
        try
        {
            var projectResult = await _projectService.GetProjectsAsync();
            if (projectResult != null && projectResult.Any())
                projects = projectResult;
        }
        catch (Exception ex)
        {
            projects = Enumerable.Empty<Project>();
        }

        return projects;
    }


}