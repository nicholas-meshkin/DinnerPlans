using Microsoft.AspNetCore.Components.Forms;
using DinnerPlans.Shared.DTOs;

namespace DinnerPlans.Client.Services.IServices
{
    public interface IFileReadService
    {
        /// <summary>
        /// get ingredients from uploaded file
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        Task<IList<RecipeIngredientDto>> GetIngredientsFromUpload(IBrowserFile file);
        /// <summary>
        ///  get instructions from uploaded file
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        Task<IList<InstructionDto>> GetInstructionsFromUpload(IBrowserFile file);
    }
}
