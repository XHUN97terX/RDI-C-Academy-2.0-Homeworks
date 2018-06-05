using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Web
{
    [ServiceContract]
    public interface IEmpService
    {
        [OperationContract]
        List<EmpireDTO> GetEmpires();
        [OperationContract]
        List<string> GetEmpireNames();
        [OperationContract]
        List<string> GetGovernmentNames();
        [OperationContract]
        bool AddEmpire(EmpireDTO empire);
        [OperationContract]
        bool RemoveEmpire(int id);
        [OperationContract]
        EmpireDTO GetEmpire(int id);
        [OperationContract]
        bool ModifyEmpire(int id, string newName, string newGovernment);
        [OperationContract]
        bool ModifyEmpireName(int id, string newName);
        [OperationContract]
        bool ModifyEmpireGovernment(int id, string newGovernment);
    }
}
