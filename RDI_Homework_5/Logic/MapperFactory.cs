using AutoMapper;
using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    static class MapperFactory
    {
        public static IMapper CreateMapper()
        {
            return new MapperConfiguration(cfg => cfg.CreateMap<Empire, EMPIRE>().ReverseMap()).CreateMapper();
        }
    }
}
