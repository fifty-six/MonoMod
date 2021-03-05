using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MonoMod.UnitTest {
    public class GenericsTest {
        [Fact]
        public void TestGenerics() {

            int handle = DetourHelper.Generic.AddPatch(
                typeof(GenericsTest).GetMethod(nameof(From), BindingFlags.NonPublic | BindingFlags.Static),
                typeof(GenericsTest).GetMethod(nameof(To), BindingFlags.NonPublic | BindingFlags.Static));

            try {
                Wrapper();
            } finally {
                DetourHelper.Generic.RemovePatch(handle);
            }
        }

        private static void Wrapper() {
            From("hello");
            From(42);
        }

        // TODO: test all the other cases
        private static void From<T>(T value) {
            Assert.True(false, "Original generic was called when it shouldn't have been!");
        }

        private static void To<T>(T value) {
            Assert.True(true); // all is well
        }
    }
}
