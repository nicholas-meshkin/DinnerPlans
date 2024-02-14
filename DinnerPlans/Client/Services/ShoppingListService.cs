//using Microsoft.IdentityModel.Tokens;
using DinnerPlans.Shared.DTOs;
//using Swashbuckle.AspNetCore.SwaggerGen;
using System.Globalization;
using DinnerPlans.Client.Services.IServices;

namespace DinnerPlans.Client.Services
{
    public class ShoppingListService : IShoppingListService
    {
        public ShoppingListService() { }

        //TODO change FE so this is used
        private bool MetricPreferred { get; set; } = false;
        private ShoppingListDto ShoppingList { get; set;} = new ShoppingListDto();

        private IList<RecipeViewIngredientListItemDto> ShoppingIngredientList { get; set; } = new List<RecipeViewIngredientListItemDto>();

        private Dictionary<string,KrogerProductSearchResponseDto> ProductLists { get; set; } = new Dictionary<string,KrogerProductSearchResponseDto>();
        private Dictionary<string, KrogerProductSearchResponseDto.Datum> SelectedProducts { get; set; } = new Dictionary<string, KrogerProductSearchResponseDto.Datum>();

        private string SelectedStore { get; set; } = "";

        public void SetIngredientList(List<RecipeViewIngredientListItemDto> list)
        {
            ShoppingIngredientList = list;
        }

        public List<RecipeViewIngredientListItemDto> GetIngredientList()
        {
            return ShoppingIngredientList.ToList();
        }

        public void AddToProductList(string query, KrogerProductSearchResponseDto dto)
        {
            ProductLists.Add(query, dto);
        }

        public void RemoveFromProductList(string query)
        {
            ProductLists.Remove(query);
        }

        public KrogerProductSearchResponseDto? GetFromProductList(string query)
        {
            return ProductLists.ContainsKey(query) ? ProductLists.GetValueOrDefault(query) : null;
        }

        public int GetProductResultCount(string query)
        {
            return ProductLists.ContainsKey(query) ? ProductLists.GetValueOrDefault(query).data.Length : 0;
        }

        public Dictionary<string, KrogerProductSearchResponseDto> GetProductList()
        {
            return ProductLists;
        }

        //TODO implement
        public void AddToSelectedProducts(string query, KrogerProductSearchResponseDto.Datum dto)
        {
            SelectedProducts.Add(query, dto);
        }

        //TODO implement
        public void ClearSelectedProducts()
        {
            SelectedProducts.Clear();
        }

        public void SelectStore(string store)
        {
            SelectedStore = store;
        }

        public string GetStore()
        {
            return SelectedStore;
        }

        public void AddToList(int recipeId, string name, int servings)
        {
            ShoppingList.Items.Add(new ShoppingListDto.ShoppingItemDto() { RecipeId = recipeId,RecipeName = name, Servings = servings });
            this.ItemsChanged?.Invoke(this, EventArgs.Empty);
        }

        public ShoppingListDto GetList()
        {
           return ShoppingList;
        }

        //TODO implement
        public bool GetMetricPref()
        {
            return MetricPreferred;
        }

        //TODO implement
        public void ToggleMetric()
        {
            MetricPreferred = !MetricPreferred;
            this.ItemsChanged?.Invoke(this, EventArgs.Empty);
        }

        //TODO implement
        public void RemoveRecipeFromList(int index) 
        {
            ShoppingList.Items.RemoveAt(index);
            this.ItemsChanged?.Invoke(this, EventArgs.Empty);
        }

        //TODO implement
        public void Clear()
        {
            ShoppingList.Items.Clear();
            this.ItemsChanged?.Invoke(this, EventArgs.Empty);
        }

        public int GetCount()
        {
            return ShoppingList.Items == null ? 0 : ShoppingList.Items.Count;
        }

        public event EventHandler? ItemsChanged;

        //need to remove units before using this
        public decimal? ParseAmount(string line)
        {
            try
            {
                if (!line.Any(c => char.IsDigit(c) || CharUnicodeInfo.GetNumericValue(c) != -1)) return 0;
                string numbers = new string(line.Where(c => char.IsDigit(c) || c.Equals('.') || CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.OtherNumber).ToArray());
                decimal number = 0;

                if (numbers.Any(a => CharUnicodeInfo.GetUnicodeCategory(a) == UnicodeCategory.OtherNumber))
                {
                    string newNumbers = "";
                    foreach (char d in numbers)
                    {
                        if (CharUnicodeInfo.GetUnicodeCategory(d) == UnicodeCategory.OtherNumber)
                            newNumbers += CharUnicodeInfo.GetNumericValue(d);
                        else newNumbers += d;
                    }
                    number = decimal.Parse(newNumbers);
                }
                else
                {
                    number = decimal.Parse(numbers);
                }

                return number;
            }
            catch (Exception ex)
            {
                return null;
            }
           

        }

