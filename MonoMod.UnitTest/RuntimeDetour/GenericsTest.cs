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
                typeof(GenericsTest).GetMethod(nameof(FromMDCtx), BindingFlags.NonPublic | BindingFlags.Static),
                typeof(GenericsTest).GetMethod(nameof(To), BindingFlags.NonPublic | BindingFlags.Static));

            try {
                WrapperMD();
            } finally {
                DetourHelper.Generic.RemovePatch(handle);
            }

            handle = DetourHelper.Generic.AddPatch(
                typeof(GenericSrc<>).GetMethod(nameof(GenericSrc<int>.FromMTCtx), BindingFlags.Public | BindingFlags.Static),
                typeof(GenericsTest).GetMethod(nameof(To), BindingFlags.NonPublic | BindingFlags.Static));

            try {
                WrapperMT();
            } finally {
                DetourHelper.Generic.RemovePatch(handle);
            }

            handle = DetourHelper.Generic.AddPatch(
                typeof(GenericSrc<>).GetMethod(nameof(GenericSrc<int>.FromTCtx), BindingFlags.Public | BindingFlags.Instance),
                typeof(GenericsTest).GetMethod(nameof(ToWithThis), BindingFlags.NonPublic | BindingFlags.Static));

            try {
                WrapperT();
            } finally {
                DetourHelper.Generic.RemovePatch(handle);
            }
        }

        private static void WrapperMD() {
            FromMDCtx("hello");
            FromMDCtx(42);
        }

        // TODO: test all the other cases
        private static void FromMDCtx<T>(T value) {
            Assert.True(false, "Original generic was called when it shouldn't have been!");
        }

        private static void To<T>(T value) {
            if (typeof(T) == typeof(string)) {
                Assert.Equal("hello", (string)(object)value);
            } else if (typeof(T) == typeof(int)) {
                Assert.Equal(42, (int) (object) value);
            }else {
                Assert.True(false, $"To called with invalid type parameter {typeof(T)}");
            }
        }

        private static void ToWithThis<T>(object thisObj, T value) {
            Assert.IsType<GenericSrc<T>>(thisObj);
            To(value);
        }

        private static void WrapperMT() {
            GenericSrc<string>.FromMTCtx("hello");
            GenericSrc<int>.FromMTCtx(42);
        }

        private static void WrapperT() {
            new GenericSrc<string>().FromTCtx("hello");
            new GenericSrc<int>().FromTCtx(42);
        }

        private class GenericSrc<T> {
            public static void FromMTCtx(T value) {
                Assert.True(false, "Original generic was called when it shouldn't have been!");
            }
            public void FromTCtx(T value) {
                Assert.True(false, "Original generic was called when it shouldn't have been!");
            }
        }
    }
}
