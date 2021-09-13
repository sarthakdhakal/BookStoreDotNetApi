namespace BookStoreDotNetApi.Models
{
    public class UserWithToken : User
    {
        public string Token { get; set; }
    

        public UserWithToken(User user)
        {
            this.UserId = user.UserId;
            this.Password = user.Password;
            this.FirstName = user.FirstName;
            this.MiddleName = user.MiddleName;
            this.LastName = user.LastName;
            this.PubId = user.PubId;
            this.HireDate = user.HireDate;
        }
    }
}