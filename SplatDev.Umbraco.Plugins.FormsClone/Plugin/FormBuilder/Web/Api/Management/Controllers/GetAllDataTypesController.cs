using FormBuilder.Core.Models;

using Microsoft.AspNetCore.Mvc;

using Umbraco.Cms.Core.Models;

using Umbraco.Cms.Core.Services;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API controller for retrieving all data types.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    [Route("/formBuilder/management/api/v1/picker/data-type")]
    public class GetAllDataTypesController(IDataTypeService dataTypeService) : PickerControllerBase
    {
        private readonly IDataTypeService _dataTypeService = dataTypeService;

        /// <summary>
        /// Management API controller for retrieving all document types.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PickerItem>), 200)]
        public async Task<IActionResult> GetAll()
        {
            GetAllDataTypesController dataTypesController = this;
            List<PickerItem>? list = [];
            foreach (IDataType dataType in await dataTypesController._dataTypeService.GetAllAsync())
                list.Add(new PickerItem()
                {
                    Id = dataType.Id.ToString(),
                    Value = dataType.Name ?? string.Empty
                });
            IActionResult all = dataTypesController.Ok(list.OrderBy(x => x.Value).ToList());
            list = null;
            return all;
        }
    }
}