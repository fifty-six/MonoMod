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

            int handle;

            handle = DetourHelper.Generic.AddPatch(
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

            handle = DetourHelper.Generic.AddPatch(
                typeof(GenericSrc<>).GetMethod(nameof(GenericSrc<int>.FromMDCtx), BindingFlags.Public | BindingFlags.Instance),
                typeof(GenericsTest).GetMethod(nameof(ToWithThis2), BindingFlags.NonPublic | BindingFlags.Static));

            try {
                WrapperT2();
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
        private static void ToWithThis2<T, T2>(object thisObj, T value, T2 value2) {
            Assert.IsType<GenericSrc<T>>(thisObj);
            To(value2);
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

        private static void WrapperT2() {
            new GenericSrc<string>().FromMDCtx("hello", "hello");
            new GenericSrc<int>().FromMDCtx(42, "hello");
            new GenericSrc<string>().FromMDCtx("hello", 42);
            new GenericSrc<int>().FromMDCtx(42, 42);
        }

        private partial class GenericSrc<T> {
            public static void FromMTCtx(T value) {
                Assert.True(false, "Original generic was called when it shouldn't have been!");
            }
            public void FromTCtx(T value) {
                Assert.True(false, "Original generic was called when it shouldn't have been!");
            }
            public void FromMDCtx<T2>(T value, T2 value2) {
                Assert.True(false, "Original generic was called when it shouldn't have been!");
            }
        }
        #endregion

        #region Stack spilling basic generics
        [Fact]
        public void TestGenericsStackSpill() {

            int handle;
            
            handle = DetourHelper.Generic.AddPatch(
                typeof(GenericsTest).GetMethod(nameof(FromSMDCtx), BindingFlags.NonPublic | BindingFlags.Static),
                typeof(GenericsTest).GetMethod(nameof(ToS), BindingFlags.NonPublic | BindingFlags.Static));

            try {
                WrapperSMD();
            } finally {
                DetourHelper.Generic.RemovePatch(handle);
            }

            handle = DetourHelper.Generic.AddPatch(
                typeof(GenericSrc<>).GetMethod(nameof(GenericSrc<int>.FromSMTCtx), BindingFlags.Public | BindingFlags.Static),
                typeof(GenericsTest).GetMethod(nameof(ToS), BindingFlags.NonPublic | BindingFlags.Static));

            try {
                WrapperSMT();
            } finally {
                DetourHelper.Generic.RemovePatch(handle);
            }
            
            handle = DetourHelper.Generic.AddPatch(
                typeof(GenericSrc<>).GetMethod(nameof(GenericSrc<int>.FromSTCtx), BindingFlags.Public | BindingFlags.Instance),
                typeof(GenericsTest).GetMethod(nameof(ToWithThisS), BindingFlags.NonPublic | BindingFlags.Static));

            try {
                WrapperST();
            } finally {
                DetourHelper.Generic.RemovePatch(handle);
            }

            handle = DetourHelper.Generic.AddPatch(
                typeof(GenericSrc<>).GetMethod(nameof(GenericSrc<int>.FromSMDCtx), BindingFlags.Public | BindingFlags.Instance),
                typeof(GenericsTest).GetMethod(nameof(ToWithThis2S), BindingFlags.NonPublic | BindingFlags.Static));

            try {
                WrapperST2();
            } finally {
                DetourHelper.Generic.RemovePatch(handle);
            }
        }

        private static void WrapperSMD() {
            FromSMDCtx("hello", 1, 2, 3, 4, 5, 6);
            FromSMDCtx(42, 1, 2, 3, 4, 5, 6);
        }

        // TODO: test all the other cases

        private static void FromSMDCtx<T>(T value, int a, int b, int c, int d, int e, int f) {
            Assert.True(false, "Original generic was called when it shouldn't have been!");
        }

        private static void ToS<T>(T value, int a, int b, int c, int d, int e, int f) {
            if (typeof(T) == typeof(string)) {
                Assert.Equal("hello", (string) (object) value);
            } else if (typeof(T) == typeof(int)) {
                Assert.Equal(42, (int) (object) value);
            } else {
                Assert.True(false, $"To called with invalid type parameter {typeof(T)}");
            }
            Assert.Equal(1, a);
            Assert.Equal(2, b);
            Assert.Equal(3, c);
            Assert.Equal(4, d);
            Assert.Equal(5, e);
            Assert.Equal(6, f);
        }

        private static void ToWithThisS<T>(object thisObj, T value, int a, int b, int c, int d, int e, int f) {
            Assert.IsType<GenericSrc<T>>(thisObj);
            ToS(value, a, b, c, d, e, f);
        }
        private static void ToWithThis2S<T, T2>(object thisObj, T value, T2 value2, int a, int b, int c, int d, int e, int f) {
            Assert.IsType<GenericSrc<T>>(thisObj);
            ToS(value2, a, b, c, d, e, f);
            ToS(value, a, b, c, d, e, f);
        }

        private static void WrapperSMT() {
            GenericSrc<string>.FromSMTCtx("hello", 1, 2, 3, 4, 5, 6);
            GenericSrc<int>.FromSMTCtx(42, 1, 2, 3, 4, 5, 6);
        }

        private static void WrapperST() {
            new GenericSrc<string>().FromSTCtx("hello", 1, 2, 3, 4, 5, 6);
            new GenericSrc<int>().FromSTCtx(42, 1, 2, 3, 4, 5, 6);
        }

        private static void WrapperST2() {
            new GenericSrc<string>().FromSMDCtx("hello", "hello", 1, 2, 3, 4, 5, 6);
            new GenericSrc<int>().FromSMDCtx(42, "hello", 1, 2, 3, 4, 5, 6);
            new GenericSrc<string>().FromSMDCtx("hello", 42, 1, 2, 3, 4, 5, 6);
            new GenericSrc<int>().FromSMDCtx(42, 42, 1, 2, 3, 4, 5, 6);
        }

        private partial class GenericSrc<T> {
            public static void FromSMTCtx(T value, int a, int b, int c, int d, int e, int f) {
                Assert.True(false, "Original generic was called when it shouldn't have been!");
            }
            public void FromSTCtx(T value, int a, int b, int c, int d, int e, int f) {
                Assert.True(false, "Original generic was called when it shouldn't have been!");
            }
            public void FromSMDCtx<T2>(T value, T2 value2, int a, int b, int c, int d, int e, int f) {
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

        #region Stack spilling return bugger generics
        [Fact]
        public void TestGenericsStackSpillReturnBuffer() {

            int handle;

            handle = DetourHelper.Generic.AddPatch(
                typeof(GenericsTest).GetMethod(nameof(FromSMDCtxR), BindingFlags.NonPublic | BindingFlags.Static),
                typeof(GenericsTest).GetMethod(nameof(ToSR), BindingFlags.NonPublic | BindingFlags.Static));

            try {
                WrapperSMDR();
            } finally {
                DetourHelper.Generic.RemovePatch(handle);
            }

            handle = DetourHelper.Generic.AddPatch(
                typeof(GenericSrc<>).GetMethod(nameof(GenericSrc<int>.FromSMTCtxR), BindingFlags.Public | BindingFlags.Static),
                typeof(GenericsTest).GetMethod(nameof(ToSR), BindingFlags.NonPublic | BindingFlags.Static));

            try {
                WrapperSMTR();
            } finally {
                DetourHelper.Generic.RemovePatch(handle);
            }

            handle = DetourHelper.Generic.AddPatch(
                typeof(GenericSrc<>).GetMethod(nameof(GenericSrc<int>.FromSTCtxR), BindingFlags.Public | BindingFlags.Instance),
                typeof(GenericsTest).GetMethod(nameof(ToWithThisSR), BindingFlags.NonPublic | BindingFlags.Static));

            try {
                WrapperSTR();
            } finally {
                DetourHelper.Generic.RemovePatch(handle);
            }

            handle = DetourHelper.Generic.AddPatch(
                typeof(GenericSrc<>).GetMethod(nameof(GenericSrc<int>.FromSMDCtxR), BindingFlags.Public | BindingFlags.Instance),
                typeof(GenericsTest).GetMethod(nameof(ToWithThis2SR), BindingFlags.NonPublic | BindingFlags.Static));

            try {
                WrapperST2R();
            } finally {
                DetourHelper.Generic.RemovePatch(handle);
            }
        }

        private static void WrapperSMDR() {
            Assert.Equal(5m, FromSMDCtxR("hello", 1, 2, 3, 4, 5, 6));
            Assert.Equal(5m, FromSMDCtxR(42, 1, 2, 3, 4, 5, 6));
        }

        // TODO: test all the other cases

        private static decimal FromSMDCtxR<T>(T value, int a, int b, int c, int d, int e, int f) {
            Assert.True(false, "Original generic was called when it shouldn't have been!");
            return 0m;
        }

        private static decimal ToSR<T>(T value, int a, int b, int c, int d, int e, int f) {
            if (typeof(T) == typeof(string)) {
                Assert.Equal("hello", (string) (object) value);
            } else if (typeof(T) == typeof(int)) {
                Assert.Equal(42, (int) (object) value);
            } else {
                Assert.True(false, $"To called with invalid type parameter {typeof(T)}");
            }
            Assert.Equal(1, a);
            Assert.Equal(2, b);
            Assert.Equal(3, c);
            Assert.Equal(4, d);
            Assert.Equal(5, e);
            Assert.Equal(6, f);
            return 5m;
        }

        private static decimal ToWithThisSR<T>(object thisObj, T value, int a, int b, int c, int d, int e, int f) {
            _ = Assert.IsType<GenericSrc<T>>(thisObj);
            return ToSR(value, a, b, c, d, e, f);
        }
        private static decimal ToWithThis2SR<T, T2>(object thisObj, T value, T2 value2, int a, int b, int c, int d, int e, int f) {
            _ = Assert.IsType<GenericSrc<T>>(thisObj);
            _ = ToSR(value2, a, b, c, d, e, f);
            return ToSR(value, a, b, c, d, e, f);
        }

        private static void WrapperSMTR() {
            Assert.Equal(5m, GenericSrc<string>.FromSMTCtxR("hello", 1, 2, 3, 4, 5, 6));
            Assert.Equal(5m, GenericSrc<int>.FromSMTCtxR(42, 1, 2, 3, 4, 5, 6));
        }

        private static void WrapperSTR() {
            Assert.Equal(5m, new GenericSrc<string>().FromSTCtxR("hello", 1, 2, 3, 4, 5, 6));
            Assert.Equal(5m, new GenericSrc<int>().FromSTCtxR(42, 1, 2, 3, 4, 5, 6));
        }

        private static void WrapperST2R() {
            Assert.Equal(5m, new GenericSrc<string>().FromSMDCtxR("hello", "hello", 1, 2, 3, 4, 5, 6));
            Assert.Equal(5m, new GenericSrc<int>().FromSMDCtxR(42, "hello", 1, 2, 3, 4, 5, 6));
            Assert.Equal(5m, new GenericSrc<string>().FromSMDCtxR("hello", 42, 1, 2, 3, 4, 5, 6));
            Assert.Equal(5m, new GenericSrc<int>().FromSMDCtxR(42, 42, 1, 2, 3, 4, 5, 6));
        }

        private partial class GenericSrc<T> {
            public static decimal FromSMTCtxR(T value, int a, int b, int c, int d, int e, int f) {
                Assert.True(false, "Original generic was called when it shouldn't have been!");
                return 0m;
            }
            public decimal FromSTCtxR(T value, int a, int b, int c, int d, int e, int f) {
                Assert.True(false, "Original generic was called when it shouldn't have been!");
                return 0m;
            }
            public decimal FromSMDCtxR<T2>(T value, T2 value2, int a, int b, int c, int d, int e, int f) {
                Assert.True(false, "Original generic was called when it shouldn't have been!");
                return 0m;
            }
        }
        #endregion
    }
}