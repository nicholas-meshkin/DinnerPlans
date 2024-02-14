
namespace DinnerPlans.Shared.DTOs
{
    public class RecipeCommentDto
    {
        public DateTime? Updated { get; set; }
        public string? UserName { get; set; }
        public string? Comment { get; set; }
        public int CommentId { get; set; }
        public int RecipeId { get; set; }
        public int UserId { get; set; }
        
    }
}
