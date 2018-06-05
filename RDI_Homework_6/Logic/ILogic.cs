using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    public interface ILogic
    {
        List<Empire> GetEmpires();
        List<string> GetEmpireNames();
        List<string> GetGovernmentNames();

        void AddEmpire(Empire empire);
        void RemoveEmpire(int id);
        Empire GetEmpire(int id);
        void ModifyEmpire(int id, string newName, string newGovernment);
        void ModifyEmpireName(int id, string newName);
        void ModifyEmpireGovernment(int id, string newGovernment);
    }
}
