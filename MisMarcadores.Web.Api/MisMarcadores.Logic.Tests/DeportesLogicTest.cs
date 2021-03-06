﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using MisMarcadores.Data.DataAccess;
using MisMarcadores.Data.Entities;
using MisMarcadores.Logic;
using MisMarcadores.Repository;
using Moq;
using System.Collections.Generic;
using System.Linq;

namespace MisMarcadores.Logic.Tests
{
    [TestClass]
    public class DeportesLogicTest
    {
        [TestMethod]
        public void ObtenerDeportesOkTest()
        {
            //Arrange
            var deportesEsperados = TestHelper.ObtenerDeportesFalsos();

            var mockDeportesRepository = new Mock<IDeportesRepository>();
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockDeportesRepository
                .Setup(r => r.GetAll())
                .Returns(deportesEsperados);

            var businessLogic = new DeportesService(mockUnitOfWork.Object, mockDeportesRepository.Object);

            //Act
            IEnumerable<Deporte> obtainedResult = businessLogic.ObtenerDeportes();

            //Assert
            mockDeportesRepository.VerifyAll();
            Assert.IsNotNull(obtainedResult);
            Assert.AreEqual(deportesEsperados, obtainedResult);
        }

        [TestMethod]
        public void ObtenerDeportesNullTest()
        {
            //Arrange
            List<Deporte> deportesEsperados = null;

            var mockDeportesRepository = new Mock<IDeportesRepository>();
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockDeportesRepository
                .Setup(r => r.GetAll())
                .Returns(deportesEsperados);

            var businessLogic = new DeportesService(mockUnitOfWork.Object, mockDeportesRepository.Object);

            //Act
            IEnumerable<Deporte> obtainedResult = businessLogic.ObtenerDeportes();

            //Assert
            mockDeportesRepository.VerifyAll();
            Assert.IsNull(obtainedResult);
        }

        [TestMethod]
        public void AgregarDeporteOkTest()
        {
            //Arrange
            var fakeDeporte = TestHelper.ObtenerDeporteFalso();

            var mockDeportesRepository = new Mock<IDeportesRepository>();
            var mockUnitOfWork = new Mock<IUnitOfWork>();

            mockDeportesRepository
                .Setup(r => r.Insert(fakeDeporte));

            var businessLogic = new DeportesService(mockUnitOfWork.Object, mockDeportesRepository.Object);

            //Act
            businessLogic.AgregarDeporte(fakeDeporte);

            //Assert
            mockDeportesRepository.VerifyAll();
        }

        [TestMethod]
        [ExpectedException(typeof(DeporteDataException))]
        public void AgregarDeporteNombreVacioTest()
        {
            var fakeDeporte = TestHelper.ObtenerDeporteNombreVacio();
            var mockDeportesRepository = new Mock<IDeportesRepository>();
            var mockUnitOfWork = new Mock<IUnitOfWork>();

            mockDeportesRepository
                .Setup(r => r.Insert(fakeDeporte));

            var businessLogic = new DeportesService(mockUnitOfWork.Object, mockDeportesRepository.Object);

            //Act
            businessLogic.AgregarDeporte(fakeDeporte);

            //Assert
            mockDeportesRepository.VerifyAll();
        }

        [TestMethod]
        [ExpectedException(typeof(ExisteDeporteException))]
        public void AgregarDeporteExistenteErrorTest()
        {
            var fakeDeporte = TestHelper.ObtenerDeporteFalso();
            var fakeNombreDeporte = fakeDeporte.Nombre;
            var mockDeportesRepository = new Mock<IDeportesRepository>();
            var mockUnitOfWork = new Mock<IUnitOfWork>();

            mockDeportesRepository.Setup(r => r.ObtenerDeportePorNombre(fakeNombreDeporte)).Returns(fakeDeporte);
            mockDeportesRepository.Setup(r => r.Insert(fakeDeporte));

            var businessLogic = new DeportesService(mockUnitOfWork.Object, mockDeportesRepository.Object);

            //Act
            businessLogic.AgregarDeporte(fakeDeporte);

            //Assert
            mockDeportesRepository.VerifyAll();
        }