        public string ParseUnit(string line)
        {
                switch (line)
                {
                    case var _ when line.ToUpper().Contains("FL. OZ."):
                        return "FL. OZ.";
                    case var _ when line.ToUpper().Contains("FL OZ"):
                        return "FL OZ";
                    case var _ when line.ToUpper().Contains("FLUID OUNCES"):
                        return "FLUID OUNCES";
                    case var _ when line.ToUpper().Contains("FLUID OUNCE"):
                        return "FLUID OUNCE";
                    case var _ when line.ToUpper().Contains("FO"):
                        return "FO";
                    case var _ when line.ToUpper().Contains("OZ"):
                            return "OZ";
                    case var _ when line.ToUpper().Contains("OZ."):
                        return "OZ.";
                    case var _ when line.ToUpper().Contains("OUNCES"):
                            return "OUNCES";
                    case var _ when line.ToUpper().Contains("OUNCE"):
                        return "OUNCE";
                    case var _ when line.ToUpper().Contains("LB"):
                        return "LB";
                    case var _ when line.ToUpper().Contains("LBS"):
                        return "LBS";
                    case var _ when line.ToUpper().Contains("TSP"):
                            return "TSP";
                    case var _ when line.ToUpper().Contains("TEASPOONS"):
                        return "TEASPOONS";
                    case var _ when line.ToUpper().Contains("TEASPOON"):
                        return "TEASPOON";
                    case var _ when line.ToUpper().Contains("TBSP"):
                        return "TBSP";
                    case var _ when line.ToUpper().Contains("TABLESPOONS"):
                        return "TABLESPOONS";
                    case var _ when line.ToUpper().Contains("TABLESPOON"):
                        return "TABLESPOON";
                    case var _ when line.ToUpper().Contains("CUP"):
                        return "CUP";
                    case var _ when line.ToUpper().Contains("CUPS"):
                        return "CUPS";
                    //TODO metric, none of my recipies currently use it 

                    default: return "UNIT";
                }
        }

        //TODO figure out if anything comes in metric
        public string GetAmountType(string unit)
        {
            switch (unit)
            {
                case var _ when unit.ToUpper().Equals("FL. OZ."):
                case var _ when unit.ToUpper().Equals("FL OZ"):
                case var _ when unit.ToUpper().Equals("FO"):
                case var _ when unit.ToUpper().Equals("FLUID OUNCES"):
                case var _ when unit.ToUpper().Equals("FLUID OUNCE"):
                case var _ when unit.ToUpper().Equals("TSP"):
                case var _ when unit.ToUpper().Equals("TSP."):
                case var _ when unit.ToUpper().Equals("TEASPOONS"):
                case var _ when unit.ToUpper().Equals("TEASPOON"):
                case var _ when unit.ToUpper().Equals("TBSP"):
                case var _ when unit.ToUpper().Equals("TBSP."):
                case var _ when unit.ToUpper().Equals("TABLESPOONS"):
                case var _ when unit.ToUpper().Equals("TABLESPOON"):
                case var _ when unit.ToUpper().Equals("CUP"):
                case var _ when unit.ToUpper().Equals("CUPS"):
                    return "ML";
                case var _ when unit.ToUpper().Equals("OZ"):
                case var _ when unit.ToUpper().Equals("OZ."):
                case var _ when unit.ToUpper().Equals("OUNCES"):
                case var _ when unit.ToUpper().Equals("OUNCE"):
                case var _ when unit.ToUpper().Equals("LB"):
                case var _ when unit.ToUpper().Equals("LBS"):
                    return "G";
            
                default: return "UNIT";
            }
        }

        //TODO figure out if anything comes in metric
        public decimal GetConversion(string unit, decimal amt)
        {
            switch (unit)
            {
                case var _ when unit.ToUpper().Equals("FL. OZ."):
                case var _ when unit.ToUpper().Equals("FL OZ"):
                case var _ when unit.ToUpper().Equals("FLUID OUNCES"):
                case var _ when unit.ToUpper().Equals("FLUID OUNCE"):
                    return amt * (decimal)29.5735;
                case var _ when unit.ToUpper().Equals("TSP"):
                case var _ when unit.ToUpper().Equals("TSP."):
                case var _ when unit.ToUpper().Equals("TEASPOONS"):
                case var _ when unit.ToUpper().Equals("TEASPOON"):
                    return amt * (decimal)4.92892;
                case var _ when unit.ToUpper().Equals("TBSP"):
                case var _ when unit.ToUpper().Equals("TBSP."):
                case var _ when unit.ToUpper().Equals("TABLESPOONS"):
                case var _ when unit.ToUpper().Equals("TABLESPOON"):
                    return amt * (decimal)14.7868;
                case var _ when unit.ToUpper().Equals("CUP"):
                case var _ when unit.ToUpper().Equals("CUPS"):
                    return amt * (decimal)236.5882365; 
                case var _ when unit.ToUpper().Equals("OZ."):
                case var _ when unit.ToUpper().Equals("OZ"):
                case var _ when unit.ToUpper().Equals("OUNCES"):
                case var _ when unit.ToUpper().Equals("OUNCE"):
                    return amt * (decimal)28.3495;
                case var _ when unit.ToUpper().Equals("LB"):
                case var _ when unit.ToUpper().Equals("LBS"):
                    return amt * (decimal)453.592;

                default: return amt;
            }
        }




        public int? GetMultiplier(ProductShoppingListDto dto)
        {
            if (dto.ProductAmount == null || dto.AmountRequired == null) return null;

            //var prodAmtType = GetAmountType(dto.ProductUnit);
            //var ingAmtType = GetAmountType(dto.IngredientUnit);

            var productAmt = GetConversion(dto.ProductUnit, (decimal)dto.ProductAmount);
            var ingredientAmt = GetConversion(dto.IngredientUnit, (decimal)dto.AmountRequired);

            return (int)Math.Ceiling(ingredientAmt / productAmt);

        }


    }
}
