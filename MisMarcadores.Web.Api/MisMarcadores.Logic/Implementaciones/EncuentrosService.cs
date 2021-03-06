﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MisMarcadores.Data.DataAccess;
using MisMarcadores.Data.Entities;
using MisMarcadores.Repository;

namespace MisMarcadores.Logic
{
    public class EncuentrosService : IEncuentrosService
    {
        private IUnitOfWork _unitOfWork;
        private IDeportesRepository _deportesRepository;
        private IParticipantesRepository _participantesRepository;
        private IEncuentrosRepository _encuentrosRepository;

        public EncuentrosService(IUnitOfWork unitOfWork, IEncuentrosRepository encuentrosRepository, IDeportesRepository deportesRepository,
            IParticipantesRepository participantesRepository)
        {
            _unitOfWork = unitOfWork;
            _deportesRepository = deportesRepository;
            _participantesRepository = participantesRepository;
            _encuentrosRepository = encuentrosRepository;
        }

        public Guid AgregarEncuentro(Encuentro encuentro)
        {
            if (DatosInvalidosEncuentro(encuentro))
                throw new EncuentroDataException();
            Deporte deporte = _deportesRepository.ObtenerDeportePorNombre(encuentro.Deporte.Nombre);
            if (deporte == null)
                throw new NoExisteDeporteException();
            ICollection<ParticipanteEncuentro> Puntajes = encuentro.ParticipanteEncuentro;
            if (Puntajes==null)
                throw new NoExisteParticipanteException();
            if (Puntajes.Count == 0)
                throw new NoExisteParticipanteException();
            if (Puntajes.Count < 2)
                throw new CantidadIncorrectaDePartcipantesException();
            if (!deporte.EsIndividual && Puntajes.Count != 2)
                throw new CantidadIncorrectaDePartcipantesException();
            if (HayPartcipanteRepetido(Puntajes))
                throw new ParticipantesRepetidoException();
            if (ExisteEcuentroMismoDiaParaParticipantes(encuentro))
                throw new ExisteEncuentroMismoDiaException();
            if (!PuntajesCorrectos(encuentro, deporte))
                throw new ResultadoIncorrectoException();
            foreach (ParticipanteEncuentro p in Puntajes)
            {
                p.Participante = _participantesRepository.ObtenerParticipantePorId(p.ParticipanteId);
                if (!p.Participante.Deporte.Equals(deporte))
                    throw new NoCoincideDeporteException();               
            }
            encuentro.ParticipanteEncuentro = Puntajes;
            encuentro.Deporte.Id = deporte.Id;
            _encuentrosRepository.Insert(encuentro);
            _unitOfWork.Save();
            return encuentro.Id;
        }



        public void BorrarEncuentro(Guid id)
        {
            Encuentro encuentro = ObtenerEncuentroPorId(id);
            if (encuentro == null)
                throw new NoExisteEncuentroException();
            
            _encuentrosRepository.BorrarEncuentro(id);
            _unitOfWork.Save();
        }

        public void ModificarEncuentro(Guid id, Encuentro encuentro)
        {
            if (DatosInvalidosEncuentro(encuentro))
                throw new EncuentroDataException();
            Deporte deporte = _deportesRepository.ObtenerDeportePorNombre(encuentro.Deporte.Nombre);
            if (deporte == null)
                throw new NoExisteDeporteException();    
            Encuentro encuentroActual = ObtenerEncuentroPorId(id);
            if (encuentroActual == null)
                throw new NoExisteEncuentroException();
            ICollection<ParticipanteEncuentro> Puntajes = encuentro.ParticipanteEncuentro;
            foreach (ParticipanteEncuentro p in Puntajes)
            {
                p.Participante = _participantesRepository.ObtenerParticipantePorId(p.ParticipanteId);
                if (!p.Participante.Deporte.Equals(deporte))
                    throw new NoCoincideDeporteException();
            }
            DateTime nuevaFecha = encuentro.FechaHora;
            if (ExisteEcuentroMismoDiaParaParticipantes(encuentro))
                throw new ExisteEncuentroMismoDiaException();
            if (Puntajes.Count == 0)
                throw new NoExisteParticipanteException();
            if (Puntajes.Count < 2)
                throw new CantidadIncorrectaDePartcipantesException();
            if (!deporte.EsIndividual && Puntajes.Count != 2)
                throw new CantidadIncorrectaDePartcipantesException();
            if (HayPartcipanteRepetido(Puntajes))
                throw new ParticipantesRepetidoException();
            if (!PuntajesCorrectos(encuentro, deporte))
                throw new ResultadoIncorrectoException();
            encuentro = encuentroActual;
            encuentro.FechaHora = nuevaFecha;
            encuentro.ParticipanteEncuentro = Puntajes;
            encuentro.Deporte = deporte;
            _encuentrosRepository.ModificarEncuentro(encuentro);
            _unitOfWork.Save();
        }

