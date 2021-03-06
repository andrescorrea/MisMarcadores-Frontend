﻿using System;
using System.Collections.Generic;
using System.Text;
using MisMarcadores.Data.Entities;

namespace MisMarcadores.Logic
{
    public class FixtureLiga : Fixture
    {
        private readonly DateTime fechaInicio;
        private readonly List<Participante> participantes;
 


        public FixtureLiga(DateTime fechaInicio, List<Participante> participantes)
        {
         
            this.fechaInicio = fechaInicio;
            this.participantes = participantes;
        }

        public override List<Encuentro> GenerarFixture()
        {
            List<Encuentro> encuentros = new List<Encuentro>();
            int fechaEncuentro = 0;
            for (int i = 0; i < participantes.Count; i++)
            {
                for (int j = 0; j < participantes.Count; j++)
                {
                    if (i != j)
                    {
                        Encuentro encuentro = new Encuentro();
                        encuentro.FechaHora = fechaInicio.AddDays(fechaEncuentro);
                        encuentro.Id = new Guid();
                        ICollection<ParticipanteEncuentro> participanteEncuentros = new List<ParticipanteEncuentro>();
                        ParticipanteEncuentro participanteEncuentroUno = new ParticipanteEncuentro();
                        participanteEncuentroUno.ParticipanteId = participantes[i].Id;
                        ParticipanteEncuentro participanteEncuentroDos = new ParticipanteEncuentro();
                        participanteEncuentroDos.ParticipanteId = participantes[j].Id;
                        participanteEncuentros.Add(participanteEncuentroUno);
                        participanteEncuentros.Add(participanteEncuentroDos);
                        encuentro.ParticipanteEncuentro = participanteEncuentros;
                        
                        encuentros.Add(encuentro);
                        fechaEncuentro += 3;
                    }
                }
            }
            return encuentros;
        }
    }
}
