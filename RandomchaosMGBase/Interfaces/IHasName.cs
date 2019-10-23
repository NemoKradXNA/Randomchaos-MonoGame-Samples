using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomchaosMGBase
{
    public interface IHasName
    {     
        string Name { get; set; }

        void GenerateDefaultName();
    }
}
