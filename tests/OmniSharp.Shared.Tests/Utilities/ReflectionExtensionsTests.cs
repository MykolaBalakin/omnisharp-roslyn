using System;
using System.Reflection;
using Xunit;
using OmniSharp.Utilities;

namespace OmniSharp.Shared.Tests.Utilities;

public class ReflectionExtensionsFacts
{
    class TestClass
    {
        public static string StaticMethod(string s) => s;

        private string _property;

        public string Property
        {
            get => _property;
            set => _property = value;
        }

        public TestClass()
        {
        }

        public TestClass(string property)
        {
            Property = property;
        }

        public string PublicMethod(string s)
        {
            return s;
        }

        private void PrivateMethod()
        {
        }
    }

    [Fact]
    public void LazyGetType_ReturnsType()
    {
        var expectedType = typeof(TestClass);

        var assemblyLazy = new Lazy<Assembly>(() => expectedType.Assembly);
        var actualType = assemblyLazy.LazyGetType(expectedType.FullName);

        Assert.Equal(expectedType, actualType.Value);
    }

    [Fact]
    public void LazyGetType_NullAssembly_ThrowsException()
    {
        var expectedType = typeof(TestClass);
        Lazy<Assembly> assemblyLazy = null;
        Assert.Throws<ArgumentNullException>(() => assemblyLazy.LazyGetType(expectedType.FullName));
    }

    [Fact]
    public void LazyGetType_UnknownType_ThrowsException()
    {
        var expectedType = typeof(TestClass);
        var assemblyLazy = new Lazy<Assembly>(() => expectedType.Assembly);
        var actualType = assemblyLazy.LazyGetType("UnknownType");
        Assert.Throws<InvalidOperationException>(() => actualType.Value);
    }

    [Fact]
    public void LazyGetMethod_ReturnsMethod()
    {
        var targetType = typeof(TestClass);
        var expectedMethod = targetType.GetMethod(nameof(TestClass.PublicMethod));

        var typeLazy = new Lazy<Type>(() => targetType);
        var actualMethod = typeLazy.LazyGetMethod(nameof(TestClass.PublicMethod));

        Assert.Equal(expectedMethod, actualMethod.Value);
    }

    [Fact]
    public void LazyGetMethod_NullType_ThrowsException()
    {
        Lazy<Type> typeLazy = null;
        Assert.Throws<ArgumentNullException>(() => typeLazy.LazyGetMethod(nameof(TestClass.PublicMethod)));
    }

    [Fact]
    public void LazyGetMethod_UnknownMethod_ThrowsException()
    {
        var typeLazy = new Lazy<Type>(() => typeof(TestClass));
        var actualResult = typeLazy.LazyGetMethod("UnknownMethod");
        Assert.Throws<InvalidOperationException>(() => actualResult.Value);
    }

    [Fact]
    public void LazyGetMethod_PrivateMethod_ReturnsMethod()
    {
        var targetType = typeof(TestClass);
        var expectedMethod = targetType.GetMethod("PrivateMethod", BindingFlags.Instance | BindingFlags.NonPublic);

        var typeLazy = new Lazy<Type>(() => targetType);
        var actualMethod = typeLazy.LazyGetMethod("PrivateMethod", BindingFlags.Instance | BindingFlags.NonPublic);

        Assert.Equal(expectedMethod, actualMethod.Value);
    }

    [Fact]
    public void LazyGetMethod_PrivateMethodNullType_ThrowsException()
    {
        Lazy<Type> typeLazy = null;
        Assert.Throws<ArgumentNullException>(() =>
            typeLazy.LazyGetMethod("PrivateMethod", BindingFlags.Instance | BindingFlags.NonPublic));
    }

