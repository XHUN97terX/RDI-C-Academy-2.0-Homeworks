﻿using AutoMapper;
using Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SignalRServer
{
    public class EmpireDTO
    {
        public static IMapper Mapper
        { get; private set; }

        static EmpireDTO()
        {
            Mapper = new MapperConfiguration(cfg => cfg.CreateMap<Empire, EmpireDTO>().ReverseMap()).CreateMapper();
        }

        public int Empno
        { get; set; }
        public string EName
        { get; set; }
        public string EGov
        { get; set; }
    }
}
