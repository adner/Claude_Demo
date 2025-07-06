using System;
using System.Linq;
using FakeXrmEasy.Abstractions;
using FakeXrmEasy.Middleware;
using FakeXrmEasy.Plugins;
using Microsoft.Xrm.Sdk;
using Xunit;

namespace TestProject1
{

    public class UnitTest1
    {
        
        IXrmFakedContext _fakedContext = XrmFakedContextFactory.New(FakeXrmEasy.Abstractions.Enums.FakeXrmEasyLicense.NonCommercial);

        public UnitTest1()
        {
            System.Diagnostics.Trace.Listeners.Clear();
        }

        [Fact]
        public void Test_correct_formatting_of_mobilephone_number()
        {
            var pluginContext = _fakedContext.GetDefaultPluginContext();
            pluginContext.MessageName = "Update";

            var guid1 = Guid.NewGuid();
            var target = new Entity("contact") { Id = guid1 };
            target.Attributes.Add("mobilephone", "0723210114");

            ParameterCollection inputParameters = new ParameterCollection();
            inputParameters.Add("Target", target);

            ParameterCollection outputParameters = new ParameterCollection();
            outputParameters.Add("id", guid1);

            pluginContext.InputParameters = inputParameters;
            pluginContext.OutputParameters = outputParameters;

            _fakedContext.ExecutePluginWith<PluginTest.Plugin1>(pluginContext);

            Assert.Equal("(072) 321-0114", target["mobilephone"]);
        }

        [Fact]
        public void Test_formatting_with_us_phone_number()
        {
            var pluginContext = _fakedContext.GetDefaultPluginContext();
            pluginContext.MessageName = "Update";

            var guid1 = Guid.NewGuid();
            var target = new Entity("contact") { Id = guid1 };
            target.Attributes.Add("mobilephone", "1234567890");

            ParameterCollection inputParameters = new ParameterCollection();
            inputParameters.Add("Target", target);

            ParameterCollection outputParameters = new ParameterCollection();
            outputParameters.Add("id", guid1);

            pluginContext.InputParameters = inputParameters;
            pluginContext.OutputParameters = outputParameters;

            _fakedContext.ExecutePluginWith<PluginTest.Plugin1>(pluginContext);

            Assert.Equal("(123) 456-7890", target["mobilephone"]);
        }

        [Fact]
        public void Test_formatting_with_seven_digit_number()
        {
            var pluginContext = _fakedContext.GetDefaultPluginContext();
            pluginContext.MessageName = "Update";

            var guid1 = Guid.NewGuid();
            var target = new Entity("contact") { Id = guid1 };
            target.Attributes.Add("mobilephone", "4567890");

            ParameterCollection inputParameters = new ParameterCollection();
            inputParameters.Add("Target", target);

            ParameterCollection outputParameters = new ParameterCollection();
            outputParameters.Add("id", guid1);

            pluginContext.InputParameters = inputParameters;
            pluginContext.OutputParameters = outputParameters;

            _fakedContext.ExecutePluginWith<PluginTest.Plugin1>(pluginContext);

            Assert.Equal("456-7890", target["mobilephone"]);
        }

        [Fact]
        public void Test_formatting_with_international_number()
        {
            var pluginContext = _fakedContext.GetDefaultPluginContext();
            pluginContext.MessageName = "Update";

            var guid1 = Guid.NewGuid();
            var target = new Entity("contact") { Id = guid1 };
            target.Attributes.Add("mobilephone", "+1234567890");

            ParameterCollection inputParameters = new ParameterCollection();
            inputParameters.Add("Target", target);

            ParameterCollection outputParameters = new ParameterCollection();
            outputParameters.Add("id", guid1);

            pluginContext.InputParameters = inputParameters;
            pluginContext.OutputParameters = outputParameters;

            _fakedContext.ExecutePluginWith<PluginTest.Plugin1>(pluginContext);

            Assert.Equal("+1 23 4567 890", target["mobilephone"]);
        }

