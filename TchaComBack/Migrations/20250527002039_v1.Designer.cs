﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TchaComBack.Data;

#nullable disable

namespace TchaComBack.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250527002039_v1")]
    partial class v1
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("TchaComBack.Models.CategoriaModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("DataAtualizacao")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DataCriacao")
                        .HasColumnType("datetime2");

                    b.Property<string>("Descricao")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Categorias");
                });

            modelBuilder.Entity("TchaComBack.Models.EstadoCivilModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("EstadoCivil")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("EstadoCivil");
                });

            modelBuilder.Entity("TchaComBack.Models.ExtratoPontoModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("DataBatida")
                        .HasColumnType("datetime2");

                    b.Property<TimeOnly?>("HoraEntrada1")
                        .HasColumnType("time");

                    b.Property<TimeOnly?>("HoraEntrada2")
                        .HasColumnType("time");

                    b.Property<TimeOnly?>("HoraSaida1")
                        .HasColumnType("time");

                    b.Property<TimeOnly?>("HoraSaida2")
                        .HasColumnType("time");

                    b.Property<TimeOnly?>("HorasExtras")
                        .HasColumnType("time");

                    b.Property<TimeOnly?>("HorasFaltas")
                        .HasColumnType("time");

                    b.Property<string>("Justificativa")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Matricula")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Matricula");

                    b.ToTable("ExtratoPonto");
                });

            modelBuilder.Entity("TchaComBack.Models.FuncionariosModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Ativo")
                        .IsRequired()
                        .HasColumnType("nvarchar(1)");

                    b.Property<string>("Cargo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Celular")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CidadeResidencia")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("DataAtualizacao")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DataCadastro")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DataIngresso")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DataNascimento")
                        .HasColumnType("datetime2");

                    b.Property<int>("DiasTrabalhados")
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Endereco")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("EstadoCivil")
                        .HasColumnType("int");

                    b.Property<int>("Matricula")
                        .HasColumnType("int");

                    b.Property<string>("Naturalidade")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NomeMae")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Raca")
                        .HasColumnType("int");

                    b.Property<decimal>("Salario")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("SetorId")
                        .HasColumnType("int");

                    b.Property<string>("Sexo")
                        .IsRequired()
                        .HasColumnType("nvarchar(1)");

                    b.Property<int>("UsuarioResponsavelId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("EstadoCivil");

                    b.HasIndex("Matricula")
                        .IsUnique();

                    b.HasIndex("Raca");

                    b.HasIndex("SetorId");

                    b.HasIndex("UsuarioResponsavelId");

                    b.ToTable("Funcionarios");
                });

            modelBuilder.Entity("TchaComBack.Models.RacaModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Raca")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Raca");
                });

            modelBuilder.Entity("TchaComBack.Models.SetoresModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Ativo")
                        .IsRequired()
                        .HasColumnType("nvarchar(1)");

                    b.Property<int>("CategoriaId")
                        .HasColumnType("int");

                    b.Property<DateTime>("DataAtualizacao")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DataCriacao")
                        .HasColumnType("datetime2");

                    b.Property<string>("Descricao")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EmailResponsavelSetor")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImagemSetor")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Localizacao")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ResponsavelSetor")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SexoResponsavel")
                        .IsRequired()
                        .HasColumnType("nvarchar(1)");

                    b.Property<int>("UsuarioResponsavelId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CategoriaId");

                    b.ToTable("Setores");
                });

            modelBuilder.Entity("TchaComBack.Models.UsuariosModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Ativo")
                        .IsRequired()
                        .HasColumnType("nvarchar(1)");

                    b.Property<int?>("Confirmado")
                        .HasColumnType("int");

                    b.Property<DateTime>("DataCadastro")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DataHoraEsqueceuSenha")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Hash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Matricula")
                        .HasColumnType("int");

                    b.Property<string>("NomeCompleto")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Salt")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Senha")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("SetorId")
                        .HasColumnType("int");

                    b.Property<int?>("TipoPerfil")
                        .HasColumnType("int");

                    b.Property<DateTime>("UltimoAcesso")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("Matricula");

                    b.HasIndex("SetorId");

                    b.ToTable("Usuarios");
                });

            modelBuilder.Entity("TchaComBack.Models.ExtratoPontoModel", b =>
                {
                    b.HasOne("TchaComBack.Models.FuncionariosModel", "Funcionario")
                        .WithMany("ExtratosDePonto")
                        .HasForeignKey("Matricula")
                        .HasPrincipalKey("Matricula")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Funcionario");
                });

            modelBuilder.Entity("TchaComBack.Models.FuncionariosModel", b =>
                {
                    b.HasOne("TchaComBack.Models.EstadoCivilModel", "EstadoCivilNav")
                        .WithMany("Funcionarios")
                        .HasForeignKey("EstadoCivil")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("TchaComBack.Models.RacaModel", "RacaNav")
                        .WithMany("Funcionarios")
                        .HasForeignKey("Raca")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("TchaComBack.Models.SetoresModel", "Setor")
                        .WithMany("Funcionarios")
                        .HasForeignKey("SetorId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("TchaComBack.Models.UsuariosModel", "UsuarioResponsavel")
                        .WithMany()
                        .HasForeignKey("UsuarioResponsavelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("EstadoCivilNav");

                    b.Navigation("RacaNav");

                    b.Navigation("Setor");

                    b.Navigation("UsuarioResponsavel");
                });

            modelBuilder.Entity("TchaComBack.Models.SetoresModel", b =>
                {
                    b.HasOne("TchaComBack.Models.CategoriaModel", "Categoria")
                        .WithMany("Setores")
                        .HasForeignKey("CategoriaId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Categoria");
                });

            modelBuilder.Entity("TchaComBack.Models.UsuariosModel", b =>
                {
                    b.HasOne("TchaComBack.Models.FuncionariosModel", "Funcionario")
                        .WithMany("UsuariosVinculados")
                        .HasForeignKey("Matricula")
                        .HasPrincipalKey("Matricula")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("TchaComBack.Models.SetoresModel", "Setor")
                        .WithMany("Usuarios")
                        .HasForeignKey("SetorId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Funcionario");

                    b.Navigation("Setor");
                });

            modelBuilder.Entity("TchaComBack.Models.CategoriaModel", b =>
                {
                    b.Navigation("Setores");
                });

            modelBuilder.Entity("TchaComBack.Models.EstadoCivilModel", b =>
                {
                    b.Navigation("Funcionarios");
                });

            modelBuilder.Entity("TchaComBack.Models.FuncionariosModel", b =>
                {
                    b.Navigation("ExtratosDePonto");

                    b.Navigation("UsuariosVinculados");
                });

            modelBuilder.Entity("TchaComBack.Models.RacaModel", b =>
                {
                    b.Navigation("Funcionarios");
                });

            modelBuilder.Entity("TchaComBack.Models.SetoresModel", b =>
                {
                    b.Navigation("Funcionarios");

                    b.Navigation("Usuarios");
                });
#pragma warning restore 612, 618
        }
    }
}
