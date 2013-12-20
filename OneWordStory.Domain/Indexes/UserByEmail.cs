using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OneWordStory.Domain.Entities;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace OneWordStory.Domain.Indexes
{
    public class UserByEmail : AbstractIndexCreationTask<User>
    {
        public override string IndexName
        {
            get
            {
                return "User/ByEmail";
            }
        }

        public UserByEmail()
        {
            Map = users => from user in users
                           select new { user.Email };
            
        }
    }
}
