namespace Asp_MVC.Dto
{
    public class UserViewModel
    {
        public UserDto CurrentUser { get; set; }
        public IEnumerable<UserDto> Users { get; set; }
    }
}
