using Logic;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web
{   
    public class EmpireHub : Hub
    {
        ILogic logic;

        public EmpireHub()
        {
            logic = new RealLogic();
        }

        public void AddEmpire(EmpireDTO empire)
        {
            logic.AddEmpire(EmpireDTO.Mapper.Map<EmpireDTO, Empire>(empire));
            Clients.All.NewEmpire(empire);
        }
        public EmpireDTO GetEmpire(int id)
        {
            return EmpireDTO.Mapper.Map<Empire, EmpireDTO>(logic.GetEmpire(id));
        }
        public List<string> GetEmpireNames()
        {
            return logic.GetEmpireNames();
        }
        public List<EmpireDTO> GetEmpires()
        {
            return EmpireDTO.Mapper.Map<List<Empire>, List<EmpireDTO>>(logic.GetEmpires());
        }
        public List<string> GetGovernmentNames()
        {
            return logic.GetGovernmentNames();
        }
        public void ModifyEmpire(int id, string newName, string newGovernment)
        {
            logic.ModifyEmpire(id, newName, newGovernment);
            Clients.All.EmpireModified(id);
        }
        public void ModifyEmpireGovernment(int id, string newGovernment)
        {
            logic.ModifyEmpireGovernment(id, newGovernment);
            Clients.All.EmpireModified(id);
        }
        public void ModifyEmpireName(int id, string newName)
        {
            logic.ModifyEmpireName(id, newName);
            Clients.All.EmpireModified(id);
        }
        public void RemoveEmpire(int id)
        {
            var k = GetEmpire(id);
            logic.RemoveEmpire(id);
            Clients.All.EmpireCrushed(k);
        }
    }
}