        [Fact]
        public void Test_formatting_with_international_uk_number()
        {
            var pluginContext = _fakedContext.GetDefaultPluginContext();
            pluginContext.MessageName = "Update";

            var guid1 = Guid.NewGuid();
            var target = new Entity("contact") { Id = guid1 };
            target.Attributes.Add("mobilephone", "+4412345678");

            ParameterCollection inputParameters = new ParameterCollection();
            inputParameters.Add("Target", target);

            ParameterCollection outputParameters = new ParameterCollection();
            outputParameters.Add("id", guid1);

            pluginContext.InputParameters = inputParameters;
            pluginContext.OutputParameters = outputParameters;

            _fakedContext.ExecutePluginWith<PluginTest.Plugin1>(pluginContext);

            Assert.Equal("+44 12 3456 78", target["mobilephone"]);
        }

        [Fact]
        public void Test_no_formatting_when_mobilephone_not_present()
        {
            var pluginContext = _fakedContext.GetDefaultPluginContext();
            pluginContext.MessageName = "Update";

            var guid1 = Guid.NewGuid();
            var target = new Entity("contact") { Id = guid1 };
            target.Attributes.Add("firstname", "John");

            ParameterCollection inputParameters = new ParameterCollection();
            inputParameters.Add("Target", target);

            ParameterCollection outputParameters = new ParameterCollection();
            outputParameters.Add("id", guid1);

            pluginContext.InputParameters = inputParameters;
            pluginContext.OutputParameters = outputParameters;

            _fakedContext.ExecutePluginWith<PluginTest.Plugin1>(pluginContext);

            Assert.False(target.Contains("mobilephone"));
        }

        [Fact]
        public void Test_no_formatting_when_mobilephone_is_null()
        {
            var pluginContext = _fakedContext.GetDefaultPluginContext();
            pluginContext.MessageName = "Update";

            var guid1 = Guid.NewGuid();
            var target = new Entity("contact") { Id = guid1 };
            target.Attributes.Add("mobilephone", null);

            ParameterCollection inputParameters = new ParameterCollection();
            inputParameters.Add("Target", target);

            ParameterCollection outputParameters = new ParameterCollection();
            outputParameters.Add("id", guid1);

            pluginContext.InputParameters = inputParameters;
            pluginContext.OutputParameters = outputParameters;

            _fakedContext.ExecutePluginWith<PluginTest.Plugin1>(pluginContext);

            Assert.Null(target["mobilephone"]);
        }

        [Fact]
        public void Test_no_formatting_when_mobilephone_is_empty()
        {
            var pluginContext = _fakedContext.GetDefaultPluginContext();
            pluginContext.MessageName = "Update";

            var guid1 = Guid.NewGuid();
            var target = new Entity("contact") { Id = guid1 };
            target.Attributes.Add("mobilephone", "");

            ParameterCollection inputParameters = new ParameterCollection();
            inputParameters.Add("Target", target);

            ParameterCollection outputParameters = new ParameterCollection();
            outputParameters.Add("id", guid1);

            pluginContext.InputParameters = inputParameters;
            pluginContext.OutputParameters = outputParameters;

            _fakedContext.ExecutePluginWith<PluginTest.Plugin1>(pluginContext);

            Assert.Equal("", target["mobilephone"]);
        }

        [Fact]
        public void Test_plugin_does_not_execute_for_create_message()
        {
            var pluginContext = _fakedContext.GetDefaultPluginContext();
            pluginContext.MessageName = "Create";

            var guid1 = Guid.NewGuid();
            var target = new Entity("contact") { Id = guid1 };
            target.Attributes.Add("mobilephone", "1234567890");

            ParameterCollection inputParameters = new ParameterCollection();
            inputParameters.Add("Target", target);

            ParameterCollection outputParameters = new ParameterCollection();
            outputParameters.Add("id", guid1);

            pluginContext.InputParameters = inputParameters;
            pluginContext.OutputParameters = outputParameters;

            _fakedContext.ExecutePluginWith<PluginTest.Plugin1>(pluginContext);

            Assert.Equal("1234567890", target["mobilephone"]);
        }

