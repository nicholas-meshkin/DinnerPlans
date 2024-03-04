using Microsoft.AspNetCore.Components.Forms;
using DinnerPlans.Shared.DTOs;
using DinnerPlans.Shared.Util;
using DinnerPlans.Client.Services.IServices;
using Tesseract;

namespace DinnerPlans.Client.Services
{
    public class FileReadService : IFileReadService
    {
        public FileReadService() { }

       // private readonly long maxFileSize = 1024 * 3*1000;
        public async Task<IList<RecipeIngredientDto>> GetIngredientsFromUpload(IBrowserFile file)
        {
                var ingStrings = await ExtractIngredientImageFileLines(file);
                if (ingStrings == null || !ingStrings.Any()) return new List<RecipeIngredientDto>();

                var dtos = await ProcessIngredientList(ingStrings.ToList());
                if (dtos == null || !dtos.Any()) return new List<RecipeIngredientDto>();

                return dtos;
        }

        public async Task<IList<InstructionDto>> GetInstructionsFromUpload(IBrowserFile file)
        {
                var insStrings = await ExtractInstructionImageFileText(file);
                if (insStrings == null || !insStrings.Any()) return new List<InstructionDto>();

                var dtos = await ProcessRecipeText(insStrings.ToList());
                if (dtos == null || !dtos.Any()) return new List<InstructionDto>();

                return dtos;
        }

        //public async Task<string> UploadImageFile(IBrowserFile file) 
        //{
        //    try 
        //    {
        //        string trustedFileName = Guid.NewGuid().ToString();
               
        //        if (file.Size > 0 && file.Size < maxFileSize)
        //        {
        //            var fileBytes = await UploadMedia(file);
        //            //TODO move this somewhere, also change on deploy
        //            var path = Path.Combine(@"C:\Users\nickt\Desktop\recipies\RecipeImages", trustedFileName + ".jpg");
        //            path = path.Replace('/', '\\');

        //            using (BinaryWriter bw = new(File.OpenWrite(path)))
        //            {
        //                string test = "test";
        //                bw.Write(test);

        //                //bw.Write(fileBytes,0,fileBytes.Length);


        //            }

        //            //string[] filePaths = Directory.GetFiles(@"C:\Users\nickt\Desktop\recipies\RecipeImagesPreImport\", "*.pdf",
        //            //                                SearchOption.TopDirectoryOnly);

        //            // var testp = Path.GetFullPath(@"C:\Users\nickt\Desktop\recipies\RecipeImagesPreImport\PorkSlaw.jpg");
        //            // testp = testp.Replace('/', '\\');
        //            // testp = testp.TrimStart('\\');
                    


        //            var testp = @"Users\nickt\PorkSlaw.jpg";
        //            var f = File.ReadAllBytes(testp);
                    
        //            //await using (FileStream fs = new(path, FileMode.Create, FileAccess.Write)) 
        //            //{
        //            //    File.Create(path);
        //            //    fs.Write(fileBytes, 0, fileBytes.Length);
        //            //    fs.Close();
        //            //    //file.OpenReadStream(maxFileSize).CopyToAsync(fs);
        //            //}




        //            return path;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //        //Logger.LogError("File: {Filename} Error: {Error}",
        //        //    file.Name, ex.Message);
        //    }
        //    return "Error uploading file";

        //}
      

        //adapted from https://stackoverflow.com/questions/70790385/ibrowserfile-fromimagefileasync-incomplete-image
        private static async Task<byte[]> UploadMedia(IBrowserFile file)
        {
            //TODO set max file size
            //virus scanner?
            //change file name
            var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            await using var fs = new FileStream(path, FileMode.Create);

            await file.OpenReadStream(file.Size).CopyToAsync(fs);

            var bytes = new byte[file.Size];

            fs.Position = 0;

            await fs.ReadAsync(bytes);

            fs.Close();

            File.Delete(path);

            return bytes;
        }

        
        private async Task<IList<string>?> ExtractIngredientImageFileLines(IBrowserFile file)
        {
            var engin = new TesseractEngine(@"C:\Users\nickt\source\repos\DinnerPlans\DinnerPlans\Client\tessdata", "eng", EngineMode.Default);
            using (var engine = new TesseractEngine(@"C:\Users\nickt\Desktop\tessdata\tessdata", "eng", EngineMode.Default))
            {
                if (file.Size > 0)
                {
                    var fileBytes = await UploadMedia(file);
                    using (var img = Pix.LoadFromMemory(fileBytes))
                    {
                        using (var page = engine.Process(img))
                        {
                            var text = page.GetText();
                            var ingredients = text.ToString().ToUpper().Split('\n').ToList();
                            return ingredients;
                        }
                    }
                }

            }

            return new List<string>();
        }