    [Fact]
    public void LazyGetMethod_UnknownPrivateMethod_ThrowsException()
    {
        var targetType = typeof(TestClass);
        var typeLazy = new Lazy<Type>(() => targetType);
        var actualMethod = typeLazy.LazyGetMethod("UnknownMethod", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.Throws<InvalidOperationException>(() => actualMethod.Value);
    }

    [Fact]
    public void GetMethod_ReturnsMethod()
    {
        var targetType = typeof(TestClass);
        var expectedMethod = targetType.GetMethod(nameof(TestClass.PublicMethod));

        var typeLazy = new Lazy<Type>(() => targetType);
        var actualMethod = typeLazy.GetMethod(nameof(TestClass.PublicMethod));

        Assert.Equal(expectedMethod, actualMethod);
    }

    [Fact]
    public void GetMethod_NullType_ThrowsException()
    {
        Lazy<Type> typeLazy = null;
        Assert.Throws<ArgumentNullException>(() => typeLazy.GetMethod(nameof(TestClass.PublicMethod)));
    }

    [Fact]
    public void GetMethod_UnknownMethod_ThrowsException()
    {
        var targetType = typeof(TestClass);
        var typeLazy = new Lazy<Type>(() => targetType);
        Assert.Throws<InvalidOperationException>(() => typeLazy.GetMethod("UnknownMethod"));
    }

    [Fact]
    public void GetMethod_PrivateMethod_ReturnsMethod()
    {
        var targetType = typeof(TestClass);
        var expectedMethod = targetType.GetMethod("PrivateMethod", BindingFlags.Instance | BindingFlags.NonPublic);

        var typeLazy = new Lazy<Type>(() => targetType);
        var actualMethod = typeLazy.GetMethod("PrivateMethod", BindingFlags.Instance | BindingFlags.NonPublic);

        Assert.Equal(expectedMethod, actualMethod);
    }

    [Fact]
    public void GetMethod_PrivateMethodNullType_ThrowsException()
    {
        Lazy<Type> typeLazy = null;
        Assert.Throws<ArgumentNullException>(() =>
            typeLazy.GetMethod("PrivateMethod", BindingFlags.Instance | BindingFlags.NonPublic));
    }

    [Fact]
    public void GetMethod_UnknownPrivateMethod_ThrowsException()
    {
        var targetType = typeof(TestClass);
        var typeLazy = new Lazy<Type>(() => targetType);
        Assert.Throws<InvalidOperationException>(() =>
            typeLazy.GetMethod("UnknownMethod", BindingFlags.Instance | BindingFlags.NonPublic));
    }

    [Fact]
    public void LazyGetProperty_ReturnsGetMethod()
    {
        var targetType = typeof(TestClass);
        var expectedMethod = targetType.GetProperty(nameof(TestClass.Property))!.GetMethod;

        var typeLazy = new Lazy<Type>(() => targetType);
        var actualMethod = typeLazy.LazyGetProperty(nameof(TestClass.Property), getMethod: true);

        Assert.Equal(expectedMethod, actualMethod.Value);
    }

    [Fact]
    public void LazyGetProperty_ReturnsSetMethod()
    {
        var targetType = typeof(TestClass);
        var expectedMethod = targetType.GetProperty(nameof(TestClass.Property))!.SetMethod;

        var typeLazy = new Lazy<Type>(() => targetType);
        var actualMethod = typeLazy.LazyGetProperty(nameof(TestClass.Property), getMethod: false);

        Assert.Equal(expectedMethod, actualMethod.Value);
    }

    [Fact]
    public void CreateNewInstance_ReturnsNewInstance()
    {
        var targetType = typeof(TestClass);

        var actualResult = targetType.CreateInstance<TestClass>();

        Assert.NotNull(actualResult);
        Assert.IsType<TestClass>(actualResult);
    }

    [Fact]
    public void CreateNewInstance_NonDefaultCtor_ReturnsNewInstance()
    {
        var targetTypeLazy = new Lazy<Type>(() => typeof(TestClass));

        var actualResult = targetTypeLazy.CreateInstance("test value");

        Assert.NotNull(actualResult);
        Assert.IsType<TestClass>(actualResult);
        Assert.Equal("test value", ((TestClass)actualResult).Property);
    }

    [Fact]
    public void Invoke_NonLazy_InvokesTheMethod()
    {
        var targetInstance = new TestClass();
        var targetMethod = typeof(TestClass).GetMethod(nameof(TestClass.PublicMethod));

        var actualResult = targetMethod.Invoke<string>(targetInstance, new object[] { "test value" });

        Assert.Equal("test value", actualResult);
    }

    [Fact]
    public void Invoke_NonLazyNullMethod_ThrowsException()
    {
        var targetInstance = new TestClass();
        MethodInfo targetMethod = null;

        Assert.Throws<ArgumentNullException>(() =>
            targetMethod.Invoke<string>(targetInstance, new object[] { "test value" }));
    }

    [Fact]
    public void Invoke_Lazy_InvokesTheMethod()
    {
        var targetInstance = new TestClass();
        var targetMethod = typeof(TestClass).GetMethod(nameof(TestClass.PublicMethod));
        var targetMethodLazy = new Lazy<MethodInfo>(() => targetMethod);

        var actualResult = targetMethodLazy.Invoke<string>(targetInstance, new object[] { "test value" });

        Assert.Equal("test value", actualResult);
    }

    [Fact]
    public void Invoke_LazyNullMethod_ThrowsException()
    {
        var targetInstance = new TestClass();
        Lazy<MethodInfo> targetMethodLazy = null;
        Assert.Throws<ArgumentNullException>(() =>
            targetMethodLazy.Invoke<string>(targetInstance, new object[] { "test value" }));
    }

    [Fact]
    public void InvokeStatic_NonGeneric_InvokesTheMethod()
    {
        var targetMethod = typeof(TestClass).GetMethod(nameof(TestClass.StaticMethod));

        var actualResult = targetMethod.InvokeStatic(new object[] { "test value" });

        Assert.IsType<string>(actualResult);
        Assert.Equal("test value", (string)actualResult);
    }

    [Fact]
    public void InvokeStatic_GenericNullMethod_ThrowsException()
    {
        MethodInfo targetMethod = null;
        Assert.Throws<ArgumentNullException>(() => targetMethod.InvokeStatic<string>(new object[] { "test value" }));
    }

    [Fact]
    public void InvokeStatic_Generic_InvokesTheMethod()
    {
        var targetMethod = typeof(TestClass).GetMethod(nameof(TestClass.StaticMethod));

        var actualResult = targetMethod.InvokeStatic<string>(new object[] { "test value" });

        Assert.Equal("test value", actualResult);
    }

    [Fact]
    public void InvokeStatic_LazyNonGeneric_InvokesTheMethod()
    {
        var targetMethod = typeof(TestClass).GetMethod(nameof(TestClass.StaticMethod));
        var targetMethodLazy = new Lazy<MethodInfo>(() => targetMethod);

        var actualResult = targetMethodLazy.InvokeStatic(new object[] { "test value" });

        Assert.IsType<string>(actualResult);
        Assert.Equal("test value", (string)actualResult);
    }

    [Fact]
    public void InvokeStatic_LazyGeneric_InvokesTheMethod()
    {
        var targetMethod = typeof(TestClass).GetMethod(nameof(TestClass.StaticMethod));
        var targetMethodLazy = new Lazy<MethodInfo>(() => targetMethod);

        var actualResult = targetMethodLazy.InvokeStatic<string>(new object[] { "test value" });

        Assert.Equal("test value", actualResult);
    }

    [Fact]
    public void InvokeStatic_LazyGenericNullMethod_ThrowsException()
    {
        Lazy<MethodInfo> targetMethodLazy = null;
        Assert.Throws<ArgumentNullException>(() =>
            targetMethodLazy.InvokeStatic<string>(new object[] { "test value" }));
    }

    [Fact]
    public void InvokeStatic_LazyType_InvokesTheMethod()
    {
        var targetTypeLazy = new Lazy<Type>(() => typeof(TestClass));

        var actualResult =
            targetTypeLazy.InvokeStatic<string>(nameof(TestClass.StaticMethod), new object[] { "test value" });

        Assert.Equal("test value", actualResult);
    }

    [Fact]
    public void InvokeStatic_LazyTypeNullMethod_ThrowsException()
    {
        Lazy<Type> targetTypeLazy = null;

        Assert.Throws<ArgumentNullException>(() =>
            targetTypeLazy.InvokeStatic<string>(nameof(TestClass.StaticMethod), new object[] { "test value" }));
    }

    [Fact]
    public void LazyGetField_ReturnsField()
    {
        var targetTypeLazy = new Lazy<Type>(() => typeof(TestClass));
        var expectedResult = typeof(TestClass).GetField("_property", BindingFlags.Instance | BindingFlags.NonPublic);

        var actualResult = targetTypeLazy.LazyGetField("_property", BindingFlags.Instance | BindingFlags.NonPublic);

        Assert.NotNull(actualResult.Value);
        Assert.Equal(expectedResult, actualResult.Value);
    }

    [Fact]
    public void GetValue_ReturnsTheValue()
    {
        var targetField = typeof(TestClass).GetField("_property", BindingFlags.Instance | BindingFlags.NonPublic);
        var targetFieldLazy = new Lazy<FieldInfo>(() => targetField);
        var targetInstance = new TestClass
        {
            Property = "test value"
        };

        var actualResult = targetFieldLazy.GetValue<string>(targetInstance);

        Assert.Equal("test value", actualResult);
    }
}
