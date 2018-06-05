using Repo.EmpRepos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repo
{
    public class MyRepository
    {
        public IEmpRepository EmpRepo
        { get; private set; }

        public MyRepository(IEmpRepository empRepo)
        {
            EmpRepo = empRepo;
        }
    }
}
