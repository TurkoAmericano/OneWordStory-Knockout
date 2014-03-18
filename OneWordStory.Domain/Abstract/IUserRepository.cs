using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OneWordStory.Domain.Entities;
using OneWordStory.Domain.Infrastructure;

namespace OneWordStory.Domain.Abstract
{
    public interface IUserRepository
    {



        GetUserResult SaveUser(User user);
        UserErrorCode DeleteUser(string userId);
        GetUserResult GetUserById(string userId);
        GetUserResult GetUserByEmail(string email);
        


    }
}

