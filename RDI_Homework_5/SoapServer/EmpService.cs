using Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace SignalRServer
{
    public class EmpService : IEmpService
    {
        ILogic logic;

        public EmpService()
        {
            logic = new RealLogic();
        }

        public bool AddEmpire(EmpireDTO empire)
        {
            logic.AddEmpire(EmpireDTO.Mapper.Map<EmpireDTO, Empire>(empire));
            return true;
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
        public bool ModifyEmpire(int id, string newName, string newGovernment)
        {
            logic.ModifyEmpire(id, newName, newGovernment);
            return true;
        }
        public bool ModifyEmpireGovernment(int id, string newGovernment)
        {
            logic.ModifyEmpireGovernment(id, newGovernment);
            return true;
        }
        public bool ModifyEmpireName(int id, string newName)
        {
            logic.ModifyEmpireName(id, newName);
            return true;
        }
        public bool RemoveEmpire(int id)
        {
            try
            {
                logic.RemoveEmpire(id);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
