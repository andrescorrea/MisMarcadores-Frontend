﻿using Microsoft.EntityFrameworkCore;
using MisMarcadores.Data.DataAccess;
using MisMarcadores.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MisMarcadores.Repository
{
    public class ParticipantesRepository : GenericRepository<Participante>, IParticipantesRepository
    {
        public ParticipantesRepository(MisMarcadoresContext context) : base(context) { }

        public void BorrarParticipante(Guid id)
        {
            Participante participante = context.Participantes.FirstOrDefault(d => d.Id == id);
            var favoritos = context.Favoritos.Where(f => f.IdParticipante == participante.Id);
            foreach (var favorito in favoritos)
            {
                context.Favoritos.Remove(favorito);
            }
            List<Encuentro> encuentros = context.Encuentros.Include(e => e.ParticipanteEncuentro).ToList();
            foreach (var encuentro in encuentros)
            {
                List<ParticipanteEncuentro> participanteEncuentros = encuentro.ParticipanteEncuentro.ToList();
                foreach (var pe in participanteEncuentros)
                {
                    if (pe.ParticipanteId.Equals(id))
                        context.Encuentros.Remove(encuentro);
                }
            }
            
            context.Participantes.Remove(participante);
        }

        public Participante ObtenerParticipantePorDeporte(string nombreDeporte, string nombreParticipante)
        {
            return context.Participantes.Include(e => e.Deporte).FirstOrDefault(x => x.Deporte.Nombre.Equals(nombreDeporte) && x.Nombre.Equals(nombreParticipante));
        }

        public void ModificarParticipante(Participante participante)
        {
            var participanteOriginal = GetByID(participante.Id);
            context.Participantes.Attach(participanteOriginal);
            var entry = context.Entry(participanteOriginal);
            entry.CurrentValues.SetValues(participante);
        }

        public Participante ObtenerParticipantePorId(Guid id)
        {
            return context.Participantes.Include(e => e.Deporte).FirstOrDefault(e => e.Id.Equals(id));
        }

        public Participante ObtenerParticipantePorNombre(string nombre)
        {
            return context.Participantes.Include(e => e.Deporte).FirstOrDefault(e => e.Nombre.Equals(nombre));
        }

        public List<Participante> ObtenerParticipantes()
        {
            return context.Participantes.Include(e => e.Deporte).Include(p => p.ParticipanteEncuentro).ToList();
        }

        public List<Participante> ObtenerParticipantesPorDeporte(string deporte)
        {
            return context.Participantes.Where(x => x.Deporte.Nombre.Equals(deporte)).ToList();
        }
    }
}
