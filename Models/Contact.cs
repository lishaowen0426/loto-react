using System.ComponentModel.DataAnnotations;

namespace Loto.Models
{
    public class Contact
    {
        [Required(ErrorMessage = "名前を入力してください")]
        [Display(Name = "名前")]
        public string Name { get; set; }

        [Required(ErrorMessage = "メールアドレスを入力してください")]
        [EmailAddress(ErrorMessage = "有効なメールアドレスを入力してください")]
        [Display(Name = "メールアドレス")]
        public string Email { get; set; }

        [Required(ErrorMessage = "メッセージを入力してください")]
        [Display(Name = "メッセージ")]
        public string Message { get; set; }
    }
}
