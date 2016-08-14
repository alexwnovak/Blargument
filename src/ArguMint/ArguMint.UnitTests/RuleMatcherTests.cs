﻿using System;
using FluentAssertions;
using Moq;
using ArguMint.TestCommon.Helpers;

namespace ArguMint.UnitTests
{
   public class RuleMatcherTests
   {
      public void Match_NoMarkedPropertiesFound_ThrowsArgumentConfigurationException()
      {
         IMarkedProperty<ArgumentAttribute>[] markedProperties = new IMarkedProperty<ArgumentAttribute>[0];

         // Arrange

         var typeInspectorMock = new Mock<ITypeInspector>();
         typeInspectorMock.Setup( ti => ti.GetMarkedProperties<ArgumentAttribute>( It.IsAny<Type>() ) ).Returns( markedProperties );

         // Act

         object argumentClass = "DoesNotMatterWhatThisIs";

         var ruleMatcher = new RuleMatcher( null, typeInspectorMock.Object );
         Action match = () => ruleMatcher.Match( argumentClass, null );

         // Assert

         match.ShouldThrow<ArgumentConfigurationException>();
      }

      public void Match_HasArgumentsAndOneProperty_MatchesPropertyAgainstAllRules()
      {
         var propertyOneMock = new Mock<IMarkedProperty<ArgumentAttribute>>();
         var propertyTwoMock = new Mock<IMarkedProperty<ArgumentAttribute>>();
         var ruleOneMock = new Mock<IArgumentRule>();
         var ruleTwoMock = new Mock<IArgumentRule>();

         var markedProperties = ArrayHelper.Create( propertyOneMock.Object, propertyTwoMock.Object );
         var rules = ArrayHelper.Create( ruleOneMock.Object, ruleTwoMock.Object );

         // Arrange

         var ruleProviderMock = new Mock<IRuleProvider>();
         ruleProviderMock.Setup( rp => rp.GetRules() ).Returns( rules );

         var typeInspectorMock = new Mock<ITypeInspector>();
         typeInspectorMock.Setup( ti => ti.GetMarkedProperties<ArgumentAttribute>( It.IsAny<Type>() ) ).Returns( markedProperties );

         // Act

         object argumentClass = "DoesNotMatterWhatThisIs";
         var stringArgs = ArrayHelper.Create( "SomeArgument" );

         var ruleMatcher = new RuleMatcher( ruleProviderMock.Object, typeInspectorMock.Object );
         ruleMatcher.Match( argumentClass, stringArgs );

         // Assert

         ruleOneMock.Verify( r => r.Match( argumentClass, propertyOneMock.Object, stringArgs ), Times.Once() );
         ruleTwoMock.Verify( r => r.Match( argumentClass, propertyOneMock.Object, stringArgs ), Times.Once() );

         ruleOneMock.Verify( r => r.Match( argumentClass, propertyTwoMock.Object, stringArgs ), Times.Once() );
         ruleTwoMock.Verify( r => r.Match( argumentClass, propertyTwoMock.Object, stringArgs ), Times.Once() );
      }
   }
}