        public Encuentro ObtenerEncuentroPorId(Guid id)
        {
             
            Encuentro encuentro = _encuentrosRepository.ObtenerEncuentroPorId(id);
            if (encuentro != null)
            {
                foreach (ParticipanteEncuentro p in encuentro.ParticipanteEncuentro)
                {
                    p.Participante = _participantesRepository.ObtenerParticipantePorId(p.ParticipanteId);
                }
            }
            return encuentro;
        }

        public IEnumerable<Encuentro> ObtenerEncuentros()
        {

            IEnumerable<Encuentro>  encuentros = _encuentrosRepository.ObtenerEncuentros();
            if (encuentros != null)
            {
                foreach (Encuentro e in encuentros)
                {
                    foreach (ParticipanteEncuentro p in e.ParticipanteEncuentro)
                    {
                        p.Participante = _participantesRepository.ObtenerParticipantePorId(p.ParticipanteId);
                    }
                }
            }
            

            return encuentros;
        }

        public IEnumerable<Encuentro> ObtenerEncuentrosDeParticipante(Guid id)
        {
            return _encuentrosRepository.ObtenerEncuentrosPorParticipante(id);
        }

        private bool DatosInvalidosEncuentro(Encuentro encuentro)
        {
            return !CampoValido(encuentro.Deporte.Nombre);
         
        }

        private bool CampoValido(string campo)
        {
            return !string.IsNullOrWhiteSpace(campo);
        } 

        private bool HayPartcipanteRepetido(ICollection<ParticipanteEncuentro> Puntajes)
        {
            return Puntajes.Select(x => x.ParticipanteId).Distinct().Count() != Puntajes.Count();
        }

        public bool FixtureGenerado(DateTime fechaInicio, string deporte, string tipo)
        {
            if (!CampoValido(deporte) || !CampoValido(tipo) || (tipo != "Liga" && tipo != "Grupos"))
                throw new EncuentroDataException();
            Deporte deporteActual = _deportesRepository.ObtenerDeportePorNombre(deporte);
            if (deporteActual == null)
                throw new NoExisteDeporteException();
            if (deporteActual.EsIndividual)
                throw new TipoDeFixtureIncompatibleException();
            List<Participante> participantes = _participantesRepository.ObtenerParticipantesPorDeporte(deporte);
            if (participantes == null || participantes.Count == 1 || participantes.Count == 0)
                throw new NoExistenParticipantesException();
            Fixture fixture = GenerarFixture(fechaInicio, tipo, participantes);
            List<Encuentro> encuentros = fixture.GenerarFixture();
            bool generado = true;
                
            foreach (Encuentro encuentro in encuentros)
            {
                if (ExisteEncuentroParticipante(encuentro.FechaHora, participantes))
                    generado = false;
                    break;
            }
            foreach (Encuentro encuentro in encuentros)
            {
                if (ExisteEcuentroMismoDiaParaParticipantes(encuentro))
                    throw new ExisteEncuentroMismoDiaException();
            }
            
            if (generado)
            {
                foreach (Encuentro encuentro in encuentros)
                {
                    encuentro.Deporte = deporteActual;
                    _encuentrosRepository.Insert(encuentro);
                    _unitOfWork.Save();
                }
            }
            return generado;
        }

        public bool ExisteEcuentroMismoDiaParaParticipantes(Encuentro encuentro) {
            foreach (var item in encuentro.ParticipanteEncuentro)
            {
                if (ExisteEncuentroMismoDiaParaUnParcipante(item.ParticipanteId, encuentro.FechaHora))
                {
                    return true;
                }
            }
            return false;
        }

