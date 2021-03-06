﻿using Microsoft.EntityFrameworkCore;
using MisMarcadores.Data.Entities;


namespace MisMarcadores.Data.DataAccess
{
    public class MisMarcadoresContext : DbContext
    {
        public MisMarcadoresContext(DbContextOptions<MisMarcadoresContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Participante> Participantes { get; set; }
        public DbSet<Deporte> Deportes { get; set; }
        public DbSet<Encuentro> Encuentros { get; set; }
        public DbSet<Comentario> Comentarios { get; set; }
        public DbSet<Favorito> Favoritos { get; set; }
        public DbSet<Sesion> Sesiones { get; set; }
        public DbSet<ParticipanteEncuentro> ParticipanteEncuentro { get; set; }
       public DbSet<Log> Log { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
          
            modelBuilder.Entity<Usuario>().Property(u => u.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Deporte>().Property(u => u.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Participante>().Property(u => u.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Encuentro>().Property(u => u.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Comentario>().Property(u => u.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Favorito>().Property(u => u.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Sesion>().HasKey(s => s.NombreUsuario);
            modelBuilder.Entity<Log>().Property(u => u.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<ParticipanteEncuentro>()
                .HasKey(x => new { x.ParticipanteId, x.EncuentroId });

            modelBuilder.Entity<ParticipanteEncuentro>()
                .HasOne(ut => ut.Encuentro)
                .WithMany(t => t.ParticipanteEncuentro)
                .HasForeignKey(ut => ut.EncuentroId);

            modelBuilder.Entity<ParticipanteEncuentro>()
               .HasOne(ut => ut.Participante)
               .WithMany(t => t.ParticipanteEncuentro)
               .HasForeignKey(ut => ut.ParticipanteId);

        }

    }
}