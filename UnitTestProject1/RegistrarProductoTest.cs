using System;
using NUnit.Framework;
using UBICALO.Models;
using UBICALO.ViewModel;

namespace UnitTestProject1
{
    [TestFixture]
    public class RegistrarProductoTest
    {
        [Test]
        public void FlujoRegistraProducto()
        {
            UbicaloEntities context = new UbicaloEntities();
            VmRegistrarProducto vmRP = new VmRegistrarProducto();
            vmRP.fill(context, 10);
            Assert.IsNotNull(vmRP);

        }
    }
}
