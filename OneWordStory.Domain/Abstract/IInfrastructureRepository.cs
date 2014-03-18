using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneWordStory.Domain.Abstract
{
    public interface IInfrastructureRepository
    {
        void LogError(Exception exception, string errorCode = "");
    }
}
