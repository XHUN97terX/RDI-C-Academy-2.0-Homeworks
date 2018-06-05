using AutoMapper;
using Data;
using Repo;
using Repo.EmpRepos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


//EMPIRES, mert EntityFramework így gondolta...
//újranevezés nem segített, nem tudom, hol kéne még átállítani, marad EMPIRES...
namespace Logic
{
    public class RealLogic : ILogic
    {
        protected MyRepository repo;
        IMapper mapper;

        public RealLogic()
        {
            repo = new MyRepository(new EmpEFRepositry(new EMPDBEntities()));
            mapper = MapperFactory.CreateMapper();
        }

        public void AddEmpire(Empire empire)
        {
            repo.EmpRepo.Insert(mapper.Map<Empire, EMPIRE>(empire));
        }
        public Empire GetEmpire(int id)
        {
            return mapper.Map<EMPIRE, Empire>(repo.EmpRepo.GetByID(id));
        }
        public List<string> GetEmpireNames()
        {
            return GetEmpires().Select(x => x.ENAME).Distinct().ToList();
        }
        public List<Empire> GetEmpires()
        {
            return mapper.Map<IQueryable<EMPIRE>, List<Empire>>(repo.EmpRepo.GetAll());
        }
        public List<string> GetGovernmentNames()
        {
            return GetEmpires().Select(x => x.EGOV).Distinct().ToList();
        }
        public void ModifyEmpire(int id, string newName, string newGovernment)
        {
            repo.EmpRepo.Modify(id, newName, newGovernment);
        }
        public void ModifyEmpireName(int id, string newName)
        {
            repo.EmpRepo.ModifyName(id, newName);
        }
        public void ModifyEmpireGovernment(int id, string newGovernment)
        {
            repo.EmpRepo.ModifyGovernment(id, newGovernment);
        }
        public void RemoveEmpire(int id)
        {
            repo.EmpRepo.Delete(id);
        }
    }
}
