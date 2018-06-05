using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repo.EmpRepos
{
    public interface IEmpRepository : GenericRepos.IRepository<EMPIRE>
    {
        void Modify(int id, string newName, string newGovernment);
        void ModifyName(int id, string newName);
        void ModifyGovernment(int id, string newGovernment);
    }
}
