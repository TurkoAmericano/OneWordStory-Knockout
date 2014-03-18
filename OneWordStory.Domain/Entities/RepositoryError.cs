using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("OneWordStory.Tests")]

namespace OneWordStory.Domain.Entities
{
    
    internal class LogError
    {
        public string ErrorCode { get; set; }
        public Exception Exception { get; set; }
        public DateTime DateOfOccurence { get; set; }

    }
}