        public bool PuntajesCorrectos(Encuentro encuentro, Deporte deporte) {
            if (deporte.EsIndividual)
                return PuntajesCorrectosIndividual(encuentro);
            else
                return PuntajesCorrectosEquipo(encuentro);
        }


        public bool PuntajesCorrectosIndividual(Encuentro encuentro)
        {
            List<ParticipanteEncuentro> puntajes = encuentro.ParticipanteEncuentro.ToList();
            var puntajesAceptables = Enumerable.Range(0, 4).ToList();
            foreach (var puntaje in puntajes)
            {
                if (puntajesAceptables.Contains(puntaje.PuntosObtenidos))
                {
                    if (puntaje.PuntosObtenidos != 0)
                    {
                        puntajesAceptables.Remove(puntaje.PuntosObtenidos);
                    }
                }
                else
                {
                    return false;
                }                   
            }
            return true;
        }
        public bool PuntajesCorrectosEquipo(Encuentro encuentro)
        {
            List<ParticipanteEncuentro> puntajes = encuentro.ParticipanteEncuentro.ToList();
            if ((puntajes[0].PuntosObtenidos == 3 && puntajes[1].PuntosObtenidos == 0) ||
                (puntajes[0].PuntosObtenidos == 0 && puntajes[1].PuntosObtenidos == 3) ||
                (puntajes[0].PuntosObtenidos == 1 && puntajes[1].PuntosObtenidos == 1))
            {
                return true;
            }
            return false;
        }
        private bool ExisteEncuentroMismoDiaParaUnParcipante(Guid participanteId, DateTime date ) {

            Participante participante = _participantesRepository.ObtenerParticipantePorId(participanteId);
            ICollection<Encuentro> encuentros = _encuentrosRepository.ObtenerEncuentros();
            if (encuentros!=null)
            {
                foreach (var encuentro in encuentros)
                {
                    foreach (var puntaje in encuentro.ParticipanteEncuentro)
                    {
                        if (puntaje.ParticipanteId.Equals(participante.Id) && EsElMismoDia(date, encuentro.FechaHora))
                        {
                            return true;
                        }
                    }
                }
            }           
            return false;
        }

        private bool EsElMismoDia(DateTime diaUno, DateTime diaDos) {
            return (diaUno.Year == diaDos.Year && diaUno.Month == diaDos.Month && diaUno.Day == diaDos.Day);
        }

        private bool ExisteEncuentroParticipante(DateTime fecha, List<Participante> participantes)
        {
            List<Encuentro> encuentros = _encuentrosRepository.ObtenerEncuentros();
            foreach (Participante participante in participantes)
            {
                if (_encuentrosRepository.ExisteEncuentroEnFecha(fecha, participante.Id))
                    return true;
            }
            return false;
        }

        private Fixture GenerarFixture(DateTime fechaInicio, string tipo, List<Participante> participantes)
        {
            switch (tipo)
            {
                case "Liga":
                    return new FixtureLiga(fechaInicio, participantes);
                case "Grupos":
                    if (participantes.Count % 4 != 0)
                        throw new FixtureGruposDataException();
                    return new FixtureGrupos(fechaInicio, participantes);
            }
            return null;
        }

        public void BorrarTodos()
        {
            _encuentrosRepository.BorrarTodos();
            _unitOfWork.Save();
        }

        public IEnumerable<Encuentro> ObtenerEncuentrosPorDeporte(string nombre)
        {
            return _encuentrosRepository.ObtenerEncuentrosPorDeporte(nombre);
        }

        public IEnumerable<Encuentro> ObtenerEncuentrosPorParticipante(Guid id)
        {
            return _encuentrosRepository.ObtenerEncuentrosPorParticipante(id);
        }

        public void AgregarComentario(Guid idEncuentro, string nombreUsuario, string texto)
        {
            Encuentro encuentroActual = ObtenerEncuentroPorId(idEncuentro);
            if (encuentroActual == null)
                throw new NoExisteEncuentroException();

            Comentario comentario = new Comentario
            {
                IdEncuentro = idEncuentro,
                NombreUsuario = nombreUsuario,
                Texto = texto
            };
            _encuentrosRepository.AgregarComentario(comentario);
            _unitOfWork.Save();

        }

        public List<Comentario> ObtenerComentarios(Guid idEncuentro)
        {
            return _encuentrosRepository.ObtenerComentarios(idEncuentro);
        }
    }
}
