using Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Web.Controllers
{
    public class EmpireController : ApiController
    {
        ILogic logic;

        public EmpireController()
        {
            logic = new RealLogic();
        }

        [HttpGet]
        [ActionName("names")]
        public IEnumerable<string> GetEmpireNames()
        {
            return logic.GetEmpireNames();
        }

        [HttpGet]
        [ActionName("governments")]
        public IEnumerable<string> GetGovernmentNames()
        {
            return logic.GetGovernmentNames();
        }

        [HttpGet]
        [ActionName("empires")]
        public IEnumerable<EmpireDTO> GetEmpires()
        {
            return EmpireDTO.Mapper.Map<List<Empire>, IEnumerable<EmpireDTO>>(logic.GetEmpires());
        }

        [HttpGet]
        [ActionName("empire")]
        public EmpireDTO GetEmpire(int id)
        {
            return EmpireDTO.Mapper.Map<Empire, EmpireDTO>(logic.GetEmpire(id));
        }

        [HttpPost]
        [ActionName("add")]
        public string AddEmpire(EmpireDTO empire)
        {
            try
            {
                logic.AddEmpire(new Empire()
                {
                    EMPNO = logic.GetEmpires().Max(x => x.EMPNO) + 1,
                    EGOV = empire.EGov,
                    ENAME = empire.EName
                });
                return "SUCCESS";
            }
            catch
            {
                return "FAIL";
            }
        }

        [HttpPost]
        [ActionName("remove")]
        public string RemoveEmpire([FromBody]int id)
        {
            try
            {
                logic.RemoveEmpire(id);
                return "SUCCESS";
            }
            catch
            {
                return "FAIL";
            }
        }

        [HttpPost]
        [ActionName("modifyname")]
        public string ModifyEmpireName(EmpireDTO empire)
        {
            try
            {
                logic.ModifyEmpireName(empire.Empno, empire.EName);
                return "SUCCESS";
            }
            catch
            {
                return "FAIL";
            }
        }

        [HttpPost]
        [ActionName("modifygovernment")]
        public string ModifyGovernmentName(EmpireDTO empire)
        {
            try
            {
                logic.ModifyEmpireGovernment(empire.Empno, empire.EGov);
                return "SUCCESS";
            }
            catch
            {
                return "FAIL";
            }
        }

        [HttpPost]
        [ActionName("modify")]
        public string ModifyEmpire(EmpireDTO empire)
        {
            try
            {
                logic.ModifyEmpire(empire.Empno, empire.EName, empire.EGov);
                return "SUCCESS";
            }
            catch
            {
                return "FAIL";
            }
        }
    }
}