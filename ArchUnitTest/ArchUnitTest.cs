using Application;
using ArchUnitNET.Domain;
using ArchUnitNET.Fluent;
using ArchUnitNET.Loader;
using ArchUnitNET.xUnit;
using Domain;
using Infrastructure;
using Web;
using Xunit;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace ArchUnitTest
{
    public class ArchUnitTest
    {
        private static readonly Architecture Architecture = new ArchLoader()
            .LoadAssemblies(System.Reflection.Assembly.Load(typeof(WebClass).Assembly.GetName()))
            .LoadAssemblies(System.Reflection.Assembly.Load(typeof(DomainClass).Assembly.GetName()))
            .LoadAssemblies(System.Reflection.Assembly.Load(typeof(ApplicationClass).Assembly.GetName()))
            .LoadAssemblies(System.Reflection.Assembly.Load(typeof(InfraClass).Assembly.GetName()))
            .Build();

        private readonly IObjectProvider<IType> WebLayer =
            Types().That()
            .ResideInAssembly(System.Reflection.Assembly.Load(typeof(WebClass).Assembly.GetName()))
            .As("Web Layer");

        private readonly IObjectProvider<IType> DomainLayer =
            Types().That()
            .ResideInAssembly(System.Reflection.Assembly.Load(typeof(DomainClass).Assembly.GetName()))
            .As("Domain Layer");

        private readonly IObjectProvider<IType> ApplicationLayer =
            Types().That()
            .ResideInAssembly(System.Reflection.Assembly.Load(typeof(ApplicationClass).Assembly.GetName()))
            .As("Application Layer");

        private readonly IObjectProvider<IType> InfraLayer =
            Types().That()
            .ResideInAssembly(System.Reflection.Assembly.Load(typeof(InfraClass).Assembly.GetName()))
            .As("Infrastructure Layer");

        [Fact]
        public void Web_layer_should_never_use_the_domain_layer()
        {
            IArchRule layerRule = Types().That().Are(WebLayer).Should().NotDependOnAny(DomainLayer)
                .Because("Web should not have any reference to the domain layer.");

            layerRule.Check(Architecture);
        }

        [Fact]
        public void Domain_layer_should_never_use_any_other_layers()
        {
            IArchRule rule = Types()
                    .That()
                    .Are(DomainLayer)
                    .Should().NotDependOnAny(WebLayer)
                    .AndShould().NotDependOnAny(ApplicationLayer)
                    .AndShould().NotDependOnAny(InfraLayer)
                    .Because("Domain should not have any references at all.");

            rule.Check(Architecture);
        }

        [Fact]
        public void Infrastructure_layer_clients_should_not_use_domain_layer_types()
        {
            const string caseInsensitiveRegex = @"(?i)\\*Client";

            IArchRule rule =
                Classes().That().Are(InfraLayer).And()
                .ImplementInterface(caseInsensitiveRegex, true)
                .Should().NotDependOnAny(DomainLayer)
                .And().Classes()
                .That().HaveName(caseInsensitiveRegex, true)
                .Should().NotDependOnAny(DomainLayer)
                .Because("The domain objects should not be used inside client classes.");

            rule.Check(Architecture);
        }
    }
}
