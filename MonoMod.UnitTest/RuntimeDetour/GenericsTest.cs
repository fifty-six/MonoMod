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
        #region Basic generics
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
                Assert.Equal("hello", (string) (object) value);
            } else if (typeof(T) == typeof(int)) {
                Assert.Equal(42, (int) (object) value);
            } else {
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

        private partial class GenericSrc<T> {
            public static void FromMTCtx(T value) {
                Assert.True(false, "Original generic was called when it shouldn't have been!");
            }
            public void FromTCtx(T value) {
                Assert.True(false, "Original generic was called when it shouldn't have been!");
            }
        }
        #endregion

        #region Return buffer generics
        [Fact]
        public void TestReturnBufferGenerics() {

            int handle = DetourHelper.Generic.AddPatch(
                typeof(GenericsTest).GetMethod(nameof(FromMDCtxR), BindingFlags.NonPublic | BindingFlags.Static),
                typeof(GenericsTest).GetMethod(nameof(ToR), BindingFlags.NonPublic | BindingFlags.Static));

            try {
                WrapperMDR();
            } finally {
                DetourHelper.Generic.RemovePatch(handle);
            }

            handle = DetourHelper.Generic.AddPatch(
                typeof(GenericSrc<>).GetMethod(nameof(GenericSrc<int>.FromMTCtxR), BindingFlags.Public | BindingFlags.Static),
                typeof(GenericsTest).GetMethod(nameof(ToR), BindingFlags.NonPublic | BindingFlags.Static));

            try {
                WrapperMTR();
            } finally {
                DetourHelper.Generic.RemovePatch(handle);
            }

            handle = DetourHelper.Generic.AddPatch(
                typeof(GenericSrc<>).GetMethod(nameof(GenericSrc<int>.FromTCtxR), BindingFlags.Public | BindingFlags.Instance),
                typeof(GenericsTest).GetMethod(nameof(ToWithThisR), BindingFlags.NonPublic | BindingFlags.Static));

            try {
                WrapperTR();
            } finally {
                DetourHelper.Generic.RemovePatch(handle);
            }

            handle = DetourHelper.Generic.AddPatch(
                typeof(GenericSrc<>).GetMethod(nameof(GenericSrc<int>.FromMDCtxR), BindingFlags.Public | BindingFlags.Instance),
                typeof(GenericsTest).GetMethod(nameof(ToWithThisR2), BindingFlags.NonPublic | BindingFlags.Static));

            try {
                WrapperT2R();
            } finally {
                DetourHelper.Generic.RemovePatch(handle);
            }
        }

        // TODO: test more types of structs

        private static void WrapperMDR() {
            Assert.Equal(5m, FromMDCtxR("hello"));
            Assert.Equal(5m, FromMDCtxR(42));
        }

        private static void WrapperMTR() {
            Assert.Equal(5m, GenericSrc<string>.FromMTCtxR("hello"));
            Assert.Equal(5m, GenericSrc<int>.FromMTCtxR(42));
        }

        private static void WrapperTR() {
            Assert.Equal(5m, new GenericSrc<string>().FromTCtxR("hello"));
            Assert.Equal(5m, new GenericSrc<int>().FromTCtxR(42));
        }

        private static void WrapperT2R() {
            Assert.Equal(5m, new GenericSrc<string>().FromMDCtxR("hello", "hello"));
            Assert.Equal(5m, new GenericSrc<int>().FromMDCtxR(42, "hello"));
            Assert.Equal(5m, new GenericSrc<string>().FromMDCtxR("hello", 42));
            Assert.Equal(5m, new GenericSrc<int>().FromMDCtxR(42, 42));
        }

        private static decimal ToR<T>(T value) {
            if (typeof(T) == typeof(string)) {
                Assert.Equal("hello", (string) (object) value);
            } else if (typeof(T) == typeof(int)) {
                Assert.Equal(42, (int) (object) value);
            } else {
                Assert.True(false, $"To called with invalid type parameter {typeof(T)}");
            }

            return 5m;
        }

        private static decimal ToWithThisR<T>(object thisObj, T value) {
            Assert.IsType<GenericSrc<T>>(thisObj);
            return ToR(value);
        }
        private static decimal ToWithThisR2<T, T2>(object thisObj, T value, T2 value2) {
            Assert.IsType<GenericSrc<T>>(thisObj);
            _ = ToR(value2);
            return ToR(value);
        }

        private static decimal FromMDCtxR<T>(T value) {
            Assert.True(false, "Original generic was called when it shouldn't have been!");
            return 0m;
        }

        private partial class GenericSrc<T> {

            public static decimal FromMTCtxR(T value) {
                Assert.True(false, "Original generic was called when it shouldn't have been!");
                return 0m;
            }
            public decimal FromTCtxR(T value) {
                Assert.True(false, "Original generic was called when it shouldn't have been!");
                return 0m;
            }
            public decimal FromMDCtxR<T2>(T value1, T2 value2) {
                Assert.True(false, "Original generic was called when it shouldn't have been!");
                return 0m;
            }
        }
        #endregion
    }
}