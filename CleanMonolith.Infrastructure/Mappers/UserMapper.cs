using CleanMonolith.Domain.Entities;
using CleanMonolith.Infrastructure.Entity;

public static class UserMapper
{
    public static UserMaster ToDomain(Tbl_UserMaster entity)
    {
        return new UserMaster
        {
            UserId = entity.UserID,
            LoginId = entity.LoginID,
            PasswordHash = entity.Password,
            LoginName = entity.LoginName,
            Email = entity.EmailID
        };
    }

    public static Tbl_UserMaster ToEntity(UserMaster domain)
    {
        return new Tbl_UserMaster
        {
            UserID = domain.UserId,
            LoginID = domain.LoginId,
            Password = domain.PasswordHash,
            LoginName = domain.LoginName,
            EmailID = domain.Email
        };
    }
}