        [Fact]
        public void Test_plugin_does_not_execute_for_non_contact_entity()
        {
            var pluginContext = _fakedContext.GetDefaultPluginContext();
            pluginContext.MessageName = "Update";

            var guid1 = Guid.NewGuid();
            var target = new Entity("account") { Id = guid1 };
            target.Attributes.Add("telephone1", "1234567890");

            ParameterCollection inputParameters = new ParameterCollection();
            inputParameters.Add("Target", target);

            ParameterCollection outputParameters = new ParameterCollection();
            outputParameters.Add("id", guid1);

            pluginContext.InputParameters = inputParameters;
            pluginContext.OutputParameters = outputParameters;

            _fakedContext.ExecutePluginWith<PluginTest.Plugin1>(pluginContext);

            Assert.Equal("1234567890", target["telephone1"]);
        }

        [Fact]
        public void Test_plugin_handles_phone_with_special_characters()
        {
            var pluginContext = _fakedContext.GetDefaultPluginContext();
            pluginContext.MessageName = "Update";

            var guid1 = Guid.NewGuid();
            var target = new Entity("contact") { Id = guid1 };
            target.Attributes.Add("mobilephone", "(123) 456-7890");

            ParameterCollection inputParameters = new ParameterCollection();
            inputParameters.Add("Target", target);

            ParameterCollection outputParameters = new ParameterCollection();
            outputParameters.Add("id", guid1);

            pluginContext.InputParameters = inputParameters;
            pluginContext.OutputParameters = outputParameters;

            _fakedContext.ExecutePluginWith<PluginTest.Plugin1>(pluginContext);

            Assert.Equal("(123) 456-7890", target["mobilephone"]);
        }

        [Fact]
        public void Test_plugin_handles_non_standard_length_number()
        {
            var pluginContext = _fakedContext.GetDefaultPluginContext();
            pluginContext.MessageName = "Update";

            var guid1 = Guid.NewGuid();
            var target = new Entity("contact") { Id = guid1 };
            target.Attributes.Add("mobilephone", "12345");

            ParameterCollection inputParameters = new ParameterCollection();
            inputParameters.Add("Target", target);

            ParameterCollection outputParameters = new ParameterCollection();
            outputParameters.Add("id", guid1);

            pluginContext.InputParameters = inputParameters;
            pluginContext.OutputParameters = outputParameters;

            _fakedContext.ExecutePluginWith<PluginTest.Plugin1>(pluginContext);

            Assert.Equal("12345", target["mobilephone"]);
        }

        [Fact]
        public void Test_plugin_throws_exception_with_null_service_provider()
        {
            var plugin = new PluginTest.Plugin1();
            
            Assert.Throws<InvalidPluginExecutionException>(() => plugin.Execute(null));
        }

        [Fact]  
        public void Test_plugin_handles_missing_target_parameter()
        {
            var pluginContext = _fakedContext.GetDefaultPluginContext();
            pluginContext.MessageName = "Update";

            ParameterCollection inputParameters = new ParameterCollection();
            ParameterCollection outputParameters = new ParameterCollection();

            pluginContext.InputParameters = inputParameters;
            pluginContext.OutputParameters = outputParameters;

            _fakedContext.ExecutePluginWith<PluginTest.Plugin1>(pluginContext);
        }

        [Fact]
        public void Test_plugin_handles_invalid_target_parameter_type()
        {
            var pluginContext = _fakedContext.GetDefaultPluginContext();
            pluginContext.MessageName = "Update";

            ParameterCollection inputParameters = new ParameterCollection();
            inputParameters.Add("Target", "invalid_string_type");

            ParameterCollection outputParameters = new ParameterCollection();

            pluginContext.InputParameters = inputParameters;
            pluginContext.OutputParameters = outputParameters;

            _fakedContext.ExecutePluginWith<PluginTest.Plugin1>(pluginContext);
        }
    }
}
