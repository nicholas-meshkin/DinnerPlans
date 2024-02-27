namespace DinnerPlans.Server.Core.ErrorHandling
{
    public class ErrorMessages
    {
        private ErrorMessages() { }
        public static readonly string Error500 = "Something messed up, sorry. :(";
        public static readonly string ErrorInvalidName = "Recipe name is already in use";
        public static readonly string FileTooBig = "Image file is too large";
        public static readonly string FileSizeZero = "Image file is empty";
    }
}
