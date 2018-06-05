using Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repo.EmpRepos
{
    public class EmpEFRepositry : GenericRepos.EFRepository<EMPIRE>, IEmpRepository
    {
        public EmpEFRepositry(DbContext context) : base(context)
        { }

        public override EMPIRE GetByID(int id)
        {
            return Get(x => x.EMPNO == id).SingleOrDefault();
        }
        public void Modify(int id, string newName, string newGovernment)
        {
            if (newName == null)
                throw new ArgumentException("NULL VALUE", "newName");
            if (newGovernment == null)
                throw new ArgumentException("NULL VALUE", "newGovernment");
            _Modify(id, newName, newGovernment);
        }
        public void ModifyGovernment(int id, string newGovernment)
        {
            if (newGovernment == null)
                throw new ArgumentException("NULL VALUE", "newGovernment");
            _Modify(id, newGovernment: newGovernment);
        }
        public void ModifyName(int id, string newName)
        {
            if (newName == null)
                throw new ArgumentException("NULL VALUE", "newName");
            _Modify(id, newName: newName);
        }
        //to minimize code duplication
        void _Modify(int id, string newName = null, string newGovernment = null)
        {
            var current = GetByID(id);
            if (current == null)
                throw new ArgumentException("NO DATA");
            if (newName != null)
                current.ENAME = newName;
            if (newGovernment != null)
                current.EGOV = newGovernment;
            context.SaveChanges();
        }
    }
}