        private async Task<IList<string>?> ExtractInstructionImageFileText(IBrowserFile file)
        {

            //var path = @"C:\Users\nickt\Desktop\recipies\wingsauceingredients.jpg";
            using (var engine = new TesseractEngine(@"C:\Users\nickt\Desktop\tessdata\tessdata", "eng", EngineMode.Default))
            {
                if (file.Size > 0)
                {
                    var fileBytes = await UploadMedia(file);
                    using (var img = Pix.LoadFromMemory(fileBytes))
                    {
                        using (var page = engine.Process(img))
                        {
                            var instructions = new List<string>();
                            var text = page.GetText();
                            if(text.Contains("\n\n")) instructions = text.ToString().ToUpper().Split("\n\n").ToList();
                            else instructions.Add(text.ToString().ToUpper());
                            
                            return instructions;
                        }
                    }
                }

            }

            return new List<string>();
        }


        private async Task<IList<RecipeIngredientDto>?> ProcessIngredientList(List<string> ingredients)
        {
            try
            {
                var list = new List<RecipeIngredientDto>();

                foreach (var item in ingredients)
                {
                    if (string.IsNullOrEmpty(item) || item.Equals("Ingredients")) continue;
                    var amountUnitString = Utils.ParseUnit(item);
                    double? amount = 0;
                    var ingredient = "";
                    if (!amountUnitString.Equals("UNIT"))
                    {
                        var amountStartIndex = item.ToUpper().IndexOf(amountUnitString);
                        if (amountStartIndex != -1)
                        {
                            amount = Utils.ParseAmount(item.Substring(0, amountStartIndex));
                            ingredient = item.Substring(amountStartIndex + amountUnitString.Length);
                        }
                        else ingredient = item;
                    }
                    else
                    {
                        amount = Utils.ParseAmount(item);
                        ingredient = item.Substring(item.LastIndexOfAny(item.Where(c => Char.IsDigit(c)).ToArray()) + 1);
                    }

                    if (ingredient.StartsWith(" ")) ingredient = ingredient.TrimStart(' ');
                    ingredient = ingredient.TrimEnd();

                    var listItemDto = new RecipeIngredientDto
                    {
                        Item = ingredient,
                        AmountDto = new RecipeAmountDto { Unit = Utils.GetStandardizedUnit(amountUnitString), Amount = (double)amount }
                    };
                    list.Add(listItemDto);
                }
                return list;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private async Task<List<InstructionDto>> ProcessRecipeText(List<string> inst)
        {
            try
            {
                //remove blank lines
                for(int i = 0; i < inst.Count; i++)
                {
                    if (string.IsNullOrWhiteSpace(inst[i]))
                    {
                        inst.RemoveAt(i);
                        i--;
                    }
                }

                var dtos = new List<InstructionDto>();

                var indexes = new List<int>();

                //attempt to see if list is numbered
                if (inst.Any(a => char.IsDigit(a[0])))
                {
                    foreach (var line in inst)
                    {
                        if (char.IsDigit(line[0])) indexes.Add(inst.IndexOf(line));
                    }

                    var groupedInst = new List<string>();

                    foreach (var index in indexes)
                    {
                        var currInst = inst[index];
                        var i = index + 1;
                        while (!indexes.Contains(i) && i < inst.Count())
                        {
                            currInst += inst[i];
                            i++;
                        }
                        groupedInst.Add(currInst);
                    }

                    foreach(var ginst in groupedInst)
                    {
                        dtos.Add(new InstructionDto { Instruction = ginst, Order = groupedInst.IndexOf(ginst) });
                    }
                }
                else
                {
                    foreach (var line in inst)
                    {
                        dtos.Add(new InstructionDto { Instruction = line, Order = inst.IndexOf(line) });
                    }
                }

                return dtos;

            }
            catch (Exception ex)
            {
                return null;
            }
           
        }

    }
}
