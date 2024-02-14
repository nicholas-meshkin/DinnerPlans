namespace DinnerPlans.Server.Core.ErrorHandling
{
    public class ErrorMessages
    {
        private ErrorMessages() { }
        public static readonly string Error500 = "Something messed up, sorry. :(";
        public static readonly string ErrorInvalidName = "Recipe name is already in use";
    }
}
