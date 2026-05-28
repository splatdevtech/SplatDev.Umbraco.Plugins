using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using FormBuilder.Extension.Controllers;
using FormBuilder.Extension.Entities;
using FormBuilder.Extension.Interfaces;

namespace FormBuilder.Extension.Tests;

public class FormBuilderApiControllerTests
{
    private readonly Mock<IFormRepository> _repo;
    private readonly FormBuilderApiController _sut;

    public FormBuilderApiControllerTests()
    {
        _repo = new Mock<IFormRepository>();
        _sut = new FormBuilderApiController(_repo.Object);
    }

    private static Form MakeForm(int id, string name) => new()
    {
        Id = id, Name = name, Category = string.Empty,
        CreatedDate = DateTime.UtcNow, UpdatedDate = DateTime.UtcNow,
        Fields = [], Workflows = [],
    };

    [Fact]
    public async Task GetAllForms_ReturnsOk()
    {
        _repo.Setup(r => r.GetAllAsync()).ReturnsAsync([MakeForm(1, "F1")]);

        var result = await _sut.GetAllForms();

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetForm_Valid_ReturnsOk()
    {
        _repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(MakeForm(1, "F1"));

        var result = await _sut.GetForm(1);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetForm_InvalidId_ReturnsBadRequest()
    {
        var result = await _sut.GetForm(0);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task GetForm_NotFound_ReturnsNotFound()
    {
        _repo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Form?)null);

        var result = await _sut.GetForm(99);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task CreateForm_Valid_ReturnsOk()
    {
        _repo.Setup(r => r.CreateAsync(It.IsAny<Form>())).ReturnsAsync(MakeForm(1, "F1"));

        var result = await _sut.CreateForm(new CreateFormRequest("F1", null, null));

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task CreateForm_EmptyName_ReturnsBadRequest()
    {
        var result = await _sut.CreateForm(new CreateFormRequest("", null, null));

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task CreateForm_WithFields_ReturnsOk()
    {
        _repo.Setup(r => r.CreateAsync(It.IsAny<Form>())).ReturnsAsync(MakeForm(2, "F2"));
        var fields = new List<CreateFormFieldRequest>
        {
            new("email", "Email", "email", true, "you@example.com", null, null, 0),
            new("name", "Name", "text", true, null, null, 3, 1),
        };

        var result = await _sut.CreateForm(new CreateFormRequest("F2", null, fields));

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task UpdateForm_Valid_ReturnsOk()
    {
        _repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(MakeForm(1, "Old"));
        _repo.Setup(r => r.UpdateAsync(It.IsAny<Form>())).ReturnsAsync(MakeForm(1, "New"));

        var result = await _sut.UpdateForm(1, new CreateFormRequest("New", null, null));

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task UpdateForm_NotFound_ReturnsNotFound()
    {
        _repo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Form?)null);

        var result = await _sut.UpdateForm(99, new CreateFormRequest("X", null, null));

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task UpdateForm_EmptyName_ReturnsBadRequest()
    {
        _repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(MakeForm(1, "Old"));

        var result = await _sut.UpdateForm(1, new CreateFormRequest("", null, null));

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task DeleteForm_Valid_ReturnsOk()
    {
        _repo.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);

        var result = await _sut.DeleteForm(1);

        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task DeleteForm_HasSubmissions_ReturnsBadRequest()
    {
        _repo.Setup(r => r.DeleteAsync(1)).ThrowsAsync(new InvalidOperationException("has submissions"));

        var result = await _sut.DeleteForm(1);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task DeleteForm_InvalidId_ReturnsBadRequest()
    {
        var result = await _sut.DeleteForm(0);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task DuplicateForm_Valid_ReturnsOk()
    {
        _repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(MakeForm(1, "F1"));
        _repo.Setup(r => r.CreateAsync(It.IsAny<Form>())).ReturnsAsync(MakeForm(2, "F1 (copy)"));

        var result = await _sut.DuplicateForm(1);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task DuplicateForm_NotFound_ReturnsNotFound()
    {
        _repo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Form?)null);

        var result = await _sut.DuplicateForm(99);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public void GetFieldTypes_ReturnsOkWithList()
    {
        var result = _sut.GetFieldTypes();

        Assert.IsType<OkObjectResult>(result);
    }
}