        [TestMethod]
        public void ObtenerDeportePorNombreOkTest()
        {
            //Arrange
            var fakeDeporte = TestHelper.ObtenerDeporteFalso();
            var fakeNombreDeporte = "Futbol";

            var mockDeportesRepository = new Mock<IDeportesRepository>();
            var mockUnitOfWork = new Mock<IUnitOfWork>();

            mockDeportesRepository
                .Setup(r => r.ObtenerDeportePorNombre(fakeNombreDeporte))
                .Returns(fakeDeporte);

            var businessLogic = new DeportesService(mockUnitOfWork.Object, mockDeportesRepository.Object);

            //Act
            Deporte obtainedResult = businessLogic.ObtenerDeportePorNombre(fakeNombreDeporte);

            //Assert
            mockDeportesRepository.VerifyAll();
            Assert.IsNotNull(obtainedResult);
            Assert.AreEqual(fakeNombreDeporte, obtainedResult.Nombre);
        }

        [TestMethod]
        public void ObtenerDeportePorNombreErrorNotFoundTest()
        {
            //Arrange
            var fakeNombreDeporte = "PingPong";

            var mockDeportesRepository = new Mock<IDeportesRepository>();
            var mockUnitOfWork = new Mock<IUnitOfWork>();

            mockDeportesRepository
                .Setup(r => r.ObtenerDeportePorNombre(fakeNombreDeporte))
                .Returns((Deporte)null);

            var businessLogic = new DeportesService(mockUnitOfWork.Object, mockDeportesRepository.Object);

            //Act
            Deporte obtainedResult = businessLogic.ObtenerDeportePorNombre(fakeNombreDeporte);

            //Assert
            mockDeportesRepository.VerifyAll();
            Assert.IsNull(obtainedResult);
        }


        [TestMethod]
        [ExpectedException(typeof(NoExisteDeporteException))]
        public void ActualizarDeporteNoExistenteTest()
        {
            //Arrange
            var fakeDeporte = TestHelper.ObtenerDeporteFalso();
            var fakeNombreDeporte = fakeDeporte.Nombre;
            var mockDeportesRepository = new Mock<IDeportesRepository>();
            var mockUnitOfWork = new Mock<IUnitOfWork>();

            var businessLogic = new DeportesService(mockUnitOfWork.Object, mockDeportesRepository.Object);

            //Act
            businessLogic.ModificarDeporte(fakeNombreDeporte, fakeDeporte);

            //Assert
            mockDeportesRepository.VerifyAll();
        }

        [TestMethod]
        [ExpectedException(typeof(DeporteDataException))]
        public void ActualizarDeporteDatosErroneosTest()
        {
            //Arrange
            var fakeDeporte = TestHelper.ObtenerDeporteFalso();
            var fakeNombreDeporte = "";
            var mockDeportesRepository = new Mock<IDeportesRepository>();
            var mockUnitOfWork = new Mock<IUnitOfWork>();

            var businessLogic = new DeportesService(mockUnitOfWork.Object, mockDeportesRepository.Object);

            //Act
            businessLogic.ModificarDeporte(fakeNombreDeporte, fakeDeporte);

            //Assert
            mockDeportesRepository.VerifyAll();
        }

        [TestMethod]
        public void BorrarDeporteOkTest()
        {
            //Arrange
            var fakeDeporte = TestHelper.ObtenerDeporteFalso();
            var fakeNombreDeporte = fakeDeporte.Nombre;

            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockDeportesRepository = new Mock<IDeportesRepository>();
            mockDeportesRepository
                .Setup(r => r.ObtenerDeportePorNombre(fakeNombreDeporte)).Returns(fakeDeporte);
            mockDeportesRepository
                .Setup(r => r.BorrarDeporte(fakeNombreDeporte));

            var businessLogic = new DeportesService(mockUnitOfWork.Object, mockDeportesRepository.Object);

            //Act
            businessLogic.BorrarDeporte(fakeNombreDeporte);

            //Assert
            mockDeportesRepository.VerifyAll();
        }

        [TestMethod]
        [ExpectedException(typeof(NoExisteDeporteException))]
        public void BorrarDeporteNoExistenteErrorTest()
        {
            //Arrange
            var fakeDeporte = TestHelper.ObtenerDeporteFalso();
            var fakeNombreDeporte = fakeDeporte.Nombre;

            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockDeportesRepository = new Mock<IDeportesRepository>();
            mockDeportesRepository
                .Setup(r => r.ObtenerDeportePorNombre(fakeNombreDeporte)).Returns((Deporte)null);

            var businessLogic = new DeportesService(mockUnitOfWork.Object, mockDeportesRepository.Object);

            //Act
            businessLogic.BorrarDeporte(fakeNombreDeporte);

            //Assert
            mockDeportesRepository.VerifyAll();
        }

    }
}